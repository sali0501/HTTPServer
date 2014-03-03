using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace HTTPserver
{
    public class Receiver
    {
        public Receiver()
        {
            string name = "localhost";
            IPAddress[] addrs = Dns.GetHostEntry(name).AddressList;

            TcpListener serverSocket = new TcpListener(6789);
            serverSocket.Start();

            while (true)
            {
                Socket connectionSocket = serverSocket.AcceptSocket();
                Console.WriteLine("Server activated now");

                Task t = Task.Run(() => new Echo(connectionSocket));
            }
        }
    }
}
