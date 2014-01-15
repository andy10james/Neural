using System;
using System.Net;
using System.Reflection;
using System.Runtime.Remoting.Messaging;
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
            ActionDictionary.Add("ADDSERVER", AddNewServer);
            ActionDictionary.Add("LISTSERVERS", ListServers);
            ActionDictionary.Add("DCL", DisableCommand);
            ActionDictionary.Add("CLS", ClearConsole);
            ActionDictionary.Add("EXIT", Exit);
            ActionDictionary.Add("HELP", CommandHelp);
        }
      
        private void CommandHelp(String[] parameters)
        {
            String[] help = Strings.CommandHelp.Split(';');
            for (int i = 0; i <= 17; i++)
                if (i % 2 == 0)
                    NLConsole.WriteLine(help[i], ConsoleColor.White);
                else
                    NLConsole.WriteLine(help[i]);
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
            Int32 disconnected = ServersDirector.DisconnectIP(ipaddress);
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
            } else if (ServersDirector.IsConnected(port)) {
                NLConsole.WriteLine(Strings.ServerAlreadyConnected, ConsoleColor.White);
            } else {
                ServersDirector.Connect(port);
            }
        }

        private void AddNewServer(String[] parameters) {
            Int16 port;
            if (parameters.Length != 2) {
                NLConsole.WriteLine(Strings.InvNoOfArgs);
                return;
            } else if (!Int16.TryParse(parameters[0], out port)) {
                NLConsole.WriteLine(Strings.InvPort);
                return;
            } else if (ServersDirector.Exists(port)) {
                String message = String.Format(Strings.ServerExistsOnPort, port);
                NLConsole.WriteLine(message, ConsoleColor.White);
                return;
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
                NLConsole.WriteLine(Strings.ControllerDoesNotExist, ConsoleColor.White);
                return;
            }

            if (controller == null) {
                NLConsole.WriteLine(Strings.ControllerDoesNotExist, ConsoleColor.White);
                return;
            }

            ServersDirector.AddServer(port, controller);

        }

        private void Exit(String[] parameters) {
            Environment.Exit(0);
        }

    }

}

