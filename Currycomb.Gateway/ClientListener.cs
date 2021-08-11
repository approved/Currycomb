using System.Net;
using System.Net.Sockets;
using System.Threading;
using System.Threading.Tasks;

namespace Currycomb.Gateway
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
            // TODO: Move this to an extension on TcpListener
            ct.Register(() =>
            {
                try
                {
                    using TcpClient client = new TcpClient();
                    client.Connect(IPAddress.Loopback, _endPoint.Port);
                }
                catch
                {
                    // this is expected to fail
                }
            });

            TcpListener listener = new TcpListener(_endPoint);
            listener.Start();

            while (!ct.IsCancellationRequested)
            {
                TcpClient client = await listener.AcceptTcpClientAsync();
                if (ct.IsCancellationRequested)
                    return;

                _clients.AddClient(new(client.GetStream()));
            }
        }
    }
}
