using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using NL.Server.Controllers;
using NL.Common;

using NL.Server.View;

namespace NL.Server.Servers {
    internal class ServerManager {

        public Int16 Port { get { return port; } }
        public IRemoteController Server { get { return server; } }
        public List<ServerHandle> Handles { get { return handles; } }
        public Boolean IsAlive {
            get { return (listenerThread != null && listenerThread.IsAlive); }
        }

        private readonly Int16 port;
        private readonly IRemoteController server;
        private TcpListener listener;
        private Thread listenerThread;
        private List<ServerHandle> handles = new List<ServerHandle>();

        public ServerManager(IRemoteController server, Int16 port) {
            this.server = server;
            this.port = port;
        }

        public void Connect() {            
            if (!IsAlive) {
                listenerThread = new Thread(new ThreadStart(Listen));
                listenerThread.Start();
                String message = String.Format(server.ConnectedMessage, port);
                NLConsole.WriteLine(message, ConsoleColor.Cyan);
            }
        }

        public void Disconnect() {
            if (IsAlive) {
                listenerThread.Abort();
            }
            String message = String.Format( server.DisconnectedMessage, port);
            NLConsole.WriteLine(message, ConsoleColor.Cyan);
        }

        private void Listen() {
            listener = new TcpListener(IPAddress.Any, port);
            listener.Start();
            while (true) {
                TcpClient client = listener.AcceptTcpClient();

                
                Thread handler = new Thread(new ParameterizedThreadStart(HandleClient));
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
            handles.Add(handle);
            
            IPAddress ipaddress = endPoint.Address;
            NetworkStream clientStream = client.GetStream();

            NLConsole.WriteLine(String.Format(server.ClientConnectedMessage, ipaddress), server.ClientConnectedColor);

            int bytesRead = 0;
            Byte[] messageBytes = new Byte[64];
            Boolean complete = false;

            while (true) {
                try { bytesRead = clientStream.Read(messageBytes, 0, messageBytes.Length); } 
                catch (Exception e) { NLConsole.WriteLine(String.Format(server.ClientTerminatedMessage, ipaddress), server.ClientTerminatedColor); break; }
                if (bytesRead == 0) { complete = true; break; }
                String message = Encoding.ASCII.GetString(messageBytes, 0, bytesRead);
                CommandPattern command = CommandPattern.Create(message);
                server.InvokeAction(command, client);
            }

            if (complete) {
                NLConsole.WriteLine(String.Format(server.ClientDisconnectedMessage, ipaddress), server.ClientDisconnectedColor);
            }

            client.Close();

            handles.Remove(handle);

        }

        public void DisconnectIP(IPAddress ipaddress) {
            foreach (ServerHandle handle in handles.Where(h => (h.Client.Client.RemoteEndPoint as IPEndPoint).Address.Equals(ipaddress))) {
                handle.Client.Close();
            }
        }


    }
}
