using System;
using System.Collections.Generic;
using System.Threading;
using System.Windows.Forms.VisualStyles;
using NL.Common;
using NL.Server.Controllers;
using NL.Server.Configuration;

namespace NL.Server.View {
    public static class NLConsole {

        private static readonly String Prompt = UIStrings.NLConsolePrompt;

        private static Int32 _writeLine;
        private static Int32 _writeLeft;
        private static String _commandBuffer;
        private static Thread _commandThread;
        private static List<String> _commandHistory;
        private readonly static List<IController> Controllers;
        private readonly static Object LockObject = new Object();

        static NLConsole() {
            Controllers = new List<IController>();
            _commandHistory = new List<String>();
        }

        public static Boolean InCommandLine {
            get {
                if (_commandThread != null && _commandThread.IsAlive)
                    return true;
                else return false;
            }
        }

        public static void StartCommandLine() {
            if (InCommandLine) return;
            _commandThread = new Thread(CommandLineAsync);
            _commandThread.Name = Thread.CurrentThread.Name + ".Command";
            _commandThread.Start();
        }

        public static void StopCommandLine() {
            lock (LockObject) {
                if (!InCommandLine) return;
                Int32 line = _writeLine >= Console.WindowHeight ?
                   _writeLine : Console.WindowHeight - 1;
                ClearLine(line);
                Console.SetCursorPosition(0, _writeLine);
                _commandThread.Abort();
            }
        }

        private static void CommandLineAsync(Object prefixObject) {
            while (true) {
                WriteCommandLine(reset: false);
                String command = Read();
                CommandPattern commandPattern = CommandPattern.Create(command);
                Boolean responded = InvokeControllers(commandPattern);
                if (!responded) {
                    WriteLine(UIStrings.UnrecognisedAction, ConsoleColor.White);
                }
            }
        }

        private static void WriteCommandLine(String prefix = null, Boolean reset = true) {
            lock (LockObject) {
                if (prefix == null) prefix = Prompt;
                Console.ForegroundColor = ConsoleColor.Gray;
                Int32 printOn = _writeLine;
                if (_writeLeft != 0) printOn = _writeLine + 1;
                Console.CursorTop = printOn;
                Console.CursorLeft = 0;
                ClearLine(printOn);
                Console.Write(prefix);
            }
        }

        public static void Clear() {
            lock (LockObject) {
                Console.Clear();
                _writeLine = 0;
                _writeLeft = 0;
                if (InCommandLine) {
                    WriteCommandLine();
                }
            }
        }

        public static void ClearLine(Int32 line) {
            lock (LockObject) {
                Int32 curLeft = Console.CursorLeft;
                Int32 curLine = Console.CursorTop;
                Console.SetCursorPosition(0, line);
                for (int i = 1; i < Console.BufferWidth; i++) {
                    Console.Write(" ");
                }
                Console.SetCursorPosition(curLeft, curLine);
            }
        }

        public static void ClearForward(Int32 lines = 1) {
            lock (LockObject) {
                Int32 curLeft = Console.CursorLeft;
                Int32 curLine = Console.CursorTop;
                Int32 buffer = Console.BufferWidth - Console.CursorLeft;
                for (int i = 1; i < buffer; i++) {
                    Console.Write(" ");
                }
                for (int i = 1; i < lines; i++) {
                    ClearLine(curLine+i);
                }
                Console.SetCursorPosition(curLeft, curLine);
            }
        }

        public static void Title(String title) {
            Console.Title = title;
        }

        public static String Read() {
            _commandBuffer = "";
            ConsoleKeyInfo key;
            Int32 commandLookup = _commandHistory.Count;
            do {
                key = Console.ReadKey(true);
                switch (key.Key) {
                    case ConsoleKey.Enter:
                        break;
                    case ConsoleKey.Backspace:
                        if (_commandBuffer.Length > 0) {
                            _commandBuffer = _commandBuffer.Substring(0, _commandBuffer.Length - 1);
                            WriteCommandLine(Prompt + _commandBuffer, false);
                        }
                        break;
                    case ConsoleKey.UpArrow:
                        if (commandLookup <= 0) {
                            break;
                        }
                        commandLookup--;
                        _commandBuffer = _commandHistory[commandLookup];
                        WriteCommandLine(Prompt + _commandBuffer, false);
                        break;
                    case ConsoleKey.DownArrow:
                        commandLookup++;
                        if (commandLookup >= _commandHistory.Count) {
                            commandLookup = _commandHistory.Count;
                            _commandBuffer = String.Empty;
                        } else {
                            _commandBuffer = _commandHistory[commandLookup];
                        }
                        WriteCommandLine(Prompt + _commandBuffer, false);
                        break;
                    default:
                        _commandBuffer += key.KeyChar;
                        WriteCommandLine(Prompt + _commandBuffer, false);
                        break;
                }
            } while (key.Key != ConsoleKey.Enter);
            _commandHistory.Add(_commandBuffer);
            return _commandBuffer;
        }

        public static void WriteLine(String input = "", ConsoleColor color = ConsoleColor.DarkGray) {
            Write(input + "\n", color);
        }

        public static void Write(String input, ConsoleColor color = ConsoleColor.DarkGray) {
            lock (LockObject) {
                Console.SetCursorPosition(_writeLeft, _writeLine);
                Int32 lineOverflow = (input.Length + _writeLeft) / Console.BufferWidth
                    + ((input.Length % Console.BufferWidth > 0) ? 1 : 0);
                ClearForward(lineOverflow);
                Console.SetCursorPosition(_writeLeft, _writeLine);
                Console.ForegroundColor = color;
                Console.Write(input);
                _writeLine = Console.CursorTop;
                _writeLeft = Console.CursorLeft;
                if (InCommandLine) WriteCommandLine(Prompt + _commandBuffer);
            }
        }

        private static Boolean InvokeControllers(CommandPattern commandPattern) {
            Boolean responded = false;
            IController[] controllers = new IController[Controllers.Count];
            Controllers.CopyTo(controllers);
            foreach (IController subscriber in controllers) {
                Boolean response = subscriber.InvokeAction(commandPattern);
                if (response) responded = true;
            }
            return responded;
        }

        public static void AddController(IController controller) {
            if (!Controllers.Contains(controller)) {
                Controllers.Add(controller);
            }
        }

    }
}
