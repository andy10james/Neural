using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.Serialization.Json;
using System.Text;
using System.Threading.Tasks;

namespace NLS {
    class Controller {

        private static FileHashIndex Index;

        static int Main ( string[] args ) {

            ProcessIndex();
Close();
            Printer.Read();

            
            return 0;

        }

        private static void ProcessIndex() {

            long start = DateTime.UtcNow.Ticks;
            Index = FileHashIndex.Create();
            long end = DateTime.UtcNow.Ticks;
            TimeSpan timeTaken = new TimeSpan(end - start);

            Printer.Write( Index.ToString(), ConsoleColor.DarkGreen );
            Printer.Write("Time taken to hash: " + timeTaken.TotalSeconds + " seconds");

        }

        private static void Close() {

            String jsonString;
            DataContractJsonSerializer jsonSerializer = new DataContractJsonSerializer(Index.GetType(),
                new DataContractJsonSerializerSettings { UseSimpleDictionaryFormat = true });
            using (MemoryStream stream = new MemoryStream()) {
                jsonSerializer.WriteObject(stream, Index);
                jsonString = Encoding.UTF8.GetString(stream.ToArray());
            }

            Printer.Write(jsonString);

            Properties.Settings.Default.PreviousFileHashIndex;
            Properties.Settings.Default.Save();
        }

    }
}
