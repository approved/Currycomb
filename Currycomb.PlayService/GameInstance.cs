using System;
using System.Threading;
using System.Threading.Channels;
using System.Threading.Tasks;
using Serilog;

namespace Currycomb.PlayService
{
    public class GameInstance
    {
        private Channel<IGameEvent> _eventQueue;

        private ChannelWriter<IGameEvent> _eventWriter;
        private ChannelReader<IGameEvent> _eventReader;

        public ChannelWriter<IGameEvent> EventWriter;

        public GameInstance()
        {
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
        }
    }

    public interface IGameEvent { }
    public record EvtPlayerConnected(Guid ClientId) : IGameEvent;
}
