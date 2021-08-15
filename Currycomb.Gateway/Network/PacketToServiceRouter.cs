using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Currycomb.Common.Extensions;
using Currycomb.Common.Network;
using Currycomb.Common.Network.Game;
using Currycomb.Common.Network.Meta;
using Serilog;

namespace Currycomb.Gateway.Routers
{
    public class PacketToServiceRouter
    {
        private readonly ServiceCollection _services;

        public PacketToServiceRouter(ServiceCollection services)
            => _services = services;

        public async ValueTask HandleGameAsync(BoundTo boundTo, State state, WrappedPacket packet)
        {
            // GetOrCreatePacketByteArray should always return an existing array since all packets reaching this point are from an external source.
            byte[] data = packet.GetOrCreatePacketByteArray();

            GamePacketHeader header = GamePacketHeader.Read(new(new MemoryStream(data)), (uint)data.Length);
            GamePacketId packetId = GamePacketIdExt.FromRaw(boundTo, state, header.PacketId);

            Log.Information("Reading game packet: {pktId}, {header}", packetId, header);

            var services = _services.Services
                .Where(x => x.Supports(packetId))
                .Inspect(x => Log.Information("Using handler: {name}", x.Name))
                .Select(x => x.SendAsync(false, packet));

            foreach (var task in services)
                await task;
        }

        public async ValueTask HandleMetaAsync(WrappedPacket packet)
        {
            // GetOrCreatePacketByteArray should always return an existing array since all packets reaching this point are from an external source.
            MetaPacketHeader header = MetaPacketHeader.Read(new(new MemoryStream(packet.GetOrCreatePacketByteArray())));
            MetaPacketId packetId = header.PacketId;

            var services = _services.Services
                .Where(s => s.Supports(packetId))
                .Select(x => x.SendAsync(true, packet));

            foreach (var task in services)
                await task;
        }
    }
}
