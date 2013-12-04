using System;
using System.Collections;
using System.Drawing;
using System.Runtime.CompilerServices;
using System.Threading;

namespace NLS {
    public static class Printer {

        public static int WritePoint = 0;

        static Printer() {
            SetCommandLine();
        }

        /*public static void PrintFileHashIndex(IDictionary dictionary) {
            
            lock (new Object()) {
                Console.ForegroundColor = ConsoleColor.DarkYellow;
                Console.SetCursorPosition(0, WritePoint);
                foreach (var key in dictionary.Keys)
                    Console.WriteLine("{0,-40}{1,-32}", key, dictionary[key]);
                WritePoint = Console.CursorTop;
                SetCommandLine();
            }
            
        }*/

        private static void SetCommandLine() {
            Console.ForegroundColor = ConsoleColor.Gray;
            Console.CursorTop = Console.WindowTop + Console.WindowHeight - 1;
            Console.Write("NLS.Console >> ");
        }

        public static void Write(String input = "") {
            lock (new Object()) {
                Console.ForegroundColor = ConsoleColor.DarkGray;
                Console.SetCursorPosition(0, WritePoint);
                Console.WriteLine(input);
                WritePoint = Console.CursorTop;
                SetCommandLine();
            }
            
        }

    }
}
