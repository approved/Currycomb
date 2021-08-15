using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Channels;
using System.Threading.Tasks;
using Currycomb.Common.Network;
using Currycomb.Common.Network.Game;
using Currycomb.Common.Network.Meta;
using Currycomb.Common.Network.Meta.Packets;

namespace Currycomb.Gateway.Network.Services
{
    public class ServiceInstance : IService, IDisposable
    {
        public readonly Guid ServiceId;
        private PacketAnnounce _announce;

        // TODO: If these are slow, replace with a lookup array similar to how we're handling game packet routing
        private HashSet<GamePacketId> _supportedGame;
        private HashSet<MetaPacketId> _supportedMeta;
        private WrappedPacketStream _wps;

        public ServiceInstance(PacketAnnounce announce, WrappedPacketStream stream)
        {
            _wps = stream;

            _supportedMeta = announce.SupportedMetaPacketIds.ToHashSet();
            _supportedGame = announce.SupportedGamePacketIds.ToHashSet();
            _announce = announce;

            ServiceId = announce.ServiceId;
        }

        public string Name
            => _announce.Name;

        public void Dispose()
            => _wps.Dispose();

        public async ValueTask SendAsync(bool isMeta, WrappedPacket packet)
            => await _wps.SendAsync(isMeta, packet);

        public async Task ReadPacketsToChannelAsync(ChannelWriter<WrappedPacketContainer> channel, CancellationToken ct = default)
        {
            while (!ct.IsCancellationRequested)
                await channel.WriteAsync(await _wps.ReadAsync(true, ct), ct);
        }

        public Task RunAsync(CancellationToken cancellationToken = default) => _wps.RunAsync();

        public bool Supports(GamePacketId id) => _supportedGame.Contains(id);
        public bool Supports(MetaPacketId id) => _supportedMeta.Contains(id);
    }
}
