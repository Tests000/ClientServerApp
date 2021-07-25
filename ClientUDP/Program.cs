using System;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;

namespace ClientTCP
{
    class Program
    {
        static void Main(string[] args)
        {

            const string ip = "127.0.0.1";
            const int port = 8082;
            var udpEndPoint = new IPEndPoint(IPAddress.Parse(ip), port);

            var udpSocket = new Socket(AddressFamily.InterNetwork, SocketType.Dgram, ProtocolType.Udp);
            udpSocket.Bind(udpEndPoint);

            Thread thread = new Thread(new ParameterizedThreadStart((object server) => {
                while (true)
                {
                    var message = Console.ReadLine();
                    udpSocket.SendTo(Encoding.UTF8.GetBytes(message), (IPEndPoint)server);
                }
            }));

            while (true)
            {
                var server = new IPEndPoint(IPAddress.Parse(ip), 8081);
                if (thread.ThreadState != ThreadState.Running)
                    thread.Start(server);

                var buffer = new byte[256];
                var size = 0;
                var data = new StringBuilder();
                EndPoint listener = new IPEndPoint(IPAddress.Any, 0);

                do
                {
                    size = udpSocket.ReceiveFrom(buffer, ref listener);
                    data.Append(Encoding.UTF8.GetString(buffer));
                } while (udpSocket.Available > 0);

                Console.WriteLine(data);
            }
        }
    }
}

