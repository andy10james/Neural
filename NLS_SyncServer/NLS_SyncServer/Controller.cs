using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Runtime.Serialization.Json;

namespace NLS {
    class Controller {

        static int Main ( string[] args ) {

            long start = DateTime.UtcNow.Ticks;
            Dictionary<String, String> fileHashIndex = FileDirector.GetFileHashIndex();
            long end = DateTime.UtcNow.Ticks;
            Printer.PrintFileHashIndex( fileHashIndex );
            TimeSpan timeTaken = new TimeSpan( end - start );
            Printer.WriteLine( "Time taken to hash: " + timeTaken.TotalSeconds + " seconds" );

            DataContractJsonSerializer jsonSerializer = new DataContractJsonSerializer( fileHashIndex.GetType(), new DataContractJsonSerializerSettings { UseSimpleDictionaryFormat = true } );
            String jsonString = "";
            using ( MemoryStream stream = new MemoryStream() ) {
                jsonSerializer.WriteObject( stream, fileHashIndex );
                jsonString = Encoding.UTF8.GetString( stream.ToArray() );
            }

            Properties.Settings.Default.PreviousFileHashIndex = jsonString;
            Properties.Settings.Default.Save();

            Console.ReadLine();
            return 0;
        }

    }
}
