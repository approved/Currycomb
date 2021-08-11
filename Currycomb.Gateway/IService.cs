using System.Threading.Tasks;
using Currycomb.Common.Network;
using System.Collections.Generic;
using Currycomb.Common.Network.Broadcast;
using System.Threading;
using System.Linq;

namespace Currycomb.Gateway
{
    public interface IService
    {
        ValueTask HandleAsync(WrappedPacket packet) => ValueTask.CompletedTask;
        ValueTask HandleAsync(ComEvent packet) => ValueTask.CompletedTask;

        Task RunAsync(CancellationToken cancellationToken = default) => Task.CompletedTask;

        IAsyncEnumerable<WrappedPacketContainer> ReadPacketsAsync(CancellationToken ct = default) => AsyncEnumerable.Empty<WrappedPacketContainer>();
    }
}
