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
using System.Threading.Tasks;

namespace ANDREICSLIB.ClassExtras
{
    /// <summary>
    ///     example usage: https://github.com/andreigec/ExtractTransform
    /// </summary>
    public static class NetExtras
    {
        private class WebClientWithHeader : WebClient
        {
            public bool HeadOnly { get; set; }
            protected override WebRequest GetWebRequest(Uri address)
            {
                WebRequest req = base.GetWebRequest(address);
                req.Method = "HEAD";
                return req;
            }
        }

        public static bool UrlExists(string url)
        {
            using (WebClientWithHeader clientWithHeader = new WebClientWithHeader())
            {
                try
                {
                    clientWithHeader.DownloadData(url); // note should be 0-length
                    string type = clientWithHeader.ResponseHeaders["content-type"];
                    clientWithHeader.HeadOnly = false;
                    return (type.StartsWith(@"text/"));
                }
                //probably a 404
                catch (WebException ex)
                {
                    return (ex.Response is HttpWebResponse &&
                            ((HttpWebResponse)ex.Response).StatusCode == HttpStatusCode.NotFound);
                }
                //dont care
                catch
                {
                    return false;
                }
            }
        }

        /// <summary>
        /// Makes the string URL safe.
        /// </summary>
        /// <param name="instr">The instr.</param>
        /// <returns></returns>
        public static string MakeStringURLSafe(this string instr)
        {
            //changes
            instr = instr.Replace("://", "_");
            instr = instr.Replace("/", "_");
            instr = instr.Replace(":", "");
            instr = instr.Replace(".", "_");
            instr = instr.Replace("{", "_");
            instr = instr.Replace("}", "_");
            instr = instr.Replace("__", "_");
            instr = instr.Trim('_');
            return instr;
        }

        /// <summary>
        /// Downloads the file.
        /// </summary>
        /// <param name="url">The URL.</param>
        /// <param name="filename">The filename.</param>
        public static void DownloadFile(string url, string filename)
        {
            using (var client = new WebClient())
            {
                client.DownloadFile(url, filename);
            }
        }

        /// <summary>
        /// Downloads the web page.
        /// </summary>
        /// <param name="url">The URL.</param>
        /// <returns></returns>
        public static async Task<string> DownloadWebPage(string url)
        {
            var t = GetWebPageStream(url);
            var webStream = t.Item1;
            var response = t.Item2;

            // Create reader object:
            if (webStream != null)
            {
                var reader = new StreamReader(webStream);

                // Read the entire stream content:
                var pageContent = await reader.ReadToEndAsync();

                // Cleanup
                reader.Close();
                webStream.Close();
                response.Close();

                return pageContent;
            }
            return null;
        }

        /// <summary>
        /// Gets the web page stream.
        /// </summary>
        /// <param name="url">The URL.</param>
        /// <returns></returns>
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
            response = webRequestObject.GetResponse();


            // Open data stream:
            var webStream = response.GetResponseStream();
            return new Tuple<Stream, WebResponse>(webStream, response);
        }

        /// <summary>
        /// Gets the name of the host.
        /// </summary>
        /// <param name="externalIP">The external ip.</param>
        /// <returns></returns>
        public static string GetHostName(IPAddress externalIP = null)
        {
            if (externalIP == null)
                return Dns.GetHostName();
            return Dns.GetHostEntry(externalIP).HostName;
        }

        /// <summary>
        /// Determines whether [is lan ip] [the specified address].
        /// </summary>
        /// <param name="address">The address.</param>
        /// <returns></returns>
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

        /// <summary>
        /// Determines whether [is lan ip check TTL] [the specified address].
        /// </summary>
        /// <param name="address">The address.</param>
        /// <returns></returns>
        public static bool IsLanIPCheckTTL(IPAddress address)
        {
            var ping = new Ping();
            try
            {
                var rep = ping.Send(address, 100, new byte[] { 1 }, new PingOptions
                {
                    DontFragment = true,
                    Ttl = 1
                });
                if (rep != null)
                    return rep.Status != IPStatus.TtlExpired && rep.Status != IPStatus.TimedOut &&
                           rep.Status != IPStatus.TimeExceeded;
            }
            catch
            {
            }
            return false;
        }

        /// <summary>
        /// Checks the mask.
        /// </summary>
        /// <param name="address">The address.</param>
        /// <param name="mask">The mask.</param>
        /// <param name="target">The target.</param>
        /// <returns></returns>
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

                var a = ba[i] & m;
                var b = bb[i] & m;

                if (a != b)
                    return false;
            }

            return true;
        }

        /// <summary>
        /// Determines whether [is ip address] [the specified s].
        /// </summary>
        /// <param name="s">The s.</param>
        /// <returns></returns>
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

        /// <summary>
        /// Hostnames to ip.
        /// </summary>
        /// <param name="hostName">Name of the host.</param>
        /// <param name="asIPV4">if set to <c>true</c> [as ip v4].</param>
        /// <returns></returns>
        public static IPAddress HostnameToIP(string hostName, bool asIPV4 = true)
        {
            IPAddress ret;

            if (IPAddress.TryParse(hostName, out ret))
                return ret;

            var he = Dns.GetHostEntry(hostName);

            try
            {
                foreach (var i in he.AddressList)
                {
                    if ((i.AddressFamily == AddressFamily.InterNetwork && asIPV4 ||
                         (i.AddressFamily == AddressFamily.InterNetworkV6 && asIPV4 == false)))
                    {
                        return IPAddress.Parse(i.ToString());
                    }
                }
            }
            catch
            {
            }

            return null;
        }

        /// <summary>
        /// Gets the external default local address.
        /// </summary>
        /// <param name="WebCheckIPURL">The web check ipurl.</param>
        /// <returns></returns>
        public static async Task<string> GetExternalDefaultLocalAddress(string WebCheckIPURL = null)
        {
            var ret = await (WebCheckIPURL == null ? GetExternalAddress() : GetExternalAddress(WebCheckIPURL));

            if (ret == null)
                ret = GetLocalAddress();

            return ret;
        }

        /// <summary>
        /// Gets the local address.
        /// </summary>
        /// <returns></returns>
        public static string GetLocalAddress()
        {
            var host = Dns.GetHostEntry(Dns.GetHostName());
            return
                (from ip in host.AddressList where ip.AddressFamily == AddressFamily.InterNetwork select ip.ToString())
                    .FirstOrDefault();
        }

        /// <summary>
        /// Gets the external address.
        /// </summary>
        /// <param name="WebCheckIPURL">The web check ipurl.</param>
        /// <returns></returns>
        public static async Task<string> GetExternalAddress(string WebCheckIPURL = "http://checkip.dyndns.org")
        {
            try
            {
                var addr = await DownloadWebPage(WebCheckIPURL);
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

        /// <summary>
        /// Ips to long.
        /// </summary>
        /// <param name="ip">The ip.</param>
        /// <returns></returns>
        public static long IPToLong(IPAddress ip)
        {
            var ab = ip.GetAddressBytes();

            if (ip.AddressFamily == AddressFamily.InterNetwork)
            {
                var a = BitConverter.ToInt32(ab, 0);
                return a;
            }

            if (ip.AddressFamily == AddressFamily.InterNetworkV6)
                return BitConverter.ToInt64(ab, 0);

            return 0;
        }

        /// <summary>
        /// Gets the mac.
        /// </summary>
        /// <param name="ip">The ip.</param>
        /// <returns></returns>
        public static string GetMAC(IPAddress ip)
        {
            try
            {
                var hostEntry = Dns.GetHostEntry(ip);

                if (hostEntry.AddressList.Length == 0)
                    return null;

                var intAddress = (int)IPToLong(ip);
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
            catch
            {
                return null;
            }
        }

        /// <summary>
        /// Gets the net bios information.
        /// </summary>
        /// <param name="ip">The ip.</param>
        /// <returns></returns>
        public static NetBiosInfo GetNetBiosInfo(IPAddress ip)
        {
            var wk = getMachineNetBiosInfo(ip);
            if (wk == null)
                return null;

            var nbi = new NetBiosInfo((WKSTA_INFO_100)wk);

            return nbi;
        }

        //admin rights required
        /// <summary>
        /// Gets the remote information.
        /// </summary>
        /// <param name="ip">The ip.</param>
        /// <param name="hostname">The hostname.</param>
        /// <param name="extraInfoTimeout">The extra information timeout.</param>
        /// <returns></returns>
        public static Dictionary<string, string> GetRemoteInfo(IPAddress ip, string hostname, int extraInfoTimeout)
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

                    if (PD.Value != null && results.ContainsKey(PD.Name) == false)
                    {
                        var val = PD.Value.ToString();
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
                    if (PD.Value != null && results.ContainsKey(PD.Name) == false)
                        results.Add(PD.Name, PD.Value.ToString());
                }
            }
            return results;
        }

        /// <summary>
        /// Gets the next free port.
        /// </summary>
        /// <param name="min">The minimum.</param>
        /// <param name="max">The maximum.</param>
        /// <returns></returns>
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

            for (var p = min; p < max; p++)
            {
                if (portArray.ContainsKey(p) == false)
                    return p;
            }

            return -1;
        }

        #region mac address

        [DllImport("iphlpapi.dll", ExactSpelling = true)]
        private static extern int SendARP(int DestIP, int SrcIP, byte[] pMacAddr, ref uint PhyAddrLen);

        #endregion mac address

        /// <summary>
        ///     add timeout to web client
        /// </summary>
        public class WebClientEx : WebClient
        {
            /// <summary>
            ///     timeout in MS
            /// </summary>
            public int Timeout { get; set; }

            /// <summary>
            /// Gets the web request.
            /// </summary>
            /// <param name="address">The address.</param>
            /// <returns></returns>
            protected override WebRequest GetWebRequest(Uri address)
            {
                var request = base.GetWebRequest(address);
                request.Timeout = Timeout;
                return request;
            }
        }

        #region netbios info

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

        #region Nested type: NetBiosInfo

        /// <summary>
        /// 
        /// </summary>
        public class NetBiosInfo
        {
            /// <summary>
            /// Gets or sets the name of the computer.
            /// </summary>
            /// <value>
            /// The name of the computer.
            /// </value>
            public string ComputerName { get; set; }
            /// <summary>
            /// Gets or sets the lan group.
            /// </summary>
            /// <value>
            /// The lan group.
            /// </value>
            public string LANGroup { get; set; }
            /// <summary>
            /// Gets or sets the platform identifier.
            /// </summary>
            /// <value>
            /// The platform identifier.
            /// </value>
            public int PlatformID { get; set; }
            /// <summary>
            /// Gets or sets the ver major.
            /// </summary>
            /// <value>
            /// The ver major.
            /// </value>
            public int VerMajor { get; set; }
            /// <summary>
            /// Gets or sets the ver minor.
            /// </summary>
            /// <value>
            /// The ver minor.
            /// </value>
            public int VerMinor { get; set; }

            /// <summary>
            /// Initializes a new instance of the <see cref="NetBiosInfo"/> class.
            /// </summary>
            /// <param name="w">The w.</param>
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

        /// <summary>
        /// 
        /// </summary>
        [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Auto)]
        public struct WKSTA_INFO_100
        {
            /// <summary>
            /// Gets or sets the wki100_platform_id.
            /// </summary>
            /// <value>
            /// The wki100_platform_id.
            /// </value>
            public int wki100_platform_id { get; set; }
            /// <summary>
            /// The wki100_computername
            /// </summary>
            [MarshalAs(UnmanagedType.LPWStr)]
            public string wki100_computername;

            /// <summary>
            /// The wki100_langroup
            /// </summary>
            [MarshalAs(UnmanagedType.LPWStr)]
            public string wki100_langroup;
            /// <summary>
            /// Gets or sets the wki100_ver_major.
            /// </summary>
            /// <value>
            /// The wki100_ver_major.
            /// </value>
            public int wki100_ver_major { get; set; }
            /// <summary>
            /// Gets or sets the wki100_ver_minor.
            /// </summary>
            /// <value>
            /// The wki100_ver_minor.
            /// </value>
            public int wki100_ver_minor { get; set; }
        }

        #endregion

        #endregion netbios info
    }
}