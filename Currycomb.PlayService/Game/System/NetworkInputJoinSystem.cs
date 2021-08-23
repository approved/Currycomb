using System;
using System.Collections.Concurrent;
using System.IO;
using System.Text;
using Currycomb.Common.Game;
using Currycomb.Common.Network.Game.Packets;
using Currycomb.Common.Network.Game.Packets.Types;
using Currycomb.Common.Network.Game.Packets.Types.Player;
using Currycomb.PlayService.Game.Component;
using Currycomb.PlayService.Game.Message;
using DefaultEcs;
using DefaultEcs.System;
using Serilog;

namespace Currycomb.PlayService.Game.System
{
    public sealed class NetworkInputJoinSystem : ISystem<GameState>
    {
        public bool IsEnabled { get; set; }

        private readonly IDisposable _subscription;
        private readonly World _world;

        private PacketSender _pkt;

        public NetworkInputJoinSystem(World world)
        {
            _world = world;
            _pkt = world.Get<PacketSender>();

            _subscription = _world.Subscribe<ClientJoinMessage>(On);
        }

        private ConcurrentQueue<ClientJoinMessage> PlayerJoin = new();
        private void On(in ClientJoinMessage join) => PlayerJoin.Enqueue(join);

        public void Update(GameState state)
        {
            while (PlayerJoin.TryDequeue(out var message))
            {
                HandleConnectedClient(message.PlayerId);

                var entity = _world.CreateEntity();

                entity.Set<ClientId>(new(message.PlayerId));
                entity.Set<GameEntitySpawning>(new());

                Log.Information("Created entity {entity}", entity);
            }
        }

        public void HandleConnectedClient(Guid clientId)
        {
            Log.Debug("PlayerJoinSystem | {clientId}", clientId);

            _pkt.Send(clientId, new PacketJoinGame(
                entityID: 0,
                isHardcore: false,
                gameMode: GameMode.Creative,
                previousGameMode: GameMode.None,
                worldNames: new[] { "minecraft:overworld" },
                spawnWorldIdentifier: "minecraft:overworld",
                worldSeed: 1234567890123456,
                maxPlayers: 100,
                renderDistance: 10,
                reducedDebugInfo: false,
                enableRespawnScreen: true,
                isDebug: false,
                isFlat: false));

            // packet.Send(clientId, new PacketDisconnectPlay("no world loaded."));

            _pkt.Send(clientId, new PacketServerCustomPayload("minecraft:brand", Encoding.UTF8.GetBytes("currycomb")));
            _pkt.Send(clientId, new PacketChangeDifficulty(Difficulty.Easy, true));
            //packet.Send(clientId, new PacketServerPlayerAbilities(new()));
            _pkt.Send(clientId, new PacketSetHeldItem(0));
            _pkt.Send(clientId, new PacketUpdateRecipes());
            //packet.Send(clientId, new PacketUpdateTags());
            _pkt.Send(clientId, new PacketEntityEvent(0, 24));
            //packet.Send(clientId, new PacketCommandList());
            _pkt.Send(clientId, new PacketRecipe(RecipePacketState.Init, new()));
            _pkt.Send(clientId, new PacketPlayerPosition(0.0, 64.0, 0.0, 0, 0, 0x1f, 0, false));

            _pkt.Send(clientId, new PacketPlayerInfo(
                action: PlayerInfoAction.AddPlayer,
                actions: new IPlayerInfoAction[] {
                    new AddPlayerInfoAction(
                        UUID: clientId,
                        Player: "Fiskpinne",
                        Properties: Array.Empty<InfoActionProperty>(),
                        GameMode: GameMode.Survival,
                        Ping: 0)
                }));

            _pkt.Send(clientId, new PacketPlayerInfo(
                action: PlayerInfoAction.UpdateLatency,
                actions: new IPlayerInfoAction[] {
                    new PingPlayerInfoAction(
                        UUID: clientId,
                        Ping: 0)
                }));

            //packet.Send(clientId, new PacketChunkCacheCenter(0, 0));

            //packet.Send(clientId, new PacketUpdateLight(
            //    ChunkX: 0,
            //    ChunkZ: 0,
            //    TrustEdges: true,
            //    SkyLightMask: Array.Empty<long>(),
            //    BlockLightMask: Array.Empty<long>(),
            //    EmptySkyLightMask: Array.Empty<long>(),
            //    EmptyBlockLightMask: Array.Empty<long>(),
            //    SkyLight: Array.Empty<byte[]>(),
            //    BlockLight: Array.Empty<byte[]>()));

            //packet.Send(clientId, new PacketWorldChunk(
            //    ChunkX: 0,
            //    ChunkZ: 0,
            //    PrimaryBitMask: Array.Empty<long>(),
            //    Heightmaps: x => x.Compound(x => x.Write("MOTION_BLOCKING", Array.Empty<byte>())),
            //    Biomes: Array.Empty<int>(),
            //    Data: Array.Empty<byte>(),
            //    BlockEntities: Array.Empty<Action<Nbt.CompoundWriter<Nbt.Cloak>>>()));

            var biomes = new int[1024];
            biomes.AsSpan().Fill(0);

            var colBiomes = new byte[256];
            colBiomes.AsSpan().Fill(0);

            for (int x = -1; x < 2; x++)
            {
                for (int z = -1; z < 2; z++)
                {
                    using MemoryStream ms = new();
                    using BinaryWriter bw = new(ms);

                    new ChunkSection(1).Write(bw);
                    new ChunkSection(2).Write(bw);
                    new ChunkSection(3).Write(bw);
                    new ChunkSection(4).Write(bw);

                    _pkt.Send(clientId, new PacketWorldChunk(
                        chunkX: x,
                        chunkZ: z,
                        stripBitMask: new[] { 0b1111L },
                        heightmaps: x => x.Compound(x => x.Write("MOTION_BLOCKING", new long[36] { 2, 2, 2, 2, 2, 2, 2, 2, 2, 2, 2, 2, 2, 2, 2, 2, 2, 2, 2, 2, 2, 2, 2, 2, 2, 2, 2, 2, 2, 2, 2, 2, 2, 2, 2, 2 })),
                        biomes: biomes,
                        data: ms.ToArray(),
                        blockEntities: Array.Empty<Action<Nbt.CompoundWriter<Nbt.Cloak>>>()));

                    _pkt.Send(clientId, new PacketUpdateLight(
                        chunkX: x,
                        chunkZ: z,
                        trustEdges: true,
                        skyLightMask: Array.Empty<long>(),
                        blockLightMask: Array.Empty<long>(),
                        emptySkyLightMask: Array.Empty<long>(),
                        emptyBlockLightMask: Array.Empty<long>(),
                        skyLight: Array.Empty<byte[]>(),
                        blockLight: Array.Empty<byte[]>()));

                    Log.Information("Chunk sent: {0}, {1}", x, z);
                }
            }

            _pkt.Send(clientId, new PacketSetContainer(0, 0, Array.Empty<InventorySlot>(), new InventorySlot { Present = false }));
            _pkt.Send(clientId, new PacketSetTime(0, 0));

            _pkt.Send(clientId, new PacketSpawnPosition(new(10, 65, 10), 0.0f));
            _pkt.Send(clientId, new PacketPlayerPosition(0.0, 128.0, 0.0, 0, 0, 0x1f, 0, false));

            _pkt.Send(clientId, new PacketServerKeepAlive());

            _pkt.Send(clientId, new PacketBlockUpdate(new(0, 60, 0), 2));
            _pkt.Send(clientId, new PacketBlockUpdate(new(1, 60, 0), 2));
            _pkt.Send(clientId, new PacketBlockUpdate(new(1, 60, 1), 2));
            _pkt.Send(clientId, new PacketBlockUpdate(new(0, 60, 1), 2));

            // _pkt.Send(clientId, new PacketDisconnectPlay(new Chat("ree")));
        }

        public void Dispose() => _subscription.Dispose();
    }
}
