using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Serilog;

namespace Currycomb.Common.Network.Meta
{
    public interface IMetaPacketRouterContext { }

    public class MetaPacketRouter<TContext> where TContext : IMetaPacketRouterContext
    {
        public readonly IList<MetaPacketId> Packets;

        private readonly PacketFunc[]? Handlers;

        public MetaPacketRouter(params (MetaPacketId Id, PacketFunc Func)[] handlers)
        {
            Packets = handlers.Select(x => x.Id).ToList().AsReadOnly();
            if (handlers.Length == 0)
                return;

            var currentMetaMaxPktId = handlers.Max(x => (int)x.Id);

            Handlers = new PacketFunc[currentMetaMaxPktId + 1];

            foreach (var handler in handlers)
                Handlers[(int)handler.Id] = handler.Func;
        }
        public delegate Task PacketFunc(TContext context, IMetaPacket packet);

        public static Builder New() => new Builder();

        public Task HandlePacketAsync(TContext context, BinaryReader reader)
            => HandlePacketAsync(context, MetaPacket.Read(reader));

        public async Task HandlePacketAsync(TContext context, MetaPacket<IMetaPacket> meta)
        {
            Log.Information("Handling MetaPacket {@meta}", meta);

            MetaPacketHeader header = meta.Header;
            IMetaPacket packet = meta.Packet;

            // TODO: Check if try-catch is faster than bounds checking before access
            int pktId = (int)header.PacketId;

            if (Handlers != null && Handlers.Length > pktId && Handlers[pktId] is PacketFunc handler)
            {
                await handler(context, packet);
                return;
            }

            throw new Exception($"MetaPacket {header} is not handled by this handler.");
        }

        public class Builder
        {
            private readonly Dictionary<MetaPacketId, PacketFunc> Handlers = new();

            public MetaPacketRouter<TContext> Build()
                => new MetaPacketRouter<TContext>(Handlers.Select(x => (x.Key, x.Value)).ToArray());

            public Builder On<T>(Func<TContext, T, Task> handler) where T : IMetaPacket
            {
                Handlers.Add(MetaPacketIdMap<T>.Id, (x, y) => handler(x, (T)y));
                return this;
            }

            public Builder On<T>(Action<TContext, T> handler) where T : IMetaPacket
            {
                Handlers.Add(MetaPacketIdMap<T>.Id, (x, y) =>
                {
                    handler(x, (T)y);
                    return Task.CompletedTask;
                });

                return this;
            }
        }
    }
}
