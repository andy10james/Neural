using System;
using System.Collections;
using System.Drawing;
using System.Runtime.CompilerServices;
using System.Threading;

namespace NLS {
    public static class Printer {

        public static int WritePoint = 0;

        public static void Read() {
            Console.ForegroundColor = ConsoleColor.Gray;
            Console.CursorTop = Console.WindowTop + Console.WindowHeight - 1;
            Console.Write("nls.console.");
            Console.ReadLine();
        }

        public static void Write(String input = "", ConsoleColor color = ConsoleColor.DarkGray) {
            lock (new Object()) {
                int prevCurLeft = Console.CursorLeft;
                int prevCurTop = Console.CursorTop;
                Console.ForegroundColor = color;
                Console.SetCursorPosition(0, WritePoint);
                Console.WriteLine(input);
                WritePoint = Console.CursorTop;
                Console.SetCursorPosition(prevCurLeft, prevCurTop);
            }
            
        }

        public static void Clear() {
            Console.Clear();
            WritePoint = 0;
        }

    }
}
