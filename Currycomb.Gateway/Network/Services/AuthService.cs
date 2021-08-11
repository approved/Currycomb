using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Threading;
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
        public Task RunAsync(CancellationToken cancellationToken = default) => ServiceStream.RunAsync();

        public async ValueTask HandleAsync(WrappedPacket packet)
        {
            Guid id = packet.ClientId;
            Log.Warning($"{id} attempting to complete handshake");

            await ServiceStream.SendWaitAsync(packet, false);
            Log.Information($"{id} sent packet to AuthService");
        }

        public async IAsyncEnumerable<WrappedPacketContainer> ReadPacketsAsync([EnumeratorCancellation] CancellationToken ct = default)
        {
            while (!ct.IsCancellationRequested)
            {
                yield return await ServiceStream.ReadAsync(true, ct);
            }
        }
    }
}
