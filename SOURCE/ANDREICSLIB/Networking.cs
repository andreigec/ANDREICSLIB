using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;

namespace ANDREICSLIB
{
    public class Networking
    {
        public UdpClient UDPListener =null;
        public List<UdpClient> UDPSenders = null;

        public void AddUDPClient(int udpPort, String ip = null, int receiveTimeout=2000)
        {
            UdpClient udpClient = null;
            if (ip != null)
                udpClient = new UdpClient(ip, udpPort);
            else
                udpClient = new UdpClient(udpPort);

            SetUDPTimeout(udpClient, receiveTimeout);

            if (ip==null)
            {
                if (UDPListener!=null)
                    StopUDPClient(UDPListener);
                UDPListener = udpClient;
            }

            if (UDPSenders.Where(s => GetSocketInfo(s).Item1 == ip && GetSocketInfo(s).Item2 == udpPort).Count() != 0)
                return;

            UDPSenders.Add(udpClient);
        }

        private static Tuple<string,int> GetSocketInfo(UdpClient uc)
        {
            var lep = ((IPEndPoint) uc.Client.LocalEndPoint);
            return new Tuple<string, int>(lep.Address.ToString(), lep.Port);
        }

        public static void StopUDPClient(UdpClient udpClient)
        {
            if (udpClient != null)
            {
                udpClient.Close();
            }
        }

        public void StopAll()
        {
            StopAllUDP();
        }

        public void StopAllUDP()
        {
            StopUDPClient(UDPListener);
            if (UDPSenders!=null)
            {
                foreach( var u in UDPSenders)
                {
                    StopUDPClient(u);
                }
            }
        }

        private void SetUDPTimeout(UdpClient uc, int timeout)
        {
            uc.Client.ReceiveTimeout = timeout;
            uc.Client.SetSocketOption(SocketOptionLevel.Socket, SocketOptionName.ReceiveTimeout, timeout);
        }
    }
}
