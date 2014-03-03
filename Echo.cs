using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace HTTPserver
{
    class Echo
    {

        private const string RootCatalog = "C:/temporary";

        public Echo(Socket connectionSocket)
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

            string[] words = firstLine.Split(' ');
            Console.WriteLine(words[1]);
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
        private static bool IsFileRequested(string text)
        {
            return !text.Equals("/");
        }
    }
}
