using System.Threading.Tasks;
using Currycomb.Common.Network;
using System.Collections.Generic;
using Currycomb.Common.Network.Broadcast;
using System.Threading;
using System.Linq;
using System.Threading.Channels;

namespace Currycomb.Gateway
{
    public interface IService
    {
        ValueTask HandleAsync(WrappedPacket packet) => ValueTask.CompletedTask;
        ValueTask HandleAsync(ComEvent packet) => ValueTask.CompletedTask;

        Task RunAsync(CancellationToken cancellationToken = default) => Task.CompletedTask;

        Task ReadPacketsToChannelAsync(ChannelWriter<WrappedPacketContainer> channel, CancellationToken ct = default) => Task.CompletedTask;
    }
}
