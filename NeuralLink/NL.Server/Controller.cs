using System;
using NL.Server.Configuration;
using NL.Common;
using NL.Server.Servers;

namespace NL.Server {
    internal class Controller {

        private static FileHashIndex _currentIndex;
        private static QueryServer _queryServer;
        private static DeliveryServer _deliveryServer;

        static int Main ( string[] args ) {

            NLConsole.Clear();
            NLConsole.Title("Neural Link");
            NLConsole.StartCommandLine();

            ProcessIndex();
            _queryServer = new QueryServer(9321);
            _queryServer.Connect();
            
            
            Close();
            return 0;

        }

        private static void ProcessIndex() {

            DateTime start = DateTime.UtcNow;
            _currentIndex = FileHashIndex.Create();
            DateTime end = DateTime.UtcNow;
            TimeSpan timeTaken = end - start;

            NLConsole.WriteLine( _currentIndex.ToString(), ConsoleColor.DarkGreen );
            NLConsole.WriteLine("Time taken to hash: " + timeTaken.TotalSeconds + " seconds");

        }

        private static void Close() {
            ServerMemory.Index = _currentIndex;
        }

    }
}
