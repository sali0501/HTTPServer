using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace HTTPserver
{
    public class Echo
    {
        private const string RootCatalog = "C:/temporary";

        private Stream ns;
        private StreamReader sr;
        private StreamWriter sw;

        public Echo(Socket connectionSocket)
        {
            ns = new NetworkStream(connectionSocket);
            sr = new StreamReader(ns);
            sw = new StreamWriter(ns);

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
                OpenFileRequested(words[1]);
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
        private bool IsFileRequested(string fileRequested)
        {
            return !fileRequested.Equals("/");
        }

        private void OpenFileRequested(string fileRequested)
        {
            try
            {
                using (FileStream fs = File.Open(RootCatalog + fileRequested, FileMode.Open))
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
    }
}
