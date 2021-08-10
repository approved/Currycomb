using System;
using System.Linq;
using System.Collections.Generic;
using System.IO;
using Serilog;
using System.Threading.Tasks;

namespace Currycomb.Common.Network.Meta
{
    public interface IMetaPacketRouterContext { }

    public class MetaPacketRouter<TContext> where TContext : IMetaPacketRouterContext
    {
        public delegate Task PacketFunc(TContext context, IMetaPacket packet);

        public class Builder
        {
            private readonly Dictionary<MetaPacketId, PacketFunc> Handlers = new();

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

            public MetaPacketRouter<TContext> Build() => new MetaPacketRouter<TContext>(Handlers.Select(x => (x.Key, x.Value)).ToArray());
        }

        public static Builder New() => new Builder();

        public PacketFunc[]? Handlers;

        public MetaPacketRouter(params (MetaPacketId Id, PacketFunc Func)[] handlers)
        {
            if (handlers.Length == 0)
                return;

            var currentMetaMaxPktId = handlers.Max(x => x.Id.ToRaw());

            Handlers = new PacketFunc[currentMetaMaxPktId + 1];

            foreach (var handler in handlers)
                Handlers[handler.Id.ToRaw()] = handler.Func;
        }

        public async Task HandlePacketAsync(TContext context, BinaryReader reader)
        {
            MetaPacketHeader header = MetaPacketHeader.Read(reader);
            Log.Information($"Read packet header: {header}");

            MetaPacketId id = MetaPacketIdExt.FromRaw(header.PacketId);
            Log.Information($"Read packet id: {id}");

            IMetaPacket packet = MetaPacketReader.Read(id, reader);
            Log.Information($"Read packet: {packet}");

            await HandlePacketAsync(context, id, packet);
        }

        public async Task HandlePacketAsync(TContext context, MetaPacketId id, IMetaPacket packet)
        {
            // TODO: Check if try-catch is faster than bounds checking before access
            int pktId = (int)id.ToRaw();

            if (Handlers == null)
                throw new Exception($"Packet {id} is not handled by this handler.");

            if (Handlers.Length <= pktId)
                throw new Exception($"Packet {id} is not handled by this handler.");

            if (Handlers[pktId] == null)
                throw new Exception($"Packet {id} is not handled by this handler.");

            await Handlers[pktId](context, packet);
        }
    }
}
