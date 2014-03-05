using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
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
    /// The class is to run the whole project
    /// </summary>
    class Program
    {

        static void Main(string[] args)
        {
            Receiver r = new Receiver("localhost", 6789);
        }
    }
}