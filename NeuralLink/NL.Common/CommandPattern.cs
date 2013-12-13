using System;
using System.Linq;
using System.Text;

namespace NL.Common {
    public class CommandPattern {

        public String Command { get { return _command; } }
        public String[] Parameters { get { return _parameters.ToArray(); } }
        public String Original { get { return _original; } }

        private String _command;
        private String[] _parameters;
        private String _original;

        public static CommandPattern Create(String commandString) {


            String[] commandSplit = commandString.Split(' ');
            String command = commandSplit[0].ToUpper();
            String[] parameters = new String[commandSplit.Length-1];
            Array.Copy(commandSplit, 1, parameters, 0, parameters.Length);

            return new CommandPattern() {
                _command = command,
                _parameters = parameters,
                _original = commandString
            };

        }

        public override String ToString() {
            StringBuilder output = new StringBuilder();
            output.Append(_command);
            foreach (String param in _parameters) {
                output.Append(" ");
                output.Append(param);
            }
            return output.ToString();
        }
    }
}
