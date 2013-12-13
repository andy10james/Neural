using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NL.Common;

namespace NL.Server.View {
    public interface ICommandSubscriber {
        Boolean OnConsoleCommand(CommandPattern command);
    }
}
