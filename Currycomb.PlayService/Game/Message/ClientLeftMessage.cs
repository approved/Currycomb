using System;

namespace Currycomb.PlayService.Game.Message
{
    public readonly struct ClientLeftMessage
    {
        public readonly Guid PlayerId;

        public ClientLeftMessage(Guid playerId) => PlayerId = playerId;
    }
}