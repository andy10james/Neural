using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NLS {
    class Controller {

        static int Main(string[] args) {
            long start = DateTime.UtcNow.Ticks;
            FileDirector.GetFileHashIndex();
            Printer.PrintFileHashIndex(FileDirector.GetFileHashIndex());
            long end = DateTime.UtcNow.Ticks;
            TimeSpan timeTaken = new TimeSpan(end - start);
            Printer.WriteLine("Time taken to hash: " + timeTaken.TotalSeconds + " seconds");
            Console.ReadLine();
            return 0;
        }

    }
}
