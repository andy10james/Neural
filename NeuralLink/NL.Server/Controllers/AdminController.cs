using System;
using System.Net;
using System.Reflection;
using NL.Server.Servers;
using NL.Server.View;
using NL.Server.Configuration;

namespace NL.Server.Controllers {
    class AdminController : IController {

        public AdminController()
            : base() {
            ActionDictionary.Add("DCIP", DisconnectIP);
            ActionDictionary.Add("DCSERVER", DisconnectServer);
            ActionDictionary.Add("CSERVER", ConnectServer);
            ActionDictionary.Add("ACSERVER", AddConnectNewServer);
            ActionDictionary.Add("ADDSERVER", AddNewServer);
            ActionDictionary.Add("DELSERVER", RemoveServer);
            ActionDictionary.Add("LISTSERVERS", ListServers);
            ActionDictionary.Add("DCL", DisableCommand);
            ActionDictionary.Add("CLS", ClearConsole);
            ActionDictionary.Add("EXIT", Exit);
            ActionDictionary.Add("HELP", Help);
            NLConsole.WriteLine(UIStrings.HelpPrompt, ConsoleColor.Red);
        }

        public override String GetName() {
            return "Admin Controller";
        }

        private Boolean DisconnectIP(String[] parameters) {
            if (parameters.Length != 1) {
                NLConsole.WriteLine(UIStrings.InvNoOfArgs, ConsoleColor.White);
                return false;
            }
            IPAddress ipaddress;
            if (!IPAddress.TryParse(parameters[0], out ipaddress)) {
                NLConsole.WriteLine(UIStrings.InvIPAddress, ConsoleColor.White);
                return false;
            }
            Int32 disconnected = ServersDirector.DisconnectIP(ipaddress);
            String response = String.Format(UIStrings.IPDisconnected, disconnected);
            NLConsole.WriteLine(response, ConsoleColor.White);
            return true;
        }

        private Boolean DisableCommand(String[] parameters) {
            NLConsole.StopCommandLine();
            return true;
        }

        private Boolean ClearConsole(String[] parameters) {
            NLConsole.Clear();
            return true;
        }

        private Boolean ListServers(String[] parameters) {
            NLConsole.WriteLine(Servers.ServersDirector.ToString(), ConsoleColor.White);
            return true;
        }

        private Boolean DisconnectServer(String[] parameters) {
            Int16 port = 0;
            if (parameters.Length != 1) {
                NLConsole.WriteLine(UIStrings.InvNoOfArgs, ConsoleColor.White);
                return false;
            }

            if (parameters[0].Equals("*")) {
                ServersDirector.DisconnectAll();
                return false;
            }

            if (!Int16.TryParse(parameters[0], out port)) {
                NLConsole.WriteLine(UIStrings.InvPort, ConsoleColor.White);
                return false;
            } else if (!ServersDirector.Exists(port)) {
                NLConsole.WriteLine(UIStrings.ServerDoesNotExistOnPort, ConsoleColor.White);
                return false;
            } else if (!ServersDirector.IsConnected(port)) {
                NLConsole.WriteLine(UIStrings.ServerNotConnected, ConsoleColor.White);
                return false;
            } else {
                ServersDirector.Disconnect(port);
                return true;
            }
        }

        private Boolean ConnectServer(String[] parameters) {
            Int16 port = 0;
            if (parameters.Length != 1) {
                NLConsole.WriteLine(UIStrings.InvNoOfArgs, ConsoleColor.White);
                return false;
            }

            if (parameters[0].Equals("*")) {
                ServersDirector.ConnectAll();
                return false;
            }

            if (!Int16.TryParse(parameters[0], out port)) {
                NLConsole.WriteLine(UIStrings.InvPort, ConsoleColor.White);
                return false;
            } else if (!ServersDirector.Exists(port)) {
                NLConsole.WriteLine(UIStrings.ServerDoesNotExistOnPort, ConsoleColor.White);
                return false;
            } else if (ServersDirector.IsConnected(port)) {
                NLConsole.WriteLine(UIStrings.ServerAlreadyConnected, ConsoleColor.White);
                return false;
            } else {
                ServersDirector.Connect(port);
                return true;
            }
        }

        private Boolean AddConnectNewServer(String[] parameters) {
            if (!AddNewServer(parameters)) return false;
            if (!ConnectServer(new String[] { parameters[0] })) return false;
            return true;
        }

        private Boolean AddNewServer(String[] parameters) {
            Int16 port;
            if (parameters.Length != 2) {
                NLConsole.WriteLine(UIStrings.InvNoOfArgs);
                return false;
            } else if (!Int16.TryParse(parameters[0], out port)) {
                NLConsole.WriteLine(UIStrings.InvPort);
                return false;
            } else if (ServersDirector.Exists(port)) {
                String message = String.Format(UIStrings.ServerExistsOnPort, port);
                NLConsole.WriteLine(message, ConsoleColor.White);
                return false;
            } else if (!ServersDirector.IsPortAvailable(port)) {
                String message = String.Format(UIStrings.PortUnavailable, port);
                NLConsole.WriteLine(message, ConsoleColor.White);
                return false;
            }

            String controllerName = parameters[1];
            if (!controllerName.StartsWith("NL.")) {
                controllerName = "NL.Server.Controllers." + controllerName;
            }

            Assembly assembly = Assembly.GetExecutingAssembly();
            IRemoteController controller;
            try {
                controller = (IRemoteController)assembly.CreateInstance(controllerName, true);
            } catch (InvalidCastException e) {
                NLConsole.WriteLine(UIStrings.ControllerDoesNotExist, ConsoleColor.White);
                return false;
            }

            if (controller == null) {
                NLConsole.WriteLine(UIStrings.ControllerDoesNotExist, ConsoleColor.White);
                return false;
            }

            ServersDirector.AddServer(port, controller);
            return true;

        }

        private Boolean RemoveServer(String[] parameters) {
            Int16 port;
            if (parameters.Length != 1) {
                NLConsole.WriteLine(UIStrings.InvNoOfArgs);
                return false;
            } else if (!Int16.TryParse(parameters[0], out port)) {
                NLConsole.WriteLine(UIStrings.InvPort);
                return false;
            } else if (!ServersDirector.Exists(port)) {
                String message = String.Format(UIStrings.ServerDoesNotExistOnPort, port);
                NLConsole.WriteLine(message, ConsoleColor.White);
                return false;
            }
            ServersDirector.RemoveServer(port);
            return true;
        }

        private Boolean Exit(String[] parameters) {
            Environment.Exit(0);
            return true;
        }

        private Boolean Help(String[] parameters) {
            String[] help = UIStrings.CommandHelp.Split(';');
            for (int i = 0; i < help.Length; i++)
                if (i % 2 == 0)
                    NLConsole.WriteLine(help[i], ConsoleColor.DarkRed);
                else
                    NLConsole.WriteLine(help[i]);
            return true;
        }

    }

}

