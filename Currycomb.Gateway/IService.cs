using System.Threading.Tasks;
using Currycomb.Common.Network;
using System.Threading;
using System.Threading.Channels;

namespace Currycomb.Gateway
{
    public interface IService
    {
        ValueTask HandleAsync(bool isMeta, WrappedPacket packet) => ValueTask.CompletedTask;

        Task RunAsync(CancellationToken cancellationToken = default) => Task.CompletedTask;

        Task ReadPacketsToChannelAsync(ChannelWriter<WrappedPacketContainer> channel, CancellationToken ct = default) => Task.CompletedTask;
    }
}
