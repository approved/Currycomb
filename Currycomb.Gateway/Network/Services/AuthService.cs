using System;
using System.Threading;
using System.Threading.Channels;
using System.Threading.Tasks;
using Currycomb.Common.Network;
using Serilog;

namespace Currycomb.Gateway.Network.Services
{
    public class AuthService : IService, IDisposable
    {
        private WrappedPacketStream ServiceStream;
        public AuthService(WrappedPacketStream stream) => ServiceStream = stream;

        public void Dispose() => ServiceStream.Dispose();
        public Task RunAsync(CancellationToken cancellationToken = default) => ServiceStream.RunAsync(cancellationToken);

        public async ValueTask HandleAsync(bool isMeta, WrappedPacket packet)
        {
            Guid id = packet.ClientId;
            await ServiceStream.SendWaitAsync(packet, isMeta);
            Log.Information("AuthService.HandleAsync | Client {@clientId} sent packet to AuthService", id);
        }

        public async Task ReadPacketsToChannelAsync(ChannelWriter<WrappedPacketContainer> channel, CancellationToken ct = default)
        {
            while (!ct.IsCancellationRequested)
            {
                await channel.WriteAsync(await ServiceStream.ReadAsync(true, ct), ct);
            }
        }
    }
}
