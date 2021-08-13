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

            public MetaPacketRouter<TContext> Build(bool throwOnUnknown = false)
                => new MetaPacketRouter<TContext>(throwOnUnknown, Handlers.Select(x => (x.Key, x.Value)).ToArray());
        }

        public static Builder New() => new Builder();

        public PacketFunc[]? Handlers;
        private readonly bool _throwOnUnknown;

        public MetaPacketRouter(bool throwOnUnknown, params (MetaPacketId Id, PacketFunc Func)[] handlers)
        {
            _throwOnUnknown = throwOnUnknown;
            if (handlers.Length == 0)
                return;

            var currentMetaMaxPktId = handlers.Max(x => (int)x.Id);

            Handlers = new PacketFunc[currentMetaMaxPktId + 1];

            foreach (var handler in handlers)
                Handlers[(int)handler.Id] = handler.Func;
        }

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

            Log.Debug("MetaPacket {id} is not handled by this handler.", header.PacketId);

            if (_throwOnUnknown)
                throw new Exception($"MetaPacket {header} is not handled by this handler.");
        }
    }
}
