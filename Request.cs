using System;
using System.IO;
using System.Net.Sockets;
using System.Runtime.Remoting.Channels;
using System.Web;
using log4net;
using log4net.Config;

namespace HTTPserver
{

    /**
     * By: 
     * Salik Nielsen
     * Martin Genchev
     * Katarzyna Grabowska
     * 
     * Github:
     * https://github.com/sali0501/HTTPServer.git
     * Branch: Salik
     */

    /// <summary>
    /// Reads the HTTP request messages
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

            XmlConfigurator.Configure();

            ReadRequest();

            connectionSocket.Close();
        }

        /// <summary>
        /// This constructor is just to test some methods
        /// </summary>
        public Request()
        {}

        /// <summary>
        /// Reads HTTP request message
        /// </summary>
        public void ReadRequest()
        {
            string message = sr.ReadLine();
            string firstLine = message;

            while (!string.IsNullOrEmpty(message))
            {
                Console.WriteLine("Client: " + message);
                Logger.Info("Client: " + message);
                message = sr.ReadLine();
            }

            // Example: GET /someFile.html HTTP/1.1
            // Splits the words
            string[] words = firstLine.Split(' ');

            // Checks if there is a request 
            if (IsRequested(words[1]))
            {
                // Checks if there is a file request.
                // If not, then a folder is requested
                if (IsFileRequested(words[1]))
                {
                    _response.OpenFileRequested(DecodeUrl(words[1]));
                }
                else
                {
                    SendLibrary(words[1]);
                }
            }
            else
            {
                _response.OpenFileRequested("/index.html");

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
        /// <param name="requested"></param>
        /// <returns>bool</returns>
        public bool IsRequested(string requested)
        {
            return !requested.Equals("/");
        }

        /// <summary>
        /// Checks if theres any file request from a HTTP request message
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        public bool IsFileRequested(string request)
        {
            try
            {
                return request.Substring(request.Length - 3, 1).Equals(".") ||
                       request.Substring(request.Length - 4, 1).Equals(".") ||
                       request.Substring(request.Length - 5, 1).Equals(".");
            }
            catch (ArgumentOutOfRangeException e)
            {
                return false;
            }
        }

        /// <summary>
        /// Sends a list of files from a folder
        /// </summary>
        /// <param name="library"></param>
        public void SendLibrary(string library)
        {
            try
            {
                string[] filePaths = Directory.GetFiles(@"c:/temporary" + library);

                foreach (string path in filePaths)
                {
                    _response.HttpBody("<a href=" + "http://localhost:6789" + path.Substring(12) + ">" +
                                       path.Substring(13) + "</a><br>");
                }
                _response.SendResponse();
            }
            catch (DirectoryNotFoundException e)
            {
                _response.HttpHeader("HTTP/1.0 404 Not Found \r\n");
                _response.HttpBody("The library doesn't exist!");
                _response.SendResponse();
            }
        }
    }
}
