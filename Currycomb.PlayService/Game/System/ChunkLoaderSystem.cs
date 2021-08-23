using System;
using System.Security.Cryptography;
using Currycomb.PlayService.Game.Component;
using Currycomb.PlayService.Game.Message;
using DefaultEcs;
using DefaultEcs.System;

namespace Currycomb.PlayService.Game.System
{
    public sealed partial class ChunkLoaderSystem : ISystem<GameState>
    {
        // private ChunkManager _chunkManager;
        private IDisposable _subscription;

        public bool IsEnabled { get; set; }

        public ChunkLoaderSystem(World world)
        {
            // _chunkManager = world.Get<ChunkManager>();
            _subscription = world.Subscribe<ChunkLoadRequestMessage>(On);
        }

        public void Dispose() { }

        public void Update(GameState state) { }

        public void On(in ChunkLoadRequestMessage message)
        {
            // var chunk = _chunkManager.GetChunk(message.ChunkX, message.ChunkZ);
        }
    }
}
