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

        private static void SetCommandLine() {
            Console.ForegroundColor = ConsoleColor.Gray;
            Console.CursorTop = Console.WindowTop + Console.WindowHeight - 1;
            Console.Write("nls.console.");
        }

        public static void Write(String input = "", ConsoleColor color = ConsoleColor.DarkGray) {
            lock (new Object()) {
                Console.ForegroundColor = color;
                Console.SetCursorPosition(0, WritePoint);
                Console.WriteLine(input);
                WritePoint = Console.CursorTop;
                SetCommandLine();
            }
            
        }

    }
}
