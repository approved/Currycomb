using System;
using System.Collections.Generic;
using System.Net.Sockets;
using System.Net;
using System.Threading;

namespace Currycomb.Gateway.ServiceCom
{
    public class WebSocketServer
    {
        public readonly List<WebSocketSession> Clients = new();

        public event EventHandler<WebSocketSession> ClientConnected;
        public event EventHandler<WebSocketSession> ClientDisconnected;

        private bool _listening;

        public void Close() => _listening = false;

        public void Listen(int port)
        {
            if (_listening) throw new Exception("Already listening!");
            _listening = true;

            TcpListener server = new TcpListener(IPAddress.Any, port);
            server.Start();


            ThreadPool.QueueUserWorkItem(_ =>
            {
                while (_listening)
                {
                    WebSocketSession session = new WebSocketSession(server.AcceptTcpClient());
                    session.HandshakeCompleted += (__, ___) =>
                    {
                        Clients.Add(session);
                    };

                    session.Disconnected += (__, ___) =>
                    {
                        Clients.Remove(session);

                        ClientDisconnected?.Invoke(this, session);
                        session.Dispose();
                    };

                    ClientConnected?.Invoke(this, session);
                    session.Start();
                }

                server.Stop();
            });
        }
    }
}
