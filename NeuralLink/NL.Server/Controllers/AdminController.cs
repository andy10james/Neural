using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NL.Common;
using NL.Server.View;
using System.Net;

namespace NL.Server.Controllers {
    class AdminController : IController {

        public AdminController() : base() {
            ActionDictionary.Add("DISCONNECTIP", DisconnectIP);
            ActionDictionary.Add("DISABLECOMMANDLINE", DisableCommand);
            ActionDictionary.Add("CLEARCONSOLE", ClearConsole);
        }

        private void DisconnectIP(String[] parameters) {
            if (parameters.Length != 1) {
                NLConsole.WriteLine("Invalid number of arguments.", ConsoleColor.White);
                return;
            }
            IPAddress ipaddress;
            if (!IPAddress.TryParse(parameters[0], out ipaddress)) {
                NLConsole.WriteLine("Invalid IP addresss given.", ConsoleColor.White);
                return;
            }
            Servers.ServersDirector.DisconnectIP(ipaddress);
        }

        private void DisableCommand(String[] parameters) {
            NLConsole.StopCommandLine();
        }

        private void ClearConsole(String[] parameters) {
            NLConsole.Clear();
        }

    }
}
