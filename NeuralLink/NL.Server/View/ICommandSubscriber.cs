using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NL.Server.View {
    internal interface ICommandSubscriber {
        void OnConsoleCommand(String[] command);
    }
}
