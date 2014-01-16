using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NL.Common;
using NL.Server.View;
using NL.Server.Configuration;

namespace NL.Server.Services {
    internal static class FileService {

        private static FileHashIndex currentIndex;

        public static FileHashIndex HashRepository() {
        
            DateTime start = DateTime.UtcNow;
            currentIndex = FileHashIndex.Create(ServerConfiguration.Repository);
            DateTime end = DateTime.UtcNow;
            TimeSpan timeTaken = end - start;

            NLConsole.WriteLine( currentIndex.ToString(), ConsoleColor.DarkGreen );
            NLConsole.WriteLine("Time taken to hash: " + timeTaken.TotalSeconds + " seconds");

            return currentIndex;

        }

        public static void Close() {
            ServerMemory.Index = currentIndex;
        }

    }
}
