using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace NL.Server.Configuration {
    internal static class ServerConfiguration {

        public static Uri Repository = new Uri("G:\\Development\\Live\\CGN_NeuralLink\\TestRepository");
        public static Int16 QueryPort = 4010;
        public static Int16 DeliveryPort = 4020;
        public static Boolean BeepOnConnection = true;
        public static Boolean BeepOnDisconnection = true;

    }
}
