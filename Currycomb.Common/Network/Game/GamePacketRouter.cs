using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Serilog;

namespace Currycomb.Common.Network.Game
{
    public interface IGamePacketRouterContext
    {
        public State State { get; }
    }

    public struct GamePacketRouter<TContext> where TContext : IGamePacketRouterContext
    {
        public readonly IList<GamePacketId> Packets;

        private readonly GamePacketFunc[][] Handlers;

        public GamePacketRouter(params (GamePacketId Id, GamePacketFunc Func)[] handlers)
        {
            Packets = handlers.Select(x => x.Id).ToList().AsReadOnly();

            // 3 meta bits
            Handlers = new GamePacketFunc[0b111][];

            for (int i = 0; i < 7; i++)
            {
                var currentMetaHandlers = handlers.Where(x => x.Id.ToMetaBits() == i).ToArray();
                if (currentMetaHandlers.Length == 0)
                    continue;

                var currentMetaMaxPktId = currentMetaHandlers.Max(x => x.Id.ToRaw());

                Handlers[i] = new GamePacketFunc[currentMetaMaxPktId + 1];

                foreach (var handler in currentMetaHandlers)
                    Handlers[i][handler.Id.ToRaw()] = handler.Func;
            }
        }
        public delegate Task GamePacketFunc(TContext context, IGamePacket packet);

        public static Builder New() => new Builder();

        public async Task HandlePacketAsync(TContext context, Stream stream, uint length)
        {
            GamePacketHeader header = await GamePacketHeader.ReadAsync(stream, length);
            Log.Information("Read packet header: {@header}", header);

            GamePacketId id = GamePacketIdExt.FromRaw(BoundTo.Server, context.State, header.PacketId);
            Log.Information("Read packet id: {@id}", id);

            IGamePacket packet = await GamePacketReader.ReadAsync(id, stream);
            Log.Information("Read packet: {@packet}", packet);

            await HandlePacketAsync(context, id, packet);
        }

        public async Task HandlePacketAsync(TContext context, GamePacketId id, IGamePacket packet)
        {
            // TODO: Check if try-catch is faster than bounds checking before access
            int metaBits = id.ToMetaBits();
            int pktId = (int)id.ToRaw();

            // TODO: Actual exception types
            if (Handlers.Length <= metaBits)
                throw new Exception($"Packet {id} is not handled by this handler.");

            GamePacketFunc[] bitHandlers = Handlers[metaBits];
            if (bitHandlers == null)
                throw new Exception($"Packet {id} is not handled by this handler.");

            if (bitHandlers.Length <= pktId)
                throw new Exception($"Packet {id} is not handled by this handler.");

            GamePacketFunc handler = bitHandlers[pktId];
            if (handler == null)
                throw new Exception($"Packet {id} is not handled by this handler.");

            await handler(context, packet);
        }

        public class Builder
        {
            private readonly Dictionary<GamePacketId, GamePacketFunc> Handlers = new();

            public GamePacketRouter<TContext> Build() => new GamePacketRouter<TContext>(Handlers.Select(x => (x.Key, x.Value)).ToArray());

            public Builder On<T>(Func<TContext, T, Task> handler) where T : IGamePacket
            {
                Handlers.Add(GamePacketIdMap<T>.Id, (x, y) => handler(x, (T)y));
                return this;
            }

            public Builder On<T>(Action<TContext, T> handler) where T : IGamePacket
            {
                Handlers.Add(GamePacketIdMap<T>.Id, (x, y) =>
                {
                    handler(x, (T)y);
                    return Task.CompletedTask;
                });

                return this;
            }
        }
    }
}
