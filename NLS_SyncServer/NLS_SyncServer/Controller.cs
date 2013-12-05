using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace NLS {
    class Controller {

        private static FileHashIndex CurIndex;

        static int Main ( string[] args ) {

            Printer.Clear();
            ProcessIndex();
            Printer.Read();

            Close();
            return 0;

        }

        private static void ProcessIndex() {

            long start = DateTime.UtcNow.Ticks;
            CurIndex = FileHashIndex.Create();
            long end = DateTime.UtcNow.Ticks;
            TimeSpan timeTaken = new TimeSpan(end - start);

            Printer.Write( CurIndex.ToString(), ConsoleColor.DarkGreen );
            Printer.Write("Time taken to hash: " + timeTaken.TotalSeconds + " seconds");

        }

        private static void Close() {
            Configuration.ApplicationMemory.Index = CurIndex;
        }

    }
}
