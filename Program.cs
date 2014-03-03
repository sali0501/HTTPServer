using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace HTTPserver
{
    class Program
    {

        private static readonly string RootCatalog = "C:/temporary";

        static void Main(string[] args)
        {
            string name = "localhost";
            IPAddress[] addrs = Dns.GetHostEntry(name).AddressList;

            TcpListener serverSocket = new TcpListener(6789);
            serverSocket.Start();

            while (true)
            {
                Socket connectionSocket = serverSocket.AcceptSocket();
                Console.WriteLine("Server activated now");

                Task t = Task.Run(() => DoIt(connectionSocket));

                //Thread thread = new Thread(() => DoIt(connectionSocket));
                //thread.Start();
            }

           serverSocket.Stop();
        }

        public static void DoIt(Socket connectionSocket)
        {
            Stream ns = new NetworkStream(connectionSocket);
            StreamReader sr = new StreamReader(ns);
            StreamWriter sw = new StreamWriter(ns);
            
            sw.AutoFlush = true; // enable automatic flushing

            string message = sr.ReadLine();
            string firstLine = message;

            while (!string.IsNullOrEmpty(message))
            {
                Console.WriteLine("Client: " + message);
                message = sr.ReadLine();
            }

            sw.WriteLine("HTTP/1.0 200 OK \r\n");
            sw.WriteLine("\r\n");

            string[] words = firstLine.Split(' ');
            if (IsFileRequested(words[1]))
            {
                try
                {
                    using (FileStream fs = File.Open(RootCatalog + words[1], FileMode.Open))
                    {
                        fs.CopyTo(ns);
                        sw.WriteLine(ns);
                    }
                }
                catch (FileNotFoundException e)
                {
                    sw.WriteLine("Invalid request!");
                }
            }
            else
            {
                sw.WriteLine("No request");
            }

            connectionSocket.Close();
        }

        /**
         * Checks if theres a request from browser
         */
        public static bool IsFileRequested(string text)
        {
            return !text.Equals("/");
        }

        public static void ReadFileRequested(string fileRequested)
        {
            
        }
    }
}
