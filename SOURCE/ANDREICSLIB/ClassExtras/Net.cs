using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Management;
using System.Net;
using System.Net.NetworkInformation;
using System.Net.Sockets;
using System.Runtime.InteropServices;
using System.Text;

namespace ANDREICSLIB.ClassExtras
{
    /// <summary>
    /// example usage: https://github.com/andreigec/ExtractTransform
    /// </summary>
    public abstract class NetExtras
    {
        /// <summary>
        /// add timeout to web client
        /// </summary>
        public class WebClientEx : WebClient
        {
            /// <summary>
            /// timeout in MS
            /// </summary>
            public int Timeout { get; set; }

            protected override WebRequest GetWebRequest(Uri address)
            {
                var request = base.GetWebRequest(address);
                request.Timeout = Timeout;
                return request;
            }
        }

        public static string MakeStringURLSafe(string instr)
        {
            //changes
            instr = instr.Replace("://", "_");
            instr = instr.Replace("/", "_");
            instr = instr.Replace(":", "");
            instr = instr.Replace(".", "_");
            instr = instr.Replace("{", "_");
            instr = instr.Replace("}", "_");
            instr = instr.Replace("__", "_");
            instr = instr.Trim(new[] { '_' });
            return instr;
        }


        public static void DownloadFile(string url, string filename)
        {
            using (WebClient client = new WebClient())
            {
                client.DownloadFile(url, filename);
            }
        }

        public static string DownloadWebPage(string url)
        {
            var t = GetWebPageStream(url);
            var webStream = t.Item1;
            var response = t.Item2;

            // Create reader object:
            if (webStream != null)
            {
                var reader = new StreamReader(webStream);

                // Read the entire stream content:
                string pageContent = reader.ReadToEnd();

                // Cleanup
                reader.Close();
                webStream.Close();
                response.Close();

                return pageContent;
            }
            return null;
        }

        public static Tuple<Stream, WebResponse> GetWebPageStream(string url)
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
            Stream webStream = response.GetResponseStream();
            return new Tuple<Stream, WebResponse>(webStream, response);
        }

        public static string GetHostName(IPAddress externalIP = null)
        {
            if (externalIP == null)
                return Dns.GetHostName();
            return Dns.GetHostEntry(externalIP).HostName;
        }

        public static bool IsLanIP(IPAddress address)
        {
            var interfaces = NetworkInterface.GetAllNetworkInterfaces();
            foreach (var iface in interfaces)
            {
                var properties = iface.GetIPProperties();
                foreach (var ifAddr in properties.UnicastAddresses)
                {
                    if (ifAddr.IPv4Mask != null &&
                        ifAddr.Address.AddressFamily == AddressFamily.InterNetwork &&
                        CheckMask(ifAddr.Address, ifAddr.IPv4Mask, address))
                        return true;
                }
            }
            return false;
        }

        public static bool IsLanIPCheckTTL(IPAddress address)
        {
            var ping = new Ping();
            try
            {
                var rep = ping.Send(address, 100, new byte[] { 1 }, new PingOptions()
                {
                    DontFragment = true,
                    Ttl = 1
                });
                if (rep != null)
                    return rep.Status != IPStatus.TtlExpired && rep.Status != IPStatus.TimedOut && rep.Status != IPStatus.TimeExceeded;
            }
            catch (Exception ex)
            {

            }
            return false;
        }

        private static bool CheckMask(IPAddress address, IPAddress mask, IPAddress target)
        {
            if (mask == null)
                return false;

            var ba = address.GetAddressBytes();
            var bm = mask.GetAddressBytes();
            var bb = target.GetAddressBytes();

            if (ba.Length != bm.Length || bm.Length != bb.Length)
                return false;

            for (var i = 0; i < ba.Length; i++)
            {
                int m = bm[i];

                int a = ba[i] & m;
                int b = bb[i] & m;

                if (a != b)
                    return false;
            }

            return true;
        }

        public static bool IsIPAddress(string s)
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

        public static IPAddress HostnameToIP(string hostName, bool asIPV4 = true)
        {
            IPAddress ret;

            if (IPAddress.TryParse(hostName, out ret))
                return ret;

            IPHostEntry he = Dns.GetHostEntry(hostName);

            try
            {
                foreach (IPAddress i in he.AddressList)
                {
                    if ((i.AddressFamily == AddressFamily.InterNetwork && asIPV4 ||
                         (i.AddressFamily == AddressFamily.InterNetworkV6 && asIPV4 == false)))
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

        public static string GetExternalDefaultLocalAddress(string WebCheckIPURL = null)
        {
            string ret = WebCheckIPURL == null ? GetExternalAddress() : GetExternalAddress(WebCheckIPURL);

            if (ret == null)
                ret = GetLocalAddress();

            return ret;
        }

        public static string GetLocalAddress()
        {
            IPHostEntry host = Dns.GetHostEntry(Dns.GetHostName());
            return (from ip in host.AddressList where ip.AddressFamily == AddressFamily.InterNetwork select ip.ToString()).FirstOrDefault();
        }

        public static string GetExternalAddress(string WebCheckIPURL = "http://checkip.dyndns.org")
        {
            try
            {
                string addr = DownloadWebPage(WebCheckIPURL);
                if (string.IsNullOrEmpty(addr) == false)
                {
                    //remove up to :
                    addr = addr.Substring(addr.IndexOf(':') + 2);
                    //remove html
                    addr = addr.Substring(0, addr.IndexOf('<'));

                    return addr;
                }
                return null;
            }
            catch (Exception)
            {
                return null;
            }
        }

        public static long IPToLong(IPAddress ip)
        {
            byte[] ab = ip.GetAddressBytes();

            if (ip.AddressFamily == AddressFamily.InterNetwork)
            {
                int a = BitConverter.ToInt32(ab, 0);
                return a;
            }

            if (ip.AddressFamily == AddressFamily.InterNetworkV6)
                return BitConverter.ToInt64(ab, 0);

            return 0;
        }

        public static string GetMAC(IPAddress ip)
        {
            try
            {
                IPHostEntry hostEntry = Dns.GetHostEntry(ip);

                if (hostEntry.AddressList.Length == 0)
                    return null;

                var intAddress = (int)IPToLong(ip);
                var macAddr = new byte[6];
                var macAddrLen = (uint)macAddr.Length;

                if (SendARP(intAddress, 0, macAddr, ref macAddrLen) != 0)
                    return null;

                var macAddressString = new StringBuilder();
                for (int i = 0; i < macAddr.Length; i++)
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

        public static NetBiosInfo GetNetBiosInfo(IPAddress ip)
        {
            WKSTA_INFO_100? wk = getMachineNetBiosInfo(ip);
            if (wk == null)
                return null;

            var nbi = new NetBiosInfo((WKSTA_INFO_100)wk);

            return nbi;
        }

        //admin rights required
        public static Dictionary<string, string> GetRemoteInfo(IPAddress ip, string hostname, int extraInfoTimeout)
        {
            var options = new ConnectionOptions();
            options.Impersonation = ImpersonationLevel.Impersonate;
            options.Authentication = AuthenticationLevel.None;

            int s = extraInfoTimeout;

            int H = s * 3600;
            s -= H * 3600;

            int M = s / 60;
            s -= M * 60;

            int S = s;

            DateTime startTime = DateTime.Now;


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

            foreach (ManagementBaseObject envVar in searcher.Get())
            {
                foreach (PropertyData PD in envVar.Properties)
                {
                    DateTime currentTime = DateTime.Now;
                    TimeSpan TS = currentTime - startTime;

                    if (TS.TotalSeconds > extraInfoTimeout)
                    {
                        return results;
                    }

                    if (PD.Value != null && results.ContainsKey(PD.Name) == false)
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

            foreach (ManagementBaseObject envVar in searcher1.Get())
            {
                DateTime currentTime = DateTime.Now;
                TimeSpan TS = currentTime - startTime;

                if (TS.TotalSeconds > extraInfoTimeout)
                {
                    return results;
                }

                foreach (PropertyData PD in envVar.Properties)
                {
                    if (PD.Value != null && results.ContainsKey(PD.Name) == false)
                        results.Add(PD.Name, PD.Value.ToString());
                }
            }
            return results;
        }

        public static int GetNextFreePort(int min = 1000, int max = 50000)
        {
            var listeners = IPGlobalProperties.GetIPGlobalProperties().GetActiveTcpListeners();
            var used = IPGlobalProperties.GetIPGlobalProperties().GetActiveTcpConnections();
            var portArray = new Dictionary<int, bool>();

            foreach (var l in listeners)
            {
                portArray.Add(l.Port, true);
            }

            foreach (var u in used)
            {
                portArray.Add(u.LocalEndPoint.Port, true);
            }

            for (int p = min; p < max; p++)
            {
                if (portArray.ContainsKey(p) == false)
                    return p;
            }

            return -1;
        }

        #region netbios info

        //group name/domain name/work group/ whatever its called
        [DllImport("netapi32.dll", CharSet = CharSet.Auto)]
        private static extern int NetWkstaGetInfo(string server,
                                                  int level,
                                                  out IntPtr info);

        private static WKSTA_INFO_100? getMachineNetBiosInfo(IPAddress ip)
        {
            IPHostEntry hostEntry = Dns.GetHostEntry(ip);

            if (hostEntry.AddressList.Length == 0)
                return null;

            IntPtr pBuffer = IntPtr.Zero;

            int retval = NetWkstaGetInfo(ip.ToString(), 100, out pBuffer);
            if (retval != 0)
                return null;

            return (WKSTA_INFO_100)Marshal.PtrToStructure(pBuffer, typeof(WKSTA_INFO_100));
        }

        #region Nested type: NetBiosInfo

        public class NetBiosInfo
        {
            public string ComputerName;
            public string LANGroup;
            public int PlatformID;
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

        #endregion

        #region Nested type: WKSTA_INFO_100

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

        #endregion

        #endregion netbios info

        #region mac address

        [DllImport("iphlpapi.dll", ExactSpelling = true)]
        private static extern int SendARP(int DestIP, int SrcIP, byte[] pMacAddr, ref uint PhyAddrLen);

        #endregion mac address
    }
}