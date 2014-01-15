using System;
using System.Collections.Generic;
using System.Drawing;
using System.Runtime.CompilerServices;
using System.Threading;
using System.Diagnostics;
using NL.Common;
using NL.Server.Controllers;
using NL.Server.Configuration;

namespace NL.Server.View {
    public static class NLConsole {

        private static readonly String Prompt = Strings.NLConsolePrompt;

        private static Int32 _writeLine = 0;
        private static String _commandBuffer;
        private static Thread _commandThread;
        private static List<String> _commandHistory;
        private readonly static List<IController> Subscribers;
        private readonly static Object LockObject = new Object();

        static NLConsole() {
            Subscribers = new List<IController>();
            _commandHistory = new List<String>();
        }

        public static Boolean InCommandLine { get {
            if (_commandThread != null && _commandThread.IsAlive)
                return true; else return false;
        } }

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
                Boolean responded = NotifySubscribers(commandPattern);
                if (!responded) {
                    WriteLine(Strings.UnrecognisedAction, ConsoleColor.White);
                }
            }
        }

        private static void WriteCommandLine(String prefix = null, Boolean reset = true) {
            lock (LockObject) {
                if (prefix == null) prefix = Prompt;
                Console.ForegroundColor = ConsoleColor.Gray;
                ClearLine(_writeLine);
                Console.Write(prefix);
            }
        }

        public static void Clear() {
            lock (LockObject) {
                Console.Clear();
                _writeLine = 0;
                if (InCommandLine) {
                    WriteCommandLine();
                }
            }
        }

        public static void ClearLine(Int32 line) {
            lock (LockObject) {
                Console.SetCursorPosition(0, line);
                for (int i = 1; i < Console.BufferWidth; i++) {
                    Console.Write(" ");
                }
                Console.SetCursorPosition(0, line);
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
            lock (LockObject) {
                Console.SetCursorPosition(0, _writeLine);
                Int32 lineOverflow = input.Length / Console.BufferWidth
                    + ((input.Length % Console.BufferWidth > 0) ? 1 : 0);
                for (int i = 0; i < lineOverflow; i++) {
                    ClearLine(Console.CursorTop + i);
                }
                Console.SetCursorPosition(0, _writeLine);
                Console.ForegroundColor = color;
                Console.WriteLine(input);
                _writeLine = Console.CursorTop;
                if (InCommandLine) WriteCommandLine(Prompt + _commandBuffer);
            }
        }

        private static Boolean NotifySubscribers(CommandPattern commandPattern) {
            Boolean responded = false;
            IController[] subscribers = new IController[Subscribers.Count];
            Subscribers.CopyTo(subscribers);
            foreach (IController subscriber in subscribers) {
                Boolean response = subscriber.InvokeAction(commandPattern);
                if (response) responded = true;
            }
            return responded;
        }

        public static void Subscribe(IController subscriber) {
            if (!Subscribers.Contains(subscriber)) {
                Subscribers.Add(subscriber);
            }
        }   

    }
}
