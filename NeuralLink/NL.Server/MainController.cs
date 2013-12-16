using System;
using System.Threading;
using NL.Server.Configuration;
using NL.Common;
using NL.Server.Servers;
using NL.Server.View;
using NL.Server.Controllers;
using NL.Server.Services;

namespace NL.Server {
    internal class MainController {

        static int Main ( string[] args ) {

            Thread.CurrentThread.Name = "NL.Server";

            NLConsole.Clear();
            NLConsole.Title("Neural Link");
            NLConsole.StartCommandLine();

            FileService.HashRepository();

            ServersDirector.ConnectAll();
            new AdminController();

            Close();
            return 0;

        }

        private static void Close() {
            FileService.Close();
        }

    }
}
