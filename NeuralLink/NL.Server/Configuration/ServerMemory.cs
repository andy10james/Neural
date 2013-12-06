using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NL.Common;
using NL.Server.Properties;
using NL.Server.Configuration;

namespace NL.Server.Configuration {
    internal static class ServerMemory {

        public static FileHashIndex Index {
            get {
                return JsonSerializer.Deserialize<FileHashIndex>(Settings.Default.Index);
            }
            set {
                Settings.Default.Index = JsonSerializer.Serialize(value);
                Settings.Default.Save();
            }
        }

    }
}
