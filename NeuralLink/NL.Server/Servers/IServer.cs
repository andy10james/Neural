using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;

namespace NL.Server.Servers {
    internal interface IServer {
        List<Thread> Handlers { get; }
        String ConnectedMessage { get; }
        String DisconnectedMessage { get; }
        void HandleClient(Object clientObject);
    }
}

