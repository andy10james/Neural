using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

using NL.Server.View;

namespace NL.Server.Servers {
    internal class ServerInit {

        public Int16 Port { get { return _port; } }
        public Boolean IsAlive {
            get { return (_listenerThread != null && _listenerThread.IsAlive); }
        }

        private readonly Int16 _port;
        private readonly IServer _server;
        private TcpListener _listener;
        private Thread _listenerThread;

        public ServerInit(IServer server, Int16 port) {
            this._server = server;
            this._port = port;
        }

        public void Connect() {            
            if (!IsAlive) {
                _listenerThread = new Thread(new ThreadStart(Listen));
                _listenerThread.Start();
                String message = String.Format(_server.ConnectedMessage, _port);
                NLConsole.WriteLine(message, ConsoleColor.Cyan);
            }
        }

        public void Disconnect() {
            if (IsAlive) {
                _listenerThread.Abort();
            }
            String message = String.Format( _server.DisconnectedMessage, _port);
            NLConsole.WriteLine(message, ConsoleColor.Cyan);
        }

        private void Listen() {
            _listener = new TcpListener(IPAddress.Any, _port);
            _listener.Start();
            while (true) {
                TcpClient client = _listener.AcceptTcpClient();
                new Thread(new ParameterizedThreadStart(_server.HandleClient))
                    .Start(client);
            }
        }

    }
}
