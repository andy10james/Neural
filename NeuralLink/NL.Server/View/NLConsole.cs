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

        private static String prompt = Strings.NLConsolePrompt;

        private static Int32 writeLine = 0;
        private static String commandBuffer;
        private static Thread commandThread;
        private static List<IController> subscribers;
        private static Object lockObject = new Object();

        static NLConsole() {
            subscribers = new List<IController>();
        }

        public static Boolean InCommandLine { get {
            if (commandThread != null && commandThread.IsAlive)
                return true; else return false;
        } }

        public static void StartCommandLine() {
            if (InCommandLine) return;
            commandThread = new Thread(CommandLineAsync);
            commandThread.Name = Thread.CurrentThread.Name + ".Command";
            commandThread.Start();
        }

        public static void StopCommandLine() {
            lock (lockObject) {
                if (!InCommandLine) return;
                Int32 line = writeLine >= Console.WindowHeight ?
                   writeLine : Console.WindowHeight - 1;
                ClearLine(line);
                Console.SetCursorPosition(0, writeLine);
                commandThread.Abort();
            }
        }

        private static void CommandLineAsync(Object prefixObject) {
            while (true) {
                WriteCommandLine(reset: false);
                String command = Read();
                CommandPattern commandPattern = CommandPattern.Create(command);
                Boolean responded = NotifySubscribers(commandPattern);
                if (!responded) {
                    WriteLine("Unrecognised action.", ConsoleColor.White);
                }
            }
        }

        private static void WriteCommandLine(String prefix = null, Boolean reset = true) {
            lock (lockObject) {
                if (prefix == null) prefix = prompt;
                Console.ForegroundColor = ConsoleColor.Gray;
                ClearLine(writeLine);
                Console.Write(prefix);
            }
        }

        public static void Clear() {
            lock (lockObject) {
                Console.Clear();
                writeLine = 0;
                if (InCommandLine) {
                    WriteCommandLine();
                }
            }
        }

        public static void ClearLine(Int32 line) {
            lock (lockObject) {
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
            commandBuffer = "";
            ConsoleKeyInfo key;
            do {
                key = Console.ReadKey(true);
                switch (key.Key) {
                    case ConsoleKey.Enter:
                        break;
                    case ConsoleKey.Backspace:
                        if (commandBuffer.Length > 0) {
                            commandBuffer = commandBuffer.Substring(0, commandBuffer.Length - 1);
                            WriteCommandLine(prompt + commandBuffer, false);
                        }
                        break;
                    default:
                        commandBuffer += key.KeyChar;
                        WriteCommandLine(prompt + commandBuffer, false);
                        break;
                }
            } while (key.Key != ConsoleKey.Enter);
            return commandBuffer;
        }

        public static void WriteLine(String input = "", ConsoleColor color = ConsoleColor.DarkGray) {
            lock (lockObject) {
                Console.SetCursorPosition(0, writeLine);
                Int32 lineOverflow = input.Length / Console.BufferWidth
                    + ((input.Length % Console.BufferWidth > 0) ? 1 : 0);
                for (int i = 0; i < lineOverflow; i++) {
                    ClearLine(Console.CursorTop + i);
                }
                Console.SetCursorPosition(0, writeLine);
                Console.ForegroundColor = color;
                Console.WriteLine(input);
                writeLine = Console.CursorTop;
                if (InCommandLine) WriteCommandLine(prompt + commandBuffer);
            }
        }

        private static Boolean NotifySubscribers(CommandPattern commandPattern) {
            Boolean responded = false;
            foreach (IController subscriber in subscribers) {
                Boolean response = subscriber.InvokeAction(commandPattern);
                if (response) responded = true;
            }
            return responded;
        }

        public static void Subscribe(IController subscriber) {
            if (!subscribers.Contains(subscriber)) {
                subscribers.Add(subscriber);
            }
        }   

    }
}
