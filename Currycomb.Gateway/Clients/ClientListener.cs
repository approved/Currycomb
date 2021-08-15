using System.Net;
using System.Net.Sockets;
using System.Threading;
using System.Threading.Tasks;
using Currycomb.Common.Extensions;

namespace Currycomb.Gateway.Clients
{
    public class ClientListener
    {
        public readonly ClientCollection _clients;
        private readonly IPEndPoint _endPoint;

        public ClientListener(ClientCollection clients, IPEndPoint endPoint)
        {
            _clients = clients;
            _endPoint = endPoint;
        }

        public async Task AcceptConnections(CancellationToken ct = default)
        {
            TcpListener listener = new TcpListener(_endPoint);
            listener.Start();

            while (!ct.IsCancellationRequested)
            {
                TcpClient client = await listener.AcceptTcpClientAsync(ct);
                if (ct.IsCancellationRequested)
                    return;

                _clients.AddClient(new(client.GetStream()));
            }
        }
    }
}
