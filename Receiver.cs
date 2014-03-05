using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;
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
    /// Waits for a HTTP request message.
    /// </summary>
    public class Receiver
    {

        private static readonly ILog Logger = LogManager.GetLogger(typeof(Receiver));
        private bool _run = true;

        /// <summary>
        /// Waits for any request.
        /// Use threadpool for every client.
        /// </summary>
        /// <param name="ip"></param>
        /// <param name="port"></param>
        public Receiver(string ip, int port)
        {
            IPAddress[] addrs = Dns.GetHostEntry(ip).AddressList;

            TcpListener serverSocket = new TcpListener(port);
            serverSocket.Start();

            // http://msdn.microsoft.com/en-us/library/system.console.cancelkeypress(v=vs.110).aspx
            Console.CancelKeyPress += new ConsoleCancelEventHandler(WaitForThreads);

            XmlConfigurator.Configure();

            while (_run)
            {
                Socket connectionSocket = serverSocket.AcceptSocket();
                Logger.Info("Server activated now");

                Task.Run(() => new Request(connectionSocket));
            }

            serverSocket.Stop();
        }

        /// <summary>
        /// Is called when CTRL+C is pressed.
        /// Waits for all threads to finish.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="arg"></param>
        public void WaitForThreads(object sender, ConsoleCancelEventArgs arg)
        {
            Task.WaitAll();
            _run = false;
            Console.WriteLine("Shutting down");
            Logger.Info("Shutting down");
        }
    }
}