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
    /// The class is used to read the HTTP request message
    /// </summary>
    public class Request
    {
        
        private Stream ns;
        private StreamReader sr;
        private static readonly ILog Logger = LogManager.GetLogger(typeof(Request));
        private readonly Response _response;

        /// <summary>
        /// Initialize variables
        /// </summary>
        /// <param name="connectionSocket"></param>
        public Request(Socket connectionSocket)
        {
            ns = new NetworkStream(connectionSocket);
            sr = new StreamReader(ns);

            _response = new Response(ns);

            ReadRequest();

            connectionSocket.Close();
        }

        /// <summary>
        /// Reads HTTP request message
        /// </summary>
        public void ReadRequest()
        {
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
                _response.OpenFileRequested(DecodeUrl(words[1]));
            }
            else
            {
                _response.HttpHeader("HTTP/1.0 200 OK \r\n");
                _response.HttpBody("No request");
                _response.SendResponse();

                Logger.Debug("No request");
            }
        }
        
        /// <summary>
        /// Decodes URL
        /// http://msdn.microsoft.com/en-us/library/system.web.httputility.urldecode.aspx
        /// </summary>
        /// <param name="url"></param>
        /// <returns>string</returns>
        public string DecodeUrl(string url)
        {
            return HttpUtility.UrlDecode(url);
        }

        /// <summary>
        /// Encodes URL
        /// http://msdn.microsoft.com/en-us/library/system.web.httputility.urlencode.aspx
        /// </summary>
        /// <param name="url"></param>
        /// <returns>string</returns>
        public string EncodeUrl(string url)
        {
            return HttpUtility.UrlEncode(url);
        }

        /// <summary>
        /// Checks if theres any file request
        /// </summary>
        /// <param name="fileRequested"></param>
        /// <returns>bool</returns>
        public bool IsFileRequested(string fileRequested)
        {
            return !fileRequested.Equals("/");
        }
    }
}
