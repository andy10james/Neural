using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net;
using NL.Common;
using NL.Server.View;
using NL.Server.Configuration;

namespace NL.Server.Controllers {
    class AdminController : IController {

        public AdminController() : base() {
            ActionDictionary.Add("DISCONNECTIP", DisconnectIP);
            ActionDictionary.Add("DISCONNECTSERVER", DisconnectServer);
            ActionDictionary.Add("LISTSERVERS", ListServers);
            ActionDictionary.Add("DISABLECOMMANDLINE", DisableCommand);
            ActionDictionary.Add("CLEARCONSOLE", ClearConsole);
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
            } else if (!Int16.TryParse(parameters[0], out port)) {
                NLConsole.WriteLine(Strings.InvPort, ConsoleColor.White);
            } else if (!Servers.ServersDirector.Exists(port)) {
                NLConsole.WriteLine(Strings.NoServerOnPort, ConsoleColor.White);
            } else if (!Servers.ServersDirector.IsConnected(port)) {
                NLConsole.WriteLine(Strings.ServerNotConnected, ConsoleColor.White);
            } else {
                Servers.ServersDirector.Disconnect(port);
            }   
        }

    }
}
