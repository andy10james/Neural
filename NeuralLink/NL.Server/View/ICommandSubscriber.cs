using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NL.Server.View {
    public interface ICommandSubscriber {
        void OnConsoleCommand(String[] command);
    }
}
