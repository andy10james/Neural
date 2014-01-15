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
                    handle.Die();
                }
                _listener.Stop();
            }
            String message = String.Format( _server.DisconnectedMessage, _port);
            NLConsole.WriteLine(message, ConsoleColor.Cyan);
        }


        public Int32 DisconnectIP(IPAddress ipaddress) {
            Int32 disconnected = 0;
            foreach (ServerHandle handle in _handles.Where(h => h.EndPoint.Address.Equals(ipaddress))) {
                handle.Die();
                disconnected++;
            }
            return disconnected;
        }
        
        private void Listen() {
            _listener = new TcpListener(IPAddress.Any, _port);
            _listener.Start();
            while (true) {
                TcpClient client;
                try { client = _listener.AcceptTcpClient(); } catch (Exception e) {
                    if (!(e is SocketException) || (e as SocketException).ErrorCode != 10004) {
                        String message = String.Format(Strings.ServerAbnormallyTerminated, _port, e.Message + "(" + e.Source + ")");
                        NLConsole.WriteLine(message, ConsoleColor.Red);
                    }
                    break;
                }
                ServerHandle handle = new ServerHandle(_server, client);
                handle.OnDeath += (ServerHandle e) => this._handles.Remove(e);
                handle.Start();
                _handles.Add(handle);
            }
        }

    }
}
