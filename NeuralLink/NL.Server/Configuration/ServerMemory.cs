using NL.Common;
using NL.Server.Properties;

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
