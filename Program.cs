using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace HTTPserver
{
    class Program
    {
        static void Main(string[] args)
        {
            string name = "localhost";
            IPAddress[] addrs = Dns.GetHostEntry(name).AddressList;

            //TcpListener welcomeSocket = new TcpListener(addrs[1], 6789);
            //IPAddress ip = IPAddress.Parse("127.0.0.1");
            //TcpListener serverSocket = new TcpListener(ip, 6789);

            TcpListener serverSocket = new TcpListener(6789);
            serverSocket.Start();

            while (true)
            {
                Socket connectionSocket = serverSocket.AcceptSocket();
                Console.WriteLine("Server activated now");
                DoIt(connectionSocket);
            }

            serverSocket.Stop();
        }

        public static void DoIt(Socket connectionSocket)
        {
            Stream ns = new NetworkStream(connectionSocket);
            StreamReader sr = new StreamReader(ns);
            StreamWriter sw = new StreamWriter(ns);
            sw.WriteLine("GET HTTP/1.0 \n\n");
            sw.AutoFlush = true; // enable automatic flushing

            string message = sr.ReadLine();
            string answer;
            while (!string.IsNullOrEmpty(message))
            {
                Console.WriteLine("Client: " + message);
                answer = message.ToUpper();
                sw.WriteLine(answer);
                message = sr.ReadLine();

            }
            connectionSocket.Close();
        }
    }
}
