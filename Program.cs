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
    class Program
    {

        static void Main(string[] args)
        {
            Receiver r = new Receiver();
        }
    }
}
