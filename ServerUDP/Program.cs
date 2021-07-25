using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;

namespace ServerTCP
{
    class Program
    {
        static void Main(string[] args)
        {
            const string ip = "127.0.0.1";
            const int port = 8081;
            var udpEndPoint = new IPEndPoint(IPAddress.Parse(ip), port);

            var udpSocket = new Socket(AddressFamily.InterNetwork, SocketType.Dgram, ProtocolType.Udp);

            udpSocket.Bind(udpEndPoint);

            var UserList = new List<EndPoint>();

            Thread thread = new Thread(new ThreadStart(() =>
            {
                string message;
                while (true)
                {
                    message = Console.ReadLine();
                    foreach (var i in UserList)
                    {
                        udpSocket.SendTo(Encoding.UTF8.GetBytes(message), i);
                    }
                }
            }));


            while (true)
            {
                var buffer = new byte[256];
                var size = 0;
                var data = new StringBuilder();
                EndPoint listener = new IPEndPoint(IPAddress.Any, 0);
                var ex = false;


                if (thread.ThreadState != ThreadState.Running)
                    thread.Start();
                do
                {
                    size = udpSocket.ReceiveFrom(buffer, ref listener);
                    data.Append(Encoding.UTF8.GetString(buffer, 0, size));
                } while (udpSocket.Available > 0);

                foreach (var i in UserList)
                    if (i == listener)
                    {
                        ex = true;
                        break;
                    }
                if (!ex && listener.ToString() != "0.0.0.0:0")
                    UserList.Add(listener);

                Console.WriteLine($"{listener.ToString().Split(':')[1]}: {data}");
            }
        }
    }
}