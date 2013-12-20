using System;
using System.Net;
using System.Reflection;
using System.Windows.Forms.VisualStyles;
using NL.Server.Servers;
using NL.Server.View;
using NL.Server.Configuration;

namespace NL.Server.Controllers {
    class AdminController : IController {

        public AdminController()
            : base() {
            ActionDictionary.Add("DCIP", DisconnectIP);
            ActionDictionary.Add("DCSERVER", DisconnectServer);
            ActionDictionary.Add("RCSERVER", ConnectServer);
            ActionDictionary.Add("LISTSERVERS", ListServers);
            ActionDictionary.Add("DISABLECL", DisableCommand);
            ActionDictionary.Add("CLS", ClearConsole);
            ActionDictionary.Add("EXIT", Exit);
        }

        private void DisconnectIP(String[] parameters) {
            if (parameters.Length != 1) {
                NLConsole.WriteLine(Strings.InvNoOfArgs, ConsoleColor.White);
                return;
            }
            IPAddress ipaddress;
            if (!IPAddress.TryParse(parameters[0], out ipaddress)) {
                NLConsole.WriteLine(Strings.InvIPAddress, ConsoleColor.White);
                return;
            }
            Int32 disconnected = Servers.ServersDirector.DisconnectIP(ipaddress);
            ServersDirector.DisconnectAll();
            String response = String.Format(Strings.IPDisconnected, disconnected);
            NLConsole.WriteLine(response, ConsoleColor.White);
        }

        private void DisableCommand(String[] parameters) {
            NLConsole.StopCommandLine();
        }

        private void ClearConsole(String[] parameters) {
            NLConsole.Clear();
        }

        private void ListServers(String[] parameters) {
            NLConsole.WriteLine(Servers.ServersDirector.ToString(), ConsoleColor.White);
        }

        private void DisconnectServer(String[] parameters) {
            Int16 port = 0;
            if (parameters.Length != 1) {
                NLConsole.WriteLine(Strings.InvNoOfArgs, ConsoleColor.White);
                return;
            }

            if (parameters[0].Equals("*")) {
                ServersDirector.DisconnectAll();
                return;
            }

            if (!Int16.TryParse(parameters[0], out port)) {
                NLConsole.WriteLine(Strings.InvPort, ConsoleColor.White);
            } else if (!ServersDirector.Exists(port)) {
                NLConsole.WriteLine(Strings.NoServerOnPort, ConsoleColor.White);
            } else if (!ServersDirector.IsConnected(port)) {
                NLConsole.WriteLine(Strings.ServerNotConnected, ConsoleColor.White);
            } else {
                ServersDirector.Disconnect(port);
            }
        }

        private void ConnectServer(String[] parameters) {
            Int16 port = 0;
            if (parameters.Length != 1) {
                NLConsole.WriteLine(Strings.InvNoOfArgs, ConsoleColor.White);
                return;
            }

            if (parameters[0].Equals("*")) {
                ServersDirector.ConnectAll();
                return;
            }

            if (!Int16.TryParse(parameters[0], out port)) {
                NLConsole.WriteLine(Strings.InvPort, ConsoleColor.White);
            } else if (!ServersDirector.Exists(port)) {
                NLConsole.WriteLine(Strings.NoServerOnPort, ConsoleColor.White);
            } else if (!ServersDirector.IsConnected(port)) {
                NLConsole.WriteLine(Strings.ServerNotConnected, ConsoleColor.White);
            } else {
                ServersDirector.Connect(port);
            }
        }

        private void AddNewServer(String[] parameters) {
            Assembly.
        }

        private void Exit(String[] parameters) {
            Environment.Exit(0);
        }

    }
}
