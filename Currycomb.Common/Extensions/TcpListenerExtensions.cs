using System.Net;
using System.Net.Sockets;
using System.Threading;
using System.Threading.Tasks;

namespace Currycomb.Common.Extensions
{
    public static class TcpListenerExtensions
    {
        public static async Task<TcpClient> AcceptTcpClientAsync(this TcpListener listener, CancellationToken ct = default)
        {
            // This is a terrible hack but as far as I can tell there is no better way
            using var reg = ct.Register(() =>
            {
                try
                {
                    if (((IPEndPoint?)listener.Server.LocalEndPoint)?.Port is int port)
                    {
                        using TcpClient client = new TcpClient();
                        client.Connect(IPAddress.Loopback, port);
                    }
                }
                catch
                {
                    // this is expected to fail
                }
            });

            TcpClient client = await listener.AcceptTcpClientAsync();
            ct.ThrowIfCancellationRequested();
            return client;
        }
    }
}
