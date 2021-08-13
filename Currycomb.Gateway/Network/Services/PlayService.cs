using Currycomb.Common.Network;
using Serilog;
using System;
using System.Threading;
using System.Threading.Channels;
using System.Threading.Tasks;

namespace Currycomb.Gateway.Network.Services
{
    public class PlayService : IService, IDisposable
    {
        private WrappedPacketStream ServiceStream;
        public PlayService(WrappedPacketStream stream) => ServiceStream = stream;

        public Task RunAsync(CancellationToken cancellationToken = default) => ServiceStream.RunAsync();

        public async ValueTask HandleAsync(bool isMeta, WrappedPacket packet)
        {
            await ServiceStream.SendAsync(packet, isMeta);
            Log.Information("PlayService.HandleAsync | Client {@clientId} sent packet to PlayService", packet.ClientId);
        }

        public async Task ReadPacketsToChannelAsync(ChannelWriter<WrappedPacketContainer> channel, CancellationToken ct = default)
        {
            while (!ct.IsCancellationRequested)
            {
                await channel.WriteAsync(await ServiceStream.ReadAsync(true, ct), ct);
            }
        }

        public void Dispose() => ServiceStream.Dispose();
    }
}
