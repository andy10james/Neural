using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NL.Common;
using NL.Server.View;

namespace NL.Server.Controllers {
    public abstract class IController {

        protected delegate void ActionDelegate(String[] parameters);
        protected Dictionary<String, ActionDelegate> ActionDictionary;

        public IController() {
            NLConsole.Subscribe(this);
            ActionDictionary = new Dictionary<String, ActionDelegate>();
        }

        public Boolean InvokeAction(CommandPattern command) {
            ActionDelegate action;
            ActionDictionary.TryGetValue(command.Command, out action);
            if (action != null) {
                action.Invoke(command.Parameters);
                return true;
            }
            return false;
        }

        protected virtual void DefaultAction(CommandPattern command) { }

    }
}
