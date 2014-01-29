using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Net;
using System.Net.Sockets;
using NL.Common;
using NL.Server.View;

namespace NL.Server.Controllers {
    internal abstract class IRemoteController : IController {

        //public readonly String ConnectedMessage = "Query Server online and listening on port {0}.";
        //public readonly String DisconnectedMessage = "Query Server disconnected from port {0}.";

        public readonly ConsoleColor ClientConnectedColor = ConsoleColor.DarkYellow;
        public readonly ConsoleColor ClientTerminatedColor = ConsoleColor.DarkRed;
        public readonly ConsoleColor ClientDisconnectedColor = ConsoleColor.DarkYellow;
        public readonly ConsoleColor ClientTransmittedColor = ConsoleColor.Yellow;

        protected delegate Boolean RemoteActionDelegate(String[] parameters, TcpClient client = null);
        protected Dictionary<String, RemoteActionDelegate> RemoteActionDictionary;

        public IRemoteController() {
            RemoteActionDictionary = new Dictionary<String, RemoteActionDelegate>();
        }

        public Boolean InvokeAction(CommandPattern command, TcpClient client) {
            RemoteActionDelegate action;
            RemoteActionDictionary.TryGetValue(command.Command, out action);
            if (action != null) {
                action.Invoke(command.Parameters, client);
                return true;
            }
            DefaultAction(command, client);
            return false;
        }

        protected abstract void DefaultAction(CommandPattern command, TcpClient client);

    }
}

