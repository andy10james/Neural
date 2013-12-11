using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Net.Sockets;
using System.Net;

using NL.Server.View;

namespace NL.Server.Servers {
    internal class QueryServer : IServer, ICommandSubscriber {

        public List<Thread> Handlers { get { return _handlers; } }
        public String ConnectedMessage { get { return _connectedMessage; } }
        public String DisconnectedMessage { get { return _disconnectedMessage; } }

        private const String _connectedMessage = "Query Server online and listening on port {0}";
        private const String _disconnectedMessage = "Query Server disconnected from port {0}";
        private const String _clientConnectedMessage = "[{0}] Connection with client was initiated.";
        private const String _clientTerminatedMessage = "[{0}] Connection with client was terminated by an exception: {1}.";
        private const String _clientDisconnectedMessage = "[{0}] Connection with client was ended.";
        private const String _clientTransmittedMessage = "[{0}] Transmitted: {1}";

        private const ConsoleColor _clientConnectedColor = ConsoleColor.DarkYellow;
        private const ConsoleColor _clientTerminatedColor = ConsoleColor.DarkRed;
        private const ConsoleColor _clientDisconnectedColor = ConsoleColor.DarkYellow;
        private const ConsoleColor _clientTransmittedColor = ConsoleColor.Yellow;

        private List<Thread> _handlers = new List<Thread>();

        public QueryServer() {
            NLConsole.Subscribe(this);
        }

        public void HandleClient(Object clientObject) {
            
            _handlers.Add(Thread.CurrentThread);

            TcpClient client = (TcpClient)clientObject;
            IPEndPoint endPoint = client.Client.RemoteEndPoint as IPEndPoint;
            IPAddress ipaddress = endPoint.Address;
            NetworkStream clientStream = client.GetStream();

            NLConsole.WriteLine(String.Format(_clientConnectedMessage, ipaddress), _clientConnectedColor);

            int bytesRead = 0;
            Byte[] messageBytes = new Byte[64];

            while (true) {
                try { bytesRead = clientStream.Read(messageBytes, 0, messageBytes.Length); } 
                catch (Exception e) { NLConsole.WriteLine(String.Format(_clientTerminatedMessage, ipaddress, e.GetType().Name), _clientTerminatedColor); break; }
                if (bytesRead == 0) break;
                String message = Encoding.ASCII.GetString(messageBytes, 0, bytesRead);
                NLConsole.WriteLine(String.Format(_clientTransmittedMessage, ipaddress, message), _clientTransmittedColor);
            }

            NLConsole.WriteLine(String.Format(_clientDisconnectedMessage, ipaddress), _clientDisconnectedColor);

            client.Close();

            _handlers.Remove(Thread.CurrentThread);

        }

        public void OnConsoleCommand(String[] e) {
            NLConsole.WriteLine("You executed the following command: " + e[0], ConsoleColor.White);
        }


    }
}
