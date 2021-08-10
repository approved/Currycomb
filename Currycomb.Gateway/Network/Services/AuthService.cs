using Currycomb.Common.Network;
using Serilog;
using System;
using System.Threading.Tasks;

namespace Currycomb.Gateway.Network.Services
{
    public class AuthService : IDisposable
    {
        private WrappedPacketStream PacketStream;
        public AuthService(WrappedPacketStream stream) => PacketStream = stream;

        public async Task HandleAsync(Guid id, Memory<byte> data)
        {
            Log.Warning($"{id} attempting to complete handshake");

            await PacketStream.SendWaitAsync(new WrappedPacket(id, data), false);
            Log.Information($"{id} sent packet to AuthService");
        }

        public void Dispose() => PacketStream.Dispose();
    }
}
