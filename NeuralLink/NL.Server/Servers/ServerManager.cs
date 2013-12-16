using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using NL.Server.Controllers;
using NL.Server.Configuration;
using NL.Common;
using NL.Server.View;

namespace NL.Server.Servers {
    internal class ServerManager {

        public Int16 Port { get { return _port; } }
        public IRemoteController Server { get { return _server; } }
        public List<ServerHandle> Handles { get { return _handles; } }
        public Boolean IsAlive {
            get { return (_listenerThread != null && _listenerThread.IsAlive); }
        }

        private readonly Int16 _port;
        private readonly IRemoteController _server;
        private readonly List<ServerHandle> _handles = new List<ServerHandle>();
        private TcpListener _listener;
        private Thread _listenerThread;

        public ServerManager(IRemoteController server, Int16 port) {
            this._server = server;
            this._port = port;
        }

        public void Connect() {            
            if (!IsAlive) {
                _listenerThread = new Thread(new ThreadStart(Listen));
                _listenerThread.Name = Thread.CurrentThread.Name + "." + _server.GetType().Name;
                _listenerThread.Start();
                String message = String.Format(_server.ConnectedMessage, _port);
                NLConsole.WriteLine(message, ConsoleColor.Cyan);
            }
        }

        public void Disconnect() {
            if (IsAlive) {
                foreach (ServerHandle handle in _handles) {
                    handle.Client.Close();
                }
                _listener.Stop();
            }
            String message = String.Format( _server.DisconnectedMessage, _port);
            NLConsole.WriteLine(message, ConsoleColor.Cyan);
        }

        private void Listen() {
            _listener = new TcpListener(IPAddress.Any, _port);
            _listener.Start();
            while (true) {
                TcpClient client;
                try { client = _listener.AcceptTcpClient(); }
                catch (Exception e) {
                    if (!(e is SocketException) || (e as SocketException).ErrorCode != 10004) {
                        String message = String.Format(Strings.ServerAbnormallyTerminated, _port, e.Message + "(" + e.Source + ")");
                        NLConsole.WriteLine(message, ConsoleColor.Red);
                    }
                    break;
                }
                Thread handler = new Thread(HandleClient);
                handler.Name = Thread.CurrentThread.Name + "." + (client.Client.RemoteEndPoint as IPEndPoint);
                handler.Start(client);
            }
        }

        public void HandleClient(Object clientObject) {

            TcpClient client = (TcpClient)clientObject;
            IPEndPoint endPoint = client.Client.RemoteEndPoint as IPEndPoint;

            if (endPoint == null) {
                client.Close();
                return;
            }

            ServerHandle handle = new ServerHandle {
                Thread = Thread.CurrentThread,
                Client = client
            };
            _handles.Add(handle);

            if (ServerConfiguration.BeepOnConnection) {
                Console.Beep(1000, 80);
                Console.Beep(1500, 80);
                Console.Beep(2000, 200);
            }
           
            IPAddress ipaddress = endPoint.Address;
            NetworkStream clientStream = client.GetStream();

            NLConsole.WriteLine(String.Format(_server.ClientConnectedMessage, ipaddress), _server.ClientConnectedColor);

            int bytesRead = 0;
            Byte[] messageBytes = new Byte[64];
            Boolean complete = false;

            while (true) {
                try { bytesRead = clientStream.Read(messageBytes, 0, messageBytes.Length); } 
                catch { NLConsole.WriteLine(String.Format(_server.ClientTerminatedMessage, ipaddress), _server.ClientTerminatedColor); break; }
                if (bytesRead == 0) { complete = true; break; }
                String message = Encoding.ASCII.GetString(messageBytes, 0, bytesRead);
                CommandPattern command = CommandPattern.Create(message);
                _server.InvokeAction(command, client);
            }

            if (complete) {
                NLConsole.WriteLine(String.Format(_server.ClientDisconnectedMessage, ipaddress), _server.ClientDisconnectedColor);
            }

            client.Close();

            _handles.Remove(handle);

            if (ServerConfiguration.BeepOnDisconnection) {
                Console.Beep(2000, 80);
                Console.Beep(1500, 80);
                Console.Beep(1000, 200);
            }

        }

        public Int32 DisconnectIP(IPAddress ipaddress) {
            Int32 disconnected = 0;
            foreach (ServerHandle handle in _handles.Where(h => (h.Client.Client.RemoteEndPoint as IPEndPoint).Address.Equals(ipaddress))) {
                handle.Client.Close();
                disconnected++;
            }
            return disconnected;
        }


    }
}
