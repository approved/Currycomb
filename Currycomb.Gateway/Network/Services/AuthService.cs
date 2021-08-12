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

        public async ValueTask HandleAsync(WrappedPacket packet)
        {
            Guid id = packet.ClientId;
            Log.Warning($"{id} attempting to complete handshake");

            await ServiceStream.SendWaitAsync(packet, false);
            Log.Information($"{id} sent packet to AuthService");
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
