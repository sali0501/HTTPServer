using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using log4net;

namespace HTTPserver
{

    /// <summary>
    /// The class is used to send a response
    /// </summary>
    public class Response
    {
        private Stream ns;
        private StreamWriter sw;
        private readonly List<string> _response;
        private static readonly ILog Logger = LogManager.GetLogger(typeof(Response));
        private const string RootCatalog = "C:/temporary";

        /// <summary>
        /// Initialize variables and flushes StreamWriter
        /// </summary>
        /// <param name="ns"></param>
        public Response(Stream ns)
        {
            _response = new List<string>();
            this.ns = ns;
            sw = new StreamWriter(ns) {AutoFlush = true};
        }

        /// <summary>
        /// Writes to the HTTP message header
        /// </summary>
        /// <param name="header"></param>
        public void HttpHeader(string header)
        {
            _response.Insert(0, header);
        }

        /**
         * Open and send the requested file
         */
        public void OpenFileRequested(string fileRequested)
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
                HttpHeader("HTTP/1.0 404 Not Found \r\n");
                HttpBody("Invalid request!");
                Logger.Error("Invalid request.");
            }
        }

        /// <summary>
        /// Writes to the HTTP body
        /// </summary>
        /// <param name="message"></param>
        public void HttpBody(string message)
        {
            _response.Add(message);
        }

        /// <summary>
        /// Sends HTTP response message
        /// </summary>
        public void SendResponse()
        {
            foreach (string t in _response)
            {
                sw.WriteLine(t);
            }
        }
    }
}
