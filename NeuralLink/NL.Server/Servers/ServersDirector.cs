using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net;
using NL.Server.Controllers;
using NL.Server.Configuration;


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

        public static Int32 DisconnectIP(IPAddress ipaddress) {
            Int32 disconnected = 0;
            foreach (ServerManager serverMan in connectedServers.Values) {
                disconnected += serverMan.DisconnectIP(ipaddress);
            }
            return disconnected;
        }

        public static Boolean Exists(Int16 port) {
            return servers.ContainsKey(port);
        }

        public static Boolean IsConnected(Int16 port) {
            return connectedServers.ContainsKey(port);
        }

        public new static String ToString() {
            StringBuilder output = new StringBuilder();
            output.AppendFormat("{0,-10}{1,-20}\n", Strings.Port, Strings.Server);
            foreach (Int16 port in servers.Keys) {
                
                output.AppendFormat("{0,-10}{1,-20}{2,-30}", port, servers[port].GetType().Name, 
                IsConnected(port) ? Strings.Connected : Strings.Disconnected );
                if (port != servers.Keys.Last()) output.AppendLine();
            }
            return output.ToString();
        }

    }
}
