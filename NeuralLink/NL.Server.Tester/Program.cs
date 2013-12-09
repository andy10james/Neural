using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.Net;
using System.Net.Sockets;

namespace NL.Server.Tester {
    class Program {
        static void Main(string[] args) {

            Console.Title = "Neural Link Server Communication Test Console";
            Console.ForegroundColor = ConsoleColor.Red;
            Console.WriteLine("NL.Server.Tester [DEV-Kana, DEV-Dilerium]");
            Console.WriteLine("Console for NL developers only.");
            Console.WriteLine();

            Console.ResetColor();
            TcpClient client = new TcpClient("localhost", 9321);
            NetworkStream stream = client.GetStream();

            while (true) {
                Console.ForegroundColor = ConsoleColor.DarkYellow;
                Console.Write("Send message: ");
                Console.ResetColor();
                String message = Console.ReadLine();
                Byte[] messageBytes = Encoding.ASCII.GetBytes(message);
                stream.Write(messageBytes, 0, messageBytes.Length);
            }
            
        }
    }
}
