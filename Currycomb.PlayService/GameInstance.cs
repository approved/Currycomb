using System;
using System.Diagnostics;
using System.Text;
using System.Threading;
using System.Threading.Channels;
using System.Threading.Tasks;
using Currycomb.Common.Game;
using Currycomb.Common.Game.Entity;
using Currycomb.Common.Network;
using Currycomb.Common.Network.Game;
using Currycomb.Common.Network.Game.Packets;
using Currycomb.Common.Network.Game.Packets.Types;
using Currycomb.Common.Network.Game.Packets.Types.Player;
using fNbt;
using Microsoft.IO;
using Serilog;

namespace Currycomb.PlayService
{
    public class GameInstance
    {
        private readonly Channel<IGameEvent> _eventQueue;

        private readonly ChannelWriter<IGameEvent> _eventWriter;
        private readonly ChannelReader<IGameEvent> _eventReader;

        public readonly ChannelWriter<IGameEvent> EventWriter;

        private readonly ChannelWriter<WrappedPacket> _packetWriter;

        private readonly RecyclableMemoryStreamManager _msManager;

        public GameInstance(RecyclableMemoryStreamManager msManager, ChannelWriter<WrappedPacket> packetWriter)
        {
            _packetWriter = packetWriter;
            _msManager = msManager;

            _eventQueue = Channel.CreateUnbounded<IGameEvent>();
            _eventWriter = _eventQueue.Writer;
            _eventReader = _eventQueue.Reader;

            EventWriter = _eventQueue.Writer;
        }

        public async Task Run(CancellationToken ct)
        {
            Log.Information("GameInstance.Run: Starting");

            while (!ct.IsCancellationRequested)
            {
                while (_eventReader.TryRead(out var gameEvent))
                {
                    Log.Information("Event: {event}", gameEvent);
                    switch (gameEvent)
                    {
                        case EvtPlayerConnected pc:
                            Log.Information("GameInstance.Run: Player connected");
                            await OnPlayerConnected(pc.ClientId);
                            break;
                    }
                }

                Tick();
                Thread.Sleep(1000);
            }

            Log.Information("GameInstance.Run: Cancelled");
        }

        private void Tick()
        {
            // Do something.
            Log.Information("Tick! {@now}", DateTimeOffset.UtcNow);
        }

        private async Task OnPlayerConnected(Guid clientId)
        {
            // Do something.
            Log.Information("GameInstance.OnPlayerConnected: {@clientId}", clientId);

            SendPacket(clientId, new PacketJoinGame(
                EntityID: 45,
                IsHardcore: false,
                GameMode: GameMode.Creative,
                PreviousGameMode: GameMode.None,
                WorldNames: new[] { "world" },
                SpawnWorldIdentifier: "world",
                WorldSeed: 0,
                RenderDistance: 32,
                ReducedDebugInfo: false,
                EnableRespawnScreen: false,
                IsDebug: false,
                IsFlat: false));

            SendPacket(clientId, new PacketServerCustomPayload("minecraft:brand", Encoding.UTF8.GetBytes("currycomb")));
            SendPacket(clientId, new PacketChangeDifficulty(Difficulty.Easy, true));
            SendPacket(clientId, new PacketServerPlayerAbilities(new()));
            SendPacket(clientId, new PacketSetHeldItem(0));
            SendPacket(clientId, new PacketUpdateRecipes());
            SendPacket(clientId, new PacketUpdateTags());
            SendPacket(clientId, new PacketEntityEvent(45, 24));
            SendPacket(clientId, new PacketCommandList());
            SendPacket(clientId, new PacketRecipe(RecipePacketState.Init, new()));
            SendPacket(clientId, new PacketPlayerPosition(0.0, 64.0, 0.0, 0, 0, 0x1f, 0, false));
            
            SendPacket(clientId, new PacketPlayerInfo(
                Action: PlayerInfoAction.AddPlayer, 
                Actions: new IPlayerInfoAction[] { 
                    new AddPlayerInfoAction(
                        UUID: clientId, 
                        Player: "TestAccount123", 
                        Properties: Array.Empty<InfoActionProperty>(), 
                        GameMode: GameMode.Survival, 
                        Ping: 0) 
                }));

            SendPacket(clientId, new PacketPlayerInfo(
                Action: PlayerInfoAction.UpdateLatency,
                Actions: new IPlayerInfoAction[] {
                    new PingPlayerInfoAction(
                        UUID: clientId,
                        Ping: 0)
                }));

            SendPacket(clientId, new PacketChunkCacheCenter(0, 0));

            SendPacket(clientId, new PacketUpdateLight(
                ChunkX: 0,
                ChunkZ: 0,
                TrustEdges: true,
                SkyLightMask: Array.Empty<long>(),
                BlockLightMask: Array.Empty<long>(),
                EmptySkyLightMask: Array.Empty<long>(),
                EmptyBlockLightMask: Array.Empty<long>(),
                SkyLight: Array.Empty<byte[]>(),
                BlockLight: Array.Empty<byte[]>()));

            SendPacket(clientId, new PacketWorldChunk(
                ChunkX: 0,
                ChunkZ: 0,
                PrimaryBitMask: Array.Empty<long>(),
                Heightmaps: new NbtCompound(string.Empty, new[] { new NbtByteArray("MOTION_BLOCKING") }),
                Biomes: Array.Empty<int>(),
                Data: Array.Empty<byte>(),
                BlockEntities: Array.Empty<NbtCompound>()));

            // ChunkDataBulk

            SendPacket(clientId, new PacketSetContainer(0, 0, Array.Empty<InventorySlot>(), new InventorySlot { Present = false }));
            SendPacket(clientId, new PacketSetTime(0, 0));

            SendPacket(clientId, new PacketSpawnPosition(new(10, 65, 10), 0.0f));
            SendPacket(clientId, new PacketPlayerPosition(0.0, 64.0, 0.0, 0, 0, 0x1f, 0, false));

            //SendPacket(clientId, new PacketDisconnect("Should be in-game"));
        }

        private bool SendPacket<T>(Guid clientId, T packet) where T : IGamePacket
            => _packetWriter.TryWrite(new WrappedPacket(clientId, packet.ToBytes(_msManager.GetStream("IGamePacket"))));
    }

    public interface IGameEvent { }
    public record EvtPlayerConnected(Guid ClientId) : IGameEvent;
}
