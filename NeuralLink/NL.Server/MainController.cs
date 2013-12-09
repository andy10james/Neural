using System;
using NL.Server.Configuration;
using NL.Common;
using NL.Server.Servers;
using NL.Server.View;

namespace NL.Server {
    internal class MainController {

        private static FileHashIndex _currentIndex;
        private static ServerInit _queryServer;
        private static ServerInit _deliveryServer;

        static int Main ( string[] args ) {

            NLConsole.Clear();
            NLConsole.Title("Neural Link");
            NLConsole.StartCommandLine();

            DateTime start = DateTime.UtcNow;
            _currentIndex = FileHashIndex.Create();
            DateTime end = DateTime.UtcNow;
            TimeSpan timeTaken = end - start;

            NLConsole.WriteLine( _currentIndex.ToString(), ConsoleColor.DarkGreen );
            NLConsole.WriteLine("Time taken to hash: " + timeTaken.TotalSeconds + " seconds");

            _queryServer = new ServerInit(new QueryServer(), 9321);
            _queryServer.Connect();
            
            Close();
            return 0;

        }

        private static void Close() {
            ServerMemory.Index = _currentIndex;
        }

    }
}
