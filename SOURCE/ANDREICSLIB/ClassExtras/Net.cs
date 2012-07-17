using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Management;
using System.Net;
using System.Net.Sockets;
using System.Runtime.InteropServices;
using System.Text;
using System.Windows.Forms;

namespace ANDREICSLIB
{
    public abstract class Net
    {
        public static string DownloadWebPage(string url)
        {
            // Open a connection
            var webRequestObject = (HttpWebRequest)WebRequest.Create(url);

            // You can also specify additional header values like 
            // the user agent or the referer:
            webRequestObject.UserAgent = ".NET Framework/2.0";
            webRequestObject.Referer = "http://www.example.com/";

            // Request response:
            WebResponse response;
            try
            {
                response = webRequestObject.GetResponse();
            }
            catch (Exception ex)
            {
                return null;
            }

            // Open data stream:
            var webStream = response.GetResponseStream();

            // Create reader object:
            if (webStream != null)
            {
                var reader = new StreamReader(webStream);

                // Read the entire stream content:
                var pageContent = reader.ReadToEnd();

                // Cleanup
                reader.Close();
                webStream.Close();
                response.Close();

                return pageContent;
            }
            return null;
        }

        public static string GetHostName(IPAddress externalIP = null)
        {
            if (externalIP == null)
                return Dns.GetHostName();
            return Dns.GetHostEntry(externalIP).HostName;
        }

        public static bool IsIPAddress(String s)
        {
            try
            {
                IPAddress.Parse(s);
            }
            catch (Exception)
            {
                return false;
            }
            return true;
        }

        public static IPAddress GetCurrentIPAddress(string hostName, bool asIPV4 = true)
        {
            var he = Dns.GetHostEntry(hostName);

            IPAddress ip = null;
            try
            {
                foreach (var i in he.AddressList)
                {
                    if ((i.AddressFamily == AddressFamily.InterNetwork && asIPV4 || (i.AddressFamily == AddressFamily.InterNetworkV6 && asIPV4 == false)))
                    {
                        return IPAddress.Parse(i.ToString());
                    }
                }
            }
            catch (Exception ex)
            {
            }

            return null;
        }

        public static long GetAddressAsNumber(IPAddress ip)
        {
            var ab = ip.GetAddressBytes();

            if (ip.AddressFamily== AddressFamily.InterNetwork)
            {
                var a=BitConverter.ToInt32(ab, 0);
                return a;
            }
            
            if (ip.AddressFamily== AddressFamily.InterNetworkV6)
            return BitConverter.ToInt64(ab, 0);

            return 0;
        }

        #region mac address
        [DllImport("iphlpapi.dll", ExactSpelling = true)]
        private static extern int SendARP(int DestIP, int SrcIP, byte[] pMacAddr, ref uint PhyAddrLen);

        public static String GetMAC(IPAddress ip)
        {
            try
            {
                var hostEntry = Dns.GetHostEntry(ip);

                if (hostEntry.AddressList.Length == 0)
                    return null;

                var intAddress = (int)GetAddressAsNumber(ip);
                var macAddr = new byte[6];
                var macAddrLen = (uint)macAddr.Length;

                if (SendARP(intAddress, 0, macAddr, ref macAddrLen) != 0)
                    return null;

                var macAddressString = new StringBuilder();
                for (var i = 0; i < macAddr.Length; i++)
                {
                    if (macAddressString.Length > 0)
                        macAddressString.Append(":");
                    macAddressString.AppendFormat("{0:x2}", macAddr[i]);
                }

                return macAddressString.ToString();
            }
            catch (Exception ex)
            {
                return null;
            }
        }
        #endregion mac address

        #region netbios info
        [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Auto)]
        public struct WKSTA_INFO_100
        {
            public int wki100_platform_id;
            [MarshalAs(UnmanagedType.LPWStr)]
            public string wki100_computername;
            [MarshalAs(UnmanagedType.LPWStr)]
            public string wki100_langroup;
            public int wki100_ver_major;
            public int wki100_ver_minor;
        }

        public class NetBiosInfo
        {
            public int PlatformID;
            public string ComputerName;
            public string LANGroup;
            public int VerMajor;
            public int VerMinor;

            public NetBiosInfo(WKSTA_INFO_100 w)
            {
                PlatformID = w.wki100_platform_id;
                ComputerName = w.wki100_computername;
                LANGroup = w.wki100_langroup;
                VerMajor = w.wki100_ver_major;
                VerMinor = w.wki100_ver_minor;
            }
        }

        //group name/domain name/work group/ whatever its called
        [DllImport("netapi32.dll", CharSet = CharSet.Auto)]
        private static extern int NetWkstaGetInfo(string server,
                                                  int level,
                                                  out IntPtr info);
        private static WKSTA_INFO_100? getMachineNetBiosInfo(IPAddress ip)
        {
            var hostEntry = Dns.GetHostEntry(ip);

            if (hostEntry.AddressList.Length == 0)
                return null;

            var pBuffer = IntPtr.Zero;

            var retval = NetWkstaGetInfo(ip.ToString(), 100, out pBuffer);
            if (retval != 0)
                return null;

            return (WKSTA_INFO_100)Marshal.PtrToStructure(pBuffer, typeof(WKSTA_INFO_100));
        }

        public static NetBiosInfo GetNetBiosInfo(IPAddress ip)
        {
            var wk = getMachineNetBiosInfo(ip);
            if (wk == null)
                return null;

            var nbi = new NetBiosInfo((WKSTA_INFO_100)wk);

            return nbi;
        }
        #endregion netbios info

        //admin rights required
        public static Dictionary<String, String> GetRemoteInfo(IPAddress ip, String hostname, int extraInfoTimeout)
        {
            var options = new ConnectionOptions();
            options.Impersonation = ImpersonationLevel.Impersonate;
            options.Authentication = AuthenticationLevel.None;

            var s = extraInfoTimeout;

            var H = s * 3600;
            s -= H * 3600;

            var M = s / 60;
            s -= M * 60;

            var S = s;

            var startTime = DateTime.Now;


            options.Timeout = new TimeSpan(H, M, S);

            var scope = new ManagementScope("\\\\" + ip + "\\root\\cimv2");
            try
            {
                //admin rights?
                scope.Connect();
                if (scope.IsConnected == false)
                    return null;
            }
            catch
            {
                return null;
            }

            //Query system for Operating System information      
            var query = new ObjectQuery("SELECT * FROM Win32_OperatingSystem");

            // To get information about Logical Disks on Computer
            var query1 = new SelectQuery("Select * from Win32_LogicalDisk");

            var searcher = new ManagementObjectSearcher(scope, query);
            var searcher1 = new ManagementObjectSearcher(scope, query1);

            var results = new Dictionary<string, string>();

            foreach (var envVar in searcher.Get())
            {
                foreach (var PD in envVar.Properties)
                {
                    var currentTime = DateTime.Now;
                    var TS = currentTime - startTime;

                    if (TS.TotalSeconds > extraInfoTimeout)
                    {
                        return results;
                    }

                    if (PD.Name != null && PD.Value != null && results.ContainsKey(PD.Name) == false)
                    {
                        string val = PD.Value.ToString();
                        if (PD.Value.GetType() == typeof(string[]))
                        {
                            var ss = PD.Value as string[];
                            val = ss.Aggregate("", (current, ss2) => current + (ss2 + "|"));
                        }

                        results.Add(PD.Name, val);
                    }
                }
            }

            foreach (var envVar in searcher1.Get())
            {
                var currentTime = DateTime.Now;
                var TS = currentTime - startTime;

                if (TS.TotalSeconds > extraInfoTimeout)
                {
                    return results;
                }

                foreach (var PD in envVar.Properties)
                {
                    if (PD.Name != null && PD.Value != null && results.ContainsKey(PD.Name) == false)
                        results.Add(PD.Name, PD.Value.ToString());
                }
            }
            return results;
        }

    }
}