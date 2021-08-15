using System.Threading.Tasks;
using Currycomb.Common.Network;
using System.Threading;
using System.Threading.Channels;
using Currycomb.Common.Network.Game;
using Currycomb.Common.Network.Meta;

namespace Currycomb.Gateway
{
    public interface IService
    {
        string Name { get; }

        bool Supports(GamePacketId id) => false;
        bool Supports(MetaPacketId id) => false;

        ValueTask SendAsync(bool isMeta, WrappedPacket packet) => ValueTask.CompletedTask;

        Task RunAsync(CancellationToken cancellationToken = default) => Task.CompletedTask;

        Task ReadPacketsToChannelAsync(ChannelWriter<WrappedPacketContainer> channel, CancellationToken ct = default) => Task.CompletedTask;
    }
}
