using System;
using System.Collections;
using System.Drawing;
using System.Runtime.CompilerServices;
using System.Threading;
using System.Diagnostics;

namespace NL.Server {
    public static class NLConsole {

        private const String prefix = "nl.server.";

        private static Int32 writeLine = 0;
        private static String commandBuffer;
        private static Thread commandThread;

        public static Boolean InCommandLine { get {
            if (commandThread != null && commandThread.IsAlive)
                return true; else return false;
        } }

        public static void StartCommandLine(String prefix = prefix) {
            if (InCommandLine) return;
            commandThread = new Thread(StartCommandLineAsync);
            commandThread.Start();
        }

        public static void StopCommandLine() {
            if (InCommandLine) commandThread.Abort();
            ClearLine(Console.WindowTop + Console.WindowHeight - 1);
            Console.SetCursorPosition(0, writeLine);
        }

        private static void StartCommandLineAsync(Object prefixObject) {
            while (true) {
                WriteCommandLine();
                Read();
            }
        }

        private static void WriteCommandLine(String prefix = prefix) {
            Console.ForegroundColor = ConsoleColor.Gray;
            Console.CursorTop = Console.WindowTop + Console.WindowHeight - 1;
            ClearLine(Console.CursorTop);
            Console.Write(prefix);
        }

        public static void Clear() {
            Console.Clear();
            writeLine = 0;
        }

        public static void ClearLine(Int32 line) {
            Console.SetCursorPosition(0, line);
            for (int i = 1; i < Console.BufferWidth; i++) {
                Console.Write(" ");
            }
            Console.SetCursorPosition(0, line);
        }

        public static void Title(String title) {
            Console.Title = title;
        }

        public static String Read(String prefix = prefix) {
            commandBuffer = "";
            ConsoleKeyInfo key;
            do {
                key = Console.ReadKey(true);
                switch (key.Key) {
                    case ConsoleKey.Enter:
                        break;
                    case ConsoleKey.Backspace:
                        if (commandBuffer.Length > 0) {
                            commandBuffer = commandBuffer.Substring(0, commandBuffer.Length-1);
                            WriteCommandLine(prefix + commandBuffer);
                        }
                        break;
                    default:
                        commandBuffer += key.KeyChar;
                        WriteCommandLine(prefix + commandBuffer);
                        break;
                }
            } while (key.Key != ConsoleKey.Enter);
            return commandBuffer;
        }

        public static void WriteLine(String input = "", ConsoleColor color = ConsoleColor.DarkGray) {
            lock (new Object()) {
                Int32 prevTop = Console.CursorTop;
                Int32 prevLeft = Console.CursorLeft;
                Console.SetCursorPosition(0, writeLine);
                Console.ForegroundColor = color;
                Console.WriteLine(input);
                writeLine = Console.CursorTop;
                if (InCommandLine) WriteCommandLine(prefix + commandBuffer);
            }
        }


    }
}
