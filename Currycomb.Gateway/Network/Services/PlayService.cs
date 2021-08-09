using Currycomb.Common.Network;
using Serilog;
using System;
using System.Threading.Tasks;

namespace Currycomb.Gateway.Network.Services
{
    public class PlayService : IDisposable
    {
        private WrappedPacketStream PacketStream;
        public PlayService(WrappedPacketStream stream) => PacketStream = stream;

        public async Task HandleAsync(Guid id, Memory<byte> data)
        {
            await PacketStream.SendWaitAsync(new WrappedPacket(id, data));

            Log.Information($"{id} sent packet to PlayService");
        }

        public void Dispose() => PacketStream.Dispose();
    }
}
