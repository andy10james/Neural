using System;
using System.Threading;
using NL.Server.Configuration;
using NL.Server.Servers;
using NL.Server.View;
using NL.Server.Controllers;
using NL.Server.Services;

namespace NL.Server {
    internal class MainController {

        static int Main ( string[] args ) {

            Thread.CurrentThread.Name = "NL.Server";

            NLConsole.Clear();
            NLConsole.Title(UIStrings.NLConsoleTitle);
            NLConsole.StartCommandLine();

            FileService.HashRepository();

            NLConsole.AddController(new AdminController());
            NLConsole.AddController(new QueryController());

            foreach (String command in ServerConfiguration.StartUpCommands) {
                NLConsole.InvokeConsoleCommand(command);
            }

            Close();
            return 0;

        }

        private static void Close() {
            FileService.Close();
        }

    }
}
