using System;
using System.Collections.Generic;
using NL.Common;

namespace NL.Server.Controllers {
    public abstract class IController {

        protected delegate Boolean ActionDelegate(String[] parameters);
        protected Dictionary<String, ActionDelegate> ActionDictionary;

        public IController() {
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

        public virtual String GetName() {
            return this.GetType().Name;
        }

        protected virtual void DefaultAction(CommandPattern command) { }

    }
}
