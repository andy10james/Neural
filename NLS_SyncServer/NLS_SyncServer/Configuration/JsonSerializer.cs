using System;
using System.Collections.Generic;
using System.Runtime.Serialization.Json;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NLS.Configuration {
    public static class JsonSerializer {

        public static String Serialize(Object obj) {
            DataContractJsonSerializer jsonSerializer = new DataContractJsonSerializer(obj.GetType(),
                new DataContractJsonSerializerSettings { UseSimpleDictionaryFormat = true });
            using (MemoryStream stream = new MemoryStream()) {
                jsonSerializer.WriteObject(stream, obj);
                return Encoding.UTF8.GetString(stream.ToArray());
            }
        }

        public static T Deserialize<T>(String str) {
            DataContractJsonSerializer jsonSerializer = new DataContractJsonSerializer(typeof(T),
                new DataContractJsonSerializerSettings { UseSimpleDictionaryFormat = true });
            using (MemoryStream stream = new MemoryStream(Encoding.UTF8.GetBytes(str))) {
                return (T) jsonSerializer.ReadObject(stream);
            }
        }

    }
}
