using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.IO;
using System.Net;
using System.Net.Sockets;

namespace NL.Server.Tester {
    class Program {

        static Thread senderThread;
        static Thread receiverThread;

        static void Main(string[] args) {

            Console.Title = "Neural Link Server Communication Test Console";
            Console.ForegroundColor = ConsoleColor.Red;
            Console.WriteLine("NL.Server.Tester [DEV-Kana, DEV-Dilerium, DEV-Seckar]");
            Console.WriteLine("Console for NL developers only.");
            Console.WriteLine();

            Console.ResetColor();
            TcpClient client = new TcpClient("localhost", 4010);
            NetworkStream stream = client.GetStream();

            senderThread = new Thread(Sender);
            receiverThread = new Thread(Receiver);
            senderThread.Start(stream);
            receiverThread.Start(stream);

            senderThread.Join();

            Console.Write("Servers ended");

        }

        static void Sender(Object streamObject) {
            NetworkStream stream = streamObject as NetworkStream;
            while (true) {
                String message = Console.ReadLine();
                Byte[] messageBytes = Encoding.ASCII.GetBytes(message);
                stream.Write(messageBytes, 0, messageBytes.Length);
            }
        }

        static void Receiver(Object streamObject) {
            NetworkStream stream = streamObject as NetworkStream;
            while (true) {
                Byte[] receiveBuffer = new Byte[128];
                Int32 received = 0;
                try { 
                    received = stream.Read(receiveBuffer, 0, receiveBuffer.Length); 
                } 
                catch {
                    Console.ForegroundColor = ConsoleColor.DarkRed;
                    Console.WriteLine("NL.Server abnormally disconnected.");
                    break;
                }
                if (received == 0) {
                    Console.ForegroundColor = ConsoleColor.DarkYellow;
                    Console.WriteLine("NL.Server disconnected.");
                    break;
                }
                String response = Encoding.ASCII.GetString(receiveBuffer, 0, received);
                Console.ForegroundColor = ConsoleColor.DarkGreen;
                Console.WriteLine(response);
                Console.ResetColor(); 
            }

            senderThread.Interrupt();
            receiverThread.Interrupt();
        }


    }
}
