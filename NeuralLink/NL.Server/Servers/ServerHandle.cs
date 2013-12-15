using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net.Sockets;
using System.Net;
using System.Threading;

namespace NL.Server.Servers {
    internal class ServerHandle {
        public Thread Thread;
        public TcpClient Client;
    }
}
