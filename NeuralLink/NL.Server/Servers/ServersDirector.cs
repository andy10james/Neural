using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net;
using NL.Server.Controllers;


namespace NL.Server.Servers {
    internal static class ServersDirector {
        
        private static Dictionary<Int16, IRemoteController> servers; 
        private static Dictionary<Int16, ServerManager> connectedServers;

        static ServersDirector() {
            servers = new Dictionary<Int16, IRemoteController>();
            connectedServers = new Dictionary<Int16, ServerManager>();
            servers.Add(4010, new QueryController());
        }

        public static void ConnectAll() {
            foreach (Int16 port in servers.Keys) {
                ServerManager server = new ServerManager(servers[port], port);
                server.Connect();
                connectedServers.Add(port, server);
            }
        }

        public static void Connect(Int16 port) {
            IRemoteController server;
            servers.TryGetValue(port, out server);
            if (server != null) {
                ServerManager serverInit = new ServerManager(server, port);
                serverInit.Connect();
                connectedServers.Add(port, serverInit);
            }
        }

        public static void DisconnectAll() {
            foreach (Int16 port in servers.Keys) {
                connectedServers[port].Disconnect();
            }
            connectedServers.Clear();
        }

        public static void Disconnect(Int16 port) {
            ServerManager server;
            connectedServers.TryGetValue(port, out server);
            if (server != null) {
                server.Disconnect();
                connectedServers.Remove(port);
            }
        }

        public static void DisconnectIP(IPAddress ipaddress) {
            foreach (ServerManager serverMan in connectedServers.Values) {
                serverMan.DisconnectIP(ipaddress);
            }
        }

    }
}
