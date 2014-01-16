using System;
using System.Collections.Generic;
using System.Text;
using System.Net.Sockets;
using System.Net;
using NL.Server.Configuration;
using System.Threading;
using NL.Server.View;
using NL.Server.Controllers;
using NL.Common;

namespace NL.Server.Servers {
    internal class ServerHandle {

        public delegate void OnDeathEvent(ServerHandle handle);
        public event OnDeathEvent OnDeath;
        public IPEndPoint EndPoint;

        private readonly IRemoteController _controller;
        private readonly TcpClient _client;
        private Boolean ended = false;

        public ServerHandle(IRemoteController controller, TcpClient client) {
            this._controller = controller;
            this._client = client;
            this.EndPoint = client.Client.RemoteEndPoint as IPEndPoint;
        }

        public void Start() {
            Thread handler = new Thread(Handle);
            handler.Name = Thread.CurrentThread.Name + "." + (_client.Client.RemoteEndPoint as IPEndPoint);
            handler.Start();
        }

        private void Handle() {

            if (EndPoint == null) {
                _client.Close();
                return;
            }

            if (ServerConfiguration.BeepOnConnection) {
                Console.Beep(1000, 80);
                Console.Beep(1500, 80);
                Console.Beep(2000, 200);
            }

            IPAddress ipaddress = EndPoint.Address;
            NetworkStream clientStream = _client.GetStream();

            NLConsole.WriteLine(String.Format(_controller.ClientConnectedMessage, ipaddress), _controller.ClientConnectedColor);

            int bytesRead = 0;
            Byte[] messageBytes = new Byte[64];
            Boolean complete = false;

            while (!ended) {

                try { bytesRead = clientStream.Read(messageBytes, 0, messageBytes.Length); }
                catch {
                    if (!ended) NLConsole.WriteLine(String.Format(_controller.ClientTerminatedMessage, ipaddress), _controller.ClientTerminatedColor);
                    break; 
                }
                if (bytesRead == 0) {
                    complete = true;
                    break;
                }

                String message = Encoding.ASCII.GetString(messageBytes, 0, bytesRead);
                CommandPattern command = CommandPattern.Create(message);
                _controller.InvokeAction(command, _client);

            }

            _client.Close();
            OnDeath.Invoke(this);

            if (complete) {
                NLConsole.WriteLine(String.Format(_controller.ClientDisconnectedMessage, ipaddress), _controller.ClientDisconnectedColor);
            }

            if (ServerConfiguration.BeepOnDisconnection) {
                Console.Beep(2000, 80);
                Console.Beep(1500, 80);
                Console.Beep(1000, 200);
            }



        }

        public void Die() {
            ended = true;
            this._client.Close();
        }

    }
}
