using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace NL.Server.Servers {
    internal class QueryServer {


        public Int16 Port { get { return _port; } }
        public Boolean IsAlive {
            get { return (_listenerThread != null && _listenerThread.IsAlive); }
        }

        private readonly Int16 _port;
        private TcpListener _listener;
        private Thread _listenerThread;
        private List<Thread> _responseThreads;

        public QueryServer(Int16 port) {
            this._port = port;
        }

        public void Connect() {            
            if (!IsAlive) {
                _listenerThread = new Thread(new ThreadStart(Listen));
                _listenerThread.Start();
                NLConsole.WriteLine("Query Server online and listening on port " + _port, ConsoleColor.Cyan);
            }
        }

        public void Disconnect() {
            if (IsAlive) {
                _listenerThread.Abort();
            }
            NLConsole.WriteLine("Query Server disconnected from port " + _port, ConsoleColor.Cyan);
        }

        private void Listen() {
            _responseThreads = new List<Thread>();
            _listener = new TcpListener(IPAddress.Any, _port);
            _listener.Start();
            
            while (true) {
                TcpClient client = _listener.AcceptTcpClient();
                new Thread(new ParameterizedThreadStart(HandleClient))
                    .Start(client);
            }
        }

        private void HandleClient(Object clientObject) {
            _responseThreads.Add(Thread.CurrentThread);
            TcpClient client = (TcpClient) clientObject;
            IPEndPoint endPoint = client.Client.RemoteEndPoint as IPEndPoint;
            IPAddress ipaddress = endPoint.Address;
            NetworkStream clientStream = client.GetStream();

            int bytesRead = 0;
            Byte[] messageBytes = new Byte[64];

            while (true) {
                try { bytesRead = clientStream.Read(messageBytes, 0, messageBytes.Length); } 
                catch { NLConsole.WriteLine("[" + ipaddress + "] Connection with client was terminated.", ConsoleColor.DarkRed); break; }
                if (bytesRead == 0) break;
                String message = Encoding.ASCII.GetString(messageBytes, 0, bytesRead);
                NLConsole.WriteLine("[" + ipaddress + "] Transmitted: " + message, ConsoleColor.DarkYellow);
            }

            client.Close();
            _responseThreads.Remove(Thread.CurrentThread);

        }

    }
}
