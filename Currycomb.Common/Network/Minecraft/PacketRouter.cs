using System;
using System.Threading.Tasks;
using Currycomb.Common.Network.Minecraft;
using System.Linq;
using System.Collections.Generic;
using System.IO;
using Serilog;

namespace Currycomb.Common.Network.Minecraft
{

    public class PacketRouter
    {
        public delegate Task PacketFunc(Guid clientId, IPacket packet);

        public class PacketRouterBuilder
        {
            private readonly Dictionary<PacketId, PacketFunc> Handlers = new();

            public PacketRouterBuilder On<T>(Func<Guid, T, Task> handler) where T : IPacket
            {
                Handlers.Add(PacketIdMap.FromType<T>(), (x, y) => handler(x, (T)y));
                return this;
            }

            public PacketRouterBuilder On<T>(Action<Guid, T> handler) where T : IPacket
            {
                Handlers.Add(PacketIdMap.FromType<T>(), (x, y) =>
                {
                    handler(x, (T)y);
                    return Task.CompletedTask;
                });

                return this;
            }

            public PacketRouter Build() => new PacketRouter(Handlers.Select(x => (x.Key, x.Value)).ToArray());
        }

        public static PacketRouterBuilder New() => new PacketRouterBuilder();

        public PacketFunc[][] Handlers;

        public PacketRouter(params (PacketId Id, PacketFunc Func)[] handlers)
        {
            // 3 meta bits
            Handlers = new PacketFunc[0b111][];

            for (int i = 0; i < 7; i++)
            {
                var currentMetaHandlers = handlers.Where(x => x.Id.ToMetaBits() == i).ToArray();
                if (currentMetaHandlers.Length == 0)
                    continue;

                var currentMetaMaxPktId = currentMetaHandlers.Max(x => x.Id.ToRaw());

                Handlers[i] = new PacketFunc[currentMetaMaxPktId + 1];

                foreach (var handler in currentMetaHandlers)
                    Handlers[i][handler.Id.ToRaw()] = handler.Func;
            }
        }

        public async Task HandlePacketAsync(State state, WrappedPacket wrapped)
        {
            using MemoryStream memoryStream = new(wrapped.GetOrCreatePacketByteArray(), false);
            await HandlePacketAsync(wrapped.ClientId, state, memoryStream, (uint)wrapped.Data.Length);
        }

        public async Task HandlePacketAsync(Guid client, State state, Stream stream, uint length)
        {
            PacketHeader header = await PacketHeader.ReadAsync(stream, length);
            Log.Information($"Read packet header: {header}");

            PacketId id = PacketIdExt.FromRaw(BoundTo.Server, state, header.PacketId);
            Log.Information($"Read packet id: {id}");

            IPacket packet = await PacketReader.ReadAsync(id, stream);
            Log.Information($"Read packet: {packet}");

            await HandlePacketAsync(client, id, packet);
        }

        public async Task HandlePacketAsync(Guid client, PacketId id, IPacket packet)
        {
            // TODO: Check if try-catch is faster than bounds checking before access
            int metaBits = id.ToMetaBits();
            int pktId = (int)id.ToRaw();

            // TODO: Actual exception types
            if (Handlers.Length <= metaBits)
                throw new Exception($"Packet {id} is not handled by this handler.");

            if (Handlers[metaBits] == null)
                throw new Exception($"Packet {id} is not handled by this handler.");

            if (Handlers[metaBits].Length <= pktId)
                throw new Exception($"Packet {id} is not handled by this handler.");

            if (Handlers[metaBits][pktId] == null)
                throw new Exception($"Packet {id} is not handled by this handler.");

            await Handlers[metaBits][pktId](client, packet);
        }
    }
}
