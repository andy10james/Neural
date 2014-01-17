using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net;
using NL.Server.Controllers;
using NL.Server.Configuration;
using NL.Server.View;


namespace NL.Server.Servers {
    internal static class ServersDirector {
        
        private static Dictionary<Int16, ServerManager> Servers; 

        static ServersDirector() {
            Servers = new Dictionary<Int16, ServerManager>();
        }

        public static ServerManager AddServer(Int16 port, IRemoteController controller) {
            return AddServer(port, new ServerManager(controller, port));
        }

        public static ServerManager AddServer(Int16 port, ServerManager server) {
            Servers.Add(port, server);
            String message = String.Format(UIStrings.ServerAdded, server.Server.GetType().Name, port);
            NLConsole.WriteLine(message, ConsoleColor.White);
            return server;
        }

        public static void ConnectAll() {
            foreach (Int16 port in Servers.Keys) {
                ServerManager server = Servers[port];
                if (!server.IsAlive) {
                    server.Connect();
                }
            }
        }

        public static void Connect(Int16 port) {
            ServerManager server;
            Servers.TryGetValue(port, out server);
            if (server != null) {
                server.Connect();
            }
        }

        public static void DisconnectAll() {
            foreach (Int16 port in Servers.Keys) {
                ServerManager server = Servers[port];
                if (server.IsAlive) {
                    server.Disconnect();
                }
            }
        }

        public static void Disconnect(Int16 port) {
            ServerManager server;
            Servers.TryGetValue(port, out server);
            if (server != null) {
                server.Disconnect();
            }
        }

        public static Int32 DisconnectIP(IPAddress ipaddress) {
            Int32 disconnected = 0;
            foreach (ServerManager server in Servers.Values.Where(s => s.IsAlive)) {
                disconnected += server.DisconnectIP(ipaddress);
            }
            return disconnected;
        }

        public static Boolean Exists(Int16 port) {
            return Servers.ContainsKey(port);
        }

        public static Boolean IsConnected(Int16 port) {
            return Servers.Where(s => s.Key == port && s.Value.IsAlive).Any();
        }

        public new static String ToString() {
            StringBuilder output = new StringBuilder();
            output.AppendFormat("{0,-10}{1,-20}\n", UIStrings.Port, UIStrings.Server);
            foreach (Int16 port in Servers.Keys) {
                output.AppendFormat("{0,-10}{1,-20}{2,-30}", port, Servers[port].Server.GetType().Name, 
                IsConnected(port) ? UIStrings.Connected : UIStrings.Disconnected );
                if (port != Servers.Keys.Last()) output.AppendLine();
            }
            return output.ToString();
        }

    }
}
