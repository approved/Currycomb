using System;

namespace Currycomb.PlayService.ExternalEvent
{
    public readonly struct ClientConnected : IMetaEvent
    {
        public readonly Guid Id;
        public ClientConnected(Guid id) => Id = id;
    }
}