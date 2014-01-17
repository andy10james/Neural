using System;
using System.IO;
using NL.Common;
using NL.Server.View;
using NL.Server.Configuration;

namespace NL.Server.Services {
    internal static class FileService {

        private static FileHashIndex currentIndex;

        public static void HashRepository() {
        
            DateTime start = DateTime.UtcNow;

            try {
                currentIndex = FileHashIndex.Create(ServerConfiguration.Repository);
            } catch (IOException e) {
                NLConsole.WriteLine(e.Message.Trim(), ConsoleColor.Red);
                return;
            }

            DateTime end = DateTime.UtcNow;
            TimeSpan timeTaken = end - start;

            NLConsole.WriteLine( currentIndex.ToString(), ConsoleColor.DarkGreen );
            NLConsole.WriteLine(String.Format(UIStrings.TimeTakenToHash, timeTaken.TotalSeconds));

        }

        public static void Close() {
            if (currentIndex != null) ServerMemory.Index = currentIndex;
        }

    }
}
