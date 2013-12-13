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
        static void Main(string[] args) {

            Console.Title = "Neural Link Server Communication Test Console";
            Console.ForegroundColor = ConsoleColor.Red;
            Console.WriteLine("NL.Server.Tester [DEV-Kana, DEV-Dilerium]");
            Console.WriteLine("Console for NL developers only.");
            Console.WriteLine();

            Console.ResetColor();
            TcpClient client = new TcpClient("localhost", 9321);
            NetworkStream stream = client.GetStream();

            Thread senderThread = new Thread(Sender);
            Thread receiverThread = new Thread(Receiver);
            senderThread.Start(stream);
            receiverThread.Start(stream);
            
            
        }

        static void Sender(Object streamObject) {
            NetworkStream stream = streamObject as NetworkStream;
            while (true) {
                Console.ForegroundColor = ConsoleColor.DarkYellow;
                Console.ResetColor();
                String message = Console.ReadLine();
                Byte[] messageBytes = Encoding.ASCII.GetBytes(message);
                stream.Write(messageBytes, 0, messageBytes.Length);
            }
        }

        static void Receiver(Object streamObject) {
            NetworkStream stream = streamObject as NetworkStream;
            while (true) {
                Byte[] receiveBuffer = new Byte[128];
                Int32 received = stream.Read(receiveBuffer, 0, receiveBuffer.Length);
                String response = Encoding.ASCII.GetString(receiveBuffer, 0, received);
                Console.ForegroundColor = ConsoleColor.DarkGreen;
                Console.WriteLine(response);
                Console.ResetColor(); 
            }
        }


    }
}
