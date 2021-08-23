using System;

namespace Currycomb.PlayService.Game.Message
{
    public readonly struct ClientJoinMessage
    {
        public readonly Guid PlayerId;

        public ClientJoinMessage(Guid playerId) => PlayerId = playerId;
    }
}