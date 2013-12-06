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
                Printer.Write("Query Server started...", ConsoleColor.Cyan);
            }
        }

        public void Disconnect() {
            if (IsAlive) {
                _listenerThread.Abort();
            }
        }

        private void Listen() {
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
            Byte[] messageBytes = new Byte[4096];

            while (client.Connected) {
                long bytesRead = 0;
                try {
                    bytesRead = clientStream.Read(messageBytes, 0, 4096);
                }
                catch {
                    //todo Implement exception logging here 
                }
                String message = BitConverter.ToString(messageBytes);
                Printer.Write("Connection from " + ipaddress + " requested " + message);
                
            }

            client.Close();
            _responseThreads.Remove(Thread.CurrentThread);

        }

    }
}
