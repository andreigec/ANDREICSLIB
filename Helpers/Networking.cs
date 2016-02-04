using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Threading;

namespace ANDREICSLIB.Helpers
{
    /// <summary>
    ///     example usage: https://github.com/andreigec/FireWind
    /// </summary>
    public class Networking
    {
        #region tcp

        /// <summary>
        /// Stuns the server.
        /// </summary>
        /// <param name="listenPort">The listen port.</param>
        /// <param name="sleepMS">The sleep ms.</param>
        public static void STUNServer(int listenPort = 801, int sleepMS = 100)
        {
            var tcpl = new TcpListener(IPAddress.Any, listenPort);
            tcpl.Start();

            var pending = new List<TcpClient>();

            //get two connections
            while (pending.Count < 2)
            {
                if (tcpl.Pending())
                {
                    var c = tcpl.AcceptTcpClient();

                    if (!pending.Any(x => x.Client.RemoteEndPoint.Equals(c.Client.RemoteEndPoint)))
                        pending.Add(c);
                }

                Thread.Sleep(sleepMS);
            }
        }

        #endregion tcp

        #region udp1

        /// <summary>
        /// Sends the UDP packet get blocking response.
        /// </summary>
        /// <param name="address">The address.</param>
        /// <param name="port">The port.</param>
        /// <param name="data">The data.</param>
        /// <returns></returns>
        public static byte[] SendUDPPacketGetBlockingResponse(IPAddress address, int port, byte[] data)
        {
            var ep = new IPEndPoint(address, port);
            var S = new Socket(AddressFamily.InterNetwork, SocketType.Dgram, ProtocolType.Udp);

            S.SendTo(data, ep);

            var rec = new byte[10000];
            rec[0] = 0;
            S.Blocking = true;
            S.ReceiveTimeout = 500;
            S.SendTimeout = 500;

            try
            {
                S.Receive(rec);
            }
            catch
            {
                return null;
            }

            return rec;
        }


        /// <summary>
        /// Gets or sets the UDP listener.
        /// </summary>
        /// <value>
        /// The UDP listener.
        /// </value>
        public UdpClient UDPListener { get; set; }
        /// <summary>
        /// Gets or sets the UDP senders.
        /// </summary>
        /// <value>
        /// The UDP senders.
        /// </value>
        public List<UdpClient> UDPSenders { get; set; }

        /// <summary>
        /// Adds the UDP client.
        /// </summary>
        /// <param name="udpPort">The UDP port.</param>
        /// <param name="ip">The ip.</param>
        /// <param name="receiveTimeout">The receive timeout.</param>
        public void AddUdpClient(int udpPort, string ip = null, int receiveTimeout = 2000)
        {
            UdpClient udpClient = null;
            if (ip != null)
                udpClient = new UdpClient(ip, udpPort);
            else
                udpClient = new UdpClient(udpPort);

            SetUDPTimeout(udpClient, receiveTimeout);

            if (ip == null)
            {
                if (UDPListener != null)
                    StopUDPClient(UDPListener);
                UDPListener = udpClient;
            }

            if (UDPSenders.Where(s => GetSocketInfo(s).Item1 == ip && GetSocketInfo(s).Item2 == udpPort).Count() != 0)
                return;

            UDPSenders.Add(udpClient);
        }

        /// <summary>
        /// Gets the socket information.
        /// </summary>
        /// <param name="uc">The uc.</param>
        /// <returns></returns>
        private static Tuple<string, int> GetSocketInfo(UdpClient uc)
        {
            var lep = ((IPEndPoint) uc.Client.LocalEndPoint);
            return new Tuple<string, int>(lep.Address.ToString(), lep.Port);
        }

        /// <summary>
        /// Stops the UDP client.
        /// </summary>
        /// <param name="udpClient">The UDP client.</param>
        public static void StopUDPClient(UdpClient udpClient)
        {
            if (udpClient != null)
            {
                udpClient.Close();
            }
        }

        /// <summary>
        /// Stops all.
        /// </summary>
        public void StopAll()
        {
            StopAllUDP();
        }

        /// <summary>
        /// Stops all UDP.
        /// </summary>
        public void StopAllUDP()
        {
            StopUDPClient(UDPListener);
            if (UDPSenders != null)
            {
                foreach (var u in UDPSenders)
                {
                    StopUDPClient(u);
                }
            }
        }

        /// <summary>
        /// Sets the UDP timeout.
        /// </summary>
        /// <param name="uc">The uc.</param>
        /// <param name="timeout">The timeout.</param>
        private void SetUDPTimeout(UdpClient uc, int timeout)
        {
            uc.Client.ReceiveTimeout = timeout;
            uc.Client.SetSocketOption(SocketOptionLevel.Socket, SocketOptionName.ReceiveTimeout, timeout);
        }

        #endregion udp
    }
}