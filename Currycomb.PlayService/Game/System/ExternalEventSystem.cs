using System.Threading.Channels;
using Currycomb.PlayService.ExternalEvent;
using Currycomb.PlayService.Game.Message;
using DefaultEcs;
using DefaultEcs.System;
using Serilog;

namespace Currycomb.PlayService.Game.System
{
    [With(typeof(GameState))]
    public sealed class ExternalEventSystem : ISystem<GameState>
    {
        public bool IsEnabled { get; set; }

        private readonly World _world;
        private readonly ChannelReader<IMetaEvent> _externalEvents;

        public ExternalEventSystem(World world, ChannelReader<IMetaEvent> externalEvents)
        {
            _world = world;
            _externalEvents = externalEvents;
        }

        public void Update(GameState state)
        {
            while (_externalEvents.TryRead(out var ev))
            {
                Log.Information("ExternalEventSystem: {@ev}", ev);
                switch (ev)
                {
                    case ClientConnected e:
                        _world.Publish<ClientJoinMessage>(new(e.Id));
                        break;
                }
            }
        }

        public void Dispose() { }
    }
}
