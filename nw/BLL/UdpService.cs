using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace BLL
{
    public delegate void UdpServiceReciveDelege(string recStr);
    public class UdpService
    {
        UdpClient udp;
        IPEndPoint sendHost;
        UdpServiceReciveDelege udpServiceRecive;
        public UdpService(string localIP, string localPort, string sendIP, string sendPort, UdpServiceReciveDelege _udpServiceRecive)
        {
            udp = new UdpClient(new IPEndPoint(IPAddress.Parse(localIP), Convert.ToInt32(localPort)));

            if (!(sendIP == null || sendPort == null || sendIP == "" || sendPort == ""))
            {
                sendHost = new IPEndPoint(IPAddress.Parse(sendIP), Convert.ToInt32(sendPort));
            }

            udpServiceRecive = _udpServiceRecive;

            ThreadPool.QueueUserWorkItem(new WaitCallback((m) =>
            {
                IPEndPoint from = null;
                try
                {
                    while (true)
                    {
                        try
                        {
                            byte[] b = udp.Receive(ref from);
                            string str = Encoding.UTF8.GetString(b, 0, b.Length);

                            if (udpServiceRecive != null)
                                udpServiceRecive(str);

                            //Console.WriteLine(str);
                        }
                        catch (Exception e)
                        {
                            Console.WriteLine(e.Message);
                        }
                    }
                }
                catch (Exception e)
                {
                    udp.Close();
                }

            }
            ));
        }
        private void Send(string msg)
        {
            byte[] b = Encoding.UTF8.GetBytes(msg);
            udp.Send(b, b.Length, sendHost);
        }
    }
}
