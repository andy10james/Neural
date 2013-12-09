using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Net.Sockets;
using System.Net;

using NL.Server.View;

namespace NL.Server.Servers {
    internal class QueryServer : IServer {

        public List<Thread> Handlers { get { return _handlers; } }
        public String ConnectedMessage { get { return _connectedMessage; } }
        public String DisconnectedMessage { get { return _disconnectedMessage; } }

        private const String _connectedMessage = "Query Server online and listening on port {0}";
        private const String _disconnectedMessage = "Query Server disconnected from port {0}";
        private List<Thread> _handlers = new List<Thread>();
        

        public void HandleClient(Object clientObject) {
            
            _handlers.Add(Thread.CurrentThread);

            TcpClient client = (TcpClient)clientObject;
            IPEndPoint endPoint = client.Client.RemoteEndPoint as IPEndPoint;
            IPAddress ipaddress = endPoint.Address;
            NetworkStream clientStream = client.GetStream();

            NLConsole.WriteLine("[" + ipaddress + "] Connection with client was initiated.", ConsoleColor.DarkYellow);

            int bytesRead = 0;
            Byte[] messageBytes = new Byte[64];

            while (true) {
                try { bytesRead = clientStream.Read(messageBytes, 0, messageBytes.Length); } catch { NLConsole.WriteLine("[" + ipaddress + "] Connection with client was terminated.", ConsoleColor.DarkRed); break; }
                if (bytesRead == 0) break;
                String message = Encoding.ASCII.GetString(messageBytes, 0, bytesRead);
                NLConsole.WriteLine("[" + ipaddress + "] Transmitted: " + message, ConsoleColor.Yellow);
            }

            NLConsole.WriteLine("[" + ipaddress + "] Connection with client was complete.", ConsoleColor.DarkYellow);

            client.Close();

            _handlers.Remove(Thread.CurrentThread);

        }
    }
}
