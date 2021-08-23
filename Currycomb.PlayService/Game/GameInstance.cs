using System;
using System.IO;
using System.Threading.Channels;
using Currycomb.Common.Network;
using Currycomb.Common.Network.Game;
using Currycomb.PlayService.Game.Message;
using Currycomb.PlayService.Game.System;
using DefaultEcs;
using DefaultEcs.System;
using DefaultEcs.Threading;
using Serilog;

namespace Currycomb.PlayService.Game
{
    public readonly struct ChunkMeta
    {
        public readonly int X;
        public readonly int Z;
        public readonly short Mask;

        public ChunkMeta(int x, int z, short mask)
        {
            X = x;
            Z = z;
            Mask = mask;
        }

        public void Write(BinaryWriter writer)
        {
            writer.Write(X);
            writer.Write(Z);
            writer.Write(true); // "Full chunk"
            writer.Write7BitEncodedInt(Mask);
        }
    }

    /// Chunk is a group of 16x16x16 blocks.
    ///
    /// `block_light`, `sky_light` are nibble arrays (4bit values)
    public struct ChunkSection
    {
        public short BlockCount; // Number of non-air blocks
        public short[] Blocks; // 4096 = 16 * 16 * 16

        public ChunkSection(short block)
        {
            BlockCount = 16 * 16 * 16;
            Blocks = new short[16 * 16 * 16];
            Blocks.AsSpan().Fill(block);
        }

        public ChunkSection(short blockCount, short[] blocks)
        {
            if (blocks.Length != 4096)
                throw new ArgumentException($"Invalid length: {blocks.Length}", nameof(blocks));

            BlockCount = blockCount;
            Blocks = blocks;
        }

        public void Write(BinaryWriter writer)
        {
            writer.Write(BlockCount);
            writer.Write7BitEncodedInt(4); // Bits per block

            writer.Write7BitEncodedInt(16); // Palette length
            for (int i = 0; i < 16; i++)
                writer.Write7BitEncodedInt(i); // Palette[i] = i (block id)

            writer.Write7BitEncodedInt(256); // Data Array Length
            for (int i = 0; i < 256; i++)
                writer.Write(0b0001_0010_0011_0100_0101_0110_0111_1000_1001_1010_1011_1100_1101_1110_1111);
        }
    }

    class ChunkManager
    {
        public ChunkSection GetChunk(int x, int y)
        {
            var chunk = new ChunkSection();

            return chunk;
        }
    }

    class GameInstance
    {
        readonly DefaultParallelRunner _runner;
        readonly World _world;
        readonly ISystem<GameState> _system;

        public GameInstance(ChannelReader<IMetaEvent> events, ChannelWriter<WrappedPacket> packetWriter)
        {
            _runner = new(Environment.ProcessorCount);

            _world = new();
            _world.Set<PacketSender>(new(packetWriter));
            _world.Set<SafeRandom>(new());
            _world.Set<ChunkManager>(new());

            _system = new SequentialSystem<GameState>(
                // Network input systems
                new ExternalEventSystem(_world, events),
                new NetworkInputJoinSystem(_world),
                new NetworkInputMoveSystem(_world, _runner),

                // General systems
                new EntitySpawnSystem(_world),
                new MovementSystem(_world),

                // Reactive systems
                new ChunkLoaderSystem(_world),
                new EntityMovedSystem(_world),

                // Network output systems
                new NetworkOutputEntitySpawnedSystem(_world));
        }

        public void HandleGamePacket<T>(in Pkt<T> packet) where T : IGamePacket
        {
            // THIS IS NOT THREAD SAFE, MEANING THAT ALL SUBSCRIPTIONS TO PACKETS NEED TO DEFER THEM IF THEY MODIFY ANYTHING
            Log.Information("Publishing packet: {pkt}", packet);
            _world.Publish(in packet);
        }

        public void Update(float delta)
        {
            Log.Information("GameInstance.Update: {delta}", delta);
            _system.Update(new(delta));
        }
    }
}
