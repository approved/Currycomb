using Currycomb.Common.Network;
using Serilog;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace Currycomb.Gateway.Network.Services
{
    public class PlayService : IService, IDisposable
    {
        private WrappedPacketStream ServiceStream;
        public PlayService(WrappedPacketStream stream) => ServiceStream = stream;

        public Task RunAsync(CancellationToken cancellationToken = default) => ServiceStream.RunAsync();

        public async ValueTask HandleAsync(WrappedPacket packet)
        {
            await ServiceStream.SendWaitAsync(packet, false);
            Log.Information($"{packet.ClientId} sent packet to PlayService");
        }

        public void Dispose() => ServiceStream.Dispose();
    }
}
