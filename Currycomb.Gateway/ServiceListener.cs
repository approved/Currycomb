using System;
using System.Net.Sockets;
using System.Net;
using System.Threading.Tasks;
using System.Threading;
using Serilog;

namespace Currycomb.Gateway
{
    public class ServiceListener<TManager, TService>
        where TManager : ServiceManager<TService>
        where TService : IService
    {
        private readonly TManager _manager;
        private readonly IPEndPoint _endpoint;

        private readonly Func<TcpClient, TService> _serviceFactory;

        public ServiceListener(TManager manager, IPEndPoint endpoint, Func<TcpClient, TService> serviceFactory)
        {
            _manager = manager;
            _endpoint = endpoint;
            _serviceFactory = serviceFactory;
        }

        public async Task AcceptConnections(CancellationToken ct = default)
        {
            ct.Register(() =>
            {
                try
                {
                    using TcpClient client = new TcpClient();
                    client.Connect(IPAddress.Loopback, _endpoint.Port);
                }
                catch
                {
                    // this is expected to fail
                }
            });

            Log.Information("Starting listener on port {port}", _endpoint.Port);
            TcpListener listener = new TcpListener(_endpoint);
            listener.Start();

            while (!ct.IsCancellationRequested)
            {
                TcpClient client = await listener.AcceptTcpClientAsync();
                if (ct.IsCancellationRequested)
                    return;

                TService service = _serviceFactory(client);
                await _manager.AddServiceInstance(service);
            }
        }
    }
}
