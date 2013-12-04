using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Runtime.Serialization.Json;

namespace NLS {
    class Controller {

        private static FileHashIndex Index;

        static int Main ( string[] args ) {

            
            



            Properties.Settings.Default.PreviousFileHashIndex = jsonString;
            Properties.Settings.Default.Save();

            Console.ReadLine();
            return 0;

        }

        private static void ProcessIndex() {

            long start = DateTime.UtcNow.Ticks;
            Index = FileHashIndex.Create();
            long end = DateTime.UtcNow.Ticks;
            TimeSpan timeTaken = new TimeSpan(end - start);

            Printer.Write( Index.ToString() );
            Printer.Write("Time taken to hash: " + timeTaken.TotalSeconds + " seconds");

        }

        private static void SerializeFileHashIndex


    }
}
