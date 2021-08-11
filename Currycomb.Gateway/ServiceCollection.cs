using System.Threading.Tasks;
using Currycomb.Common.Network;
using System.Threading.Channels;
using System.Threading;
using Serilog;

namespace Currycomb.Gateway
{
    public struct ServiceCollection
    {
        private readonly IService[] _services;

        public ServiceCollection(params IService[] services) => _services = services;

        public async Task ReadPacketsToChannel(Channel<WrappedPacketContainer> queue, CancellationToken ct = default)
        {
            Log.Debug("Reading packets from {serviceCount} services", _services.Length);

            IService[] services = _services;
            Task[] tasks = new Task[services.Length];

            for (int i = 0; i < services.Length; i++)
            {
                Log.Debug("Setting up reader for packets from {serviceCount}", services[i].GetType());

                IService service = services[i];
                tasks[i] = Task.Run(async () =>
                {
                    Log.Debug("Starting reader for packets {serviceType}", service.GetType());

                    await foreach (var packet in service.ReadPacketsAsync(ct))
                        await queue.Writer.WriteAsync(packet);
                });
            }

            Log.Debug("Finished service collection initialization");
            await Task.WhenAll(tasks);
            Log.Debug("Finished!?");
        }
    }
}
