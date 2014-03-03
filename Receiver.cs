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

    /// <summary>
    /// Waits to receive for a HTTP request message
    /// </summary>
    public class Receiver
    {
        
        private static readonly ILog Logger = LogManager.GetLogger(typeof(Receiver));
        private bool _run = true;

        /// <summary>
        /// Listens to localhost:6789
        /// Use threadpool for every client
        /// </summary>
        public Receiver()
        {
            string name = "localhost";
            IPAddress[] addrs = Dns.GetHostEntry(name).AddressList;

            TcpListener serverSocket = new TcpListener(6789);
            serverSocket.Start();

            // http://msdn.microsoft.com/en-us/library/system.console.cancelkeypress(v=vs.110).aspx
            Console.CancelKeyPress += new ConsoleCancelEventHandler(WaitForThreads);

            BasicConfigurator.Configure();

            while (_run)
            {
                Socket connectionSocket = serverSocket.AcceptSocket();
                Logger.Info("Server activated now");

                Task.Run(() => new Echo(connectionSocket));
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
        }
    }
}
