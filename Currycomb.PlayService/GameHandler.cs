using System;
using System.Diagnostics;
using System.Threading;
using System.Threading.Channels;
using Currycomb.Common.Network;
using Currycomb.Common.Network.Game;
using Currycomb.PlayService.Game;
using Microsoft.IO;
using Serilog;

namespace Currycomb.PlayService
{
    public interface IMetaEvent { }
    public class GameHandler
    {
        public readonly ChannelWriter<IMetaEvent> EventWriter;

        private readonly Channel<IMetaEvent> _eventQueue;
        private readonly ChannelReader<IMetaEvent> _eventReader;
        private readonly ChannelWriter<IMetaEvent> _eventWriter;

        private readonly RecyclableMemoryStreamManager _msManager;

        private readonly ChannelWriter<WrappedPacket> _packetWriter;

        private readonly GameInstance _game;

        public GameHandler(RecyclableMemoryStreamManager msManager, ChannelWriter<WrappedPacket> packetWriter)
        {
            _packetWriter = packetWriter;
            _msManager = msManager;

            _eventQueue = Channel.CreateUnbounded<IMetaEvent>();
            _eventWriter = _eventQueue.Writer;
            _eventReader = _eventQueue.Reader;

            EventWriter = _eventQueue.Writer;

            _game = new GameInstance(_eventReader, _packetWriter);
        }

        public void HandleGamePacket<T>(Context ctx, T packet) where T : IGamePacket
            => _game.HandleGamePacket<T>(new(new(ctx.ClientId), packet));

        public void Run(CancellationToken ct)
        {
            Log.Information("GameInstance.Run: Starting");

            Stopwatch sw = Stopwatch.StartNew();

            double lastTick = 0.0;
            while (!ct.IsCancellationRequested)
            {
                double time = sw.Elapsed.TotalSeconds;
                float delta = (float)(time - lastTick);
                lastTick = time;

                _game.Update(delta);

                // TODO: Better.
                Thread.Sleep(250);
            }

            Log.Information("GameInstance.Run: Cancelled");
        }
    }
}
