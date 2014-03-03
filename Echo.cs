using System;
using System.IO;
using System.Net.Sockets;
using System.Runtime.Remoting.Channels;
using System.Web;
using log4net;
using log4net.Config;

namespace HTTPserver
{

    /// <summary>
    /// Response to a HTTP request message
    /// </summary>
    public class Echo
    {
        
        private Stream ns;
        private StreamReader sr;
        private StreamWriter sw;
        private const string RootCatalog = "C:/temporary";
        private static readonly ILog Logger = LogManager.GetLogger(typeof(Echo));

        /// <summary>
        /// Reads HTTP request message
        /// </summary>
        /// <param name="connectionSocket"></param>
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
                Logger.Info("Client: " + message);
                message = sr.ReadLine();
            }

            string[] words = firstLine.Split(' ');

            if (IsFileRequested(words[1]))
            {
                OpenFileRequested(words[1]);
            }
            else
            {
                sw.WriteLine("HTTP/1.0 200 OK \r\n");
                sw.WriteLine("No request");
                Logger.Debug("No request");
            }

            connectionSocket.Close();
        }

        /**
         * Decodes URL
         * http://msdn.microsoft.com/en-us/library/system.web.httputility.urldecode.aspx
         */
        private string DecodeUrl(string url)
        {
            return HttpUtility.UrlDecode(url);
        }

        /**
         * Decodes URL
         * http://msdn.microsoft.com/en-us/library/system.web.httputility.urlencode.aspx
         */
        private string EncodeUrl(string url)
        {
            return HttpUtility.UrlEncode(url);
        }

        /**
         * Checks if theres a request from browser
         */
        private bool IsFileRequested(string fileRequested)
        {
            return !fileRequested.Equals("/");
        }

        /**
         * Open requested file
         */
        private void OpenFileRequested(string fileRequested)
        {
            try
            {
                using (FileStream fs = File.Open(RootCatalog + fileRequested, FileMode.Open))
                {
                    sw.WriteLine("HTTP/1.0 202 Accepted \r\n");
                    fs.CopyTo(ns);
                    Logger.Info("A file was requested.");
                }
            }
            catch (FileNotFoundException e)
            {
                sw.WriteLine("HTTP/1.0 404 Not Found \r\n");
                sw.WriteLine("Invalid request!");
                Logger.Error("Invalid request.");
            }
        }
    }
}
