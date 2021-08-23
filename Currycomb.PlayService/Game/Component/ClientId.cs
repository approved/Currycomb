using System;

namespace Currycomb.PlayService.Game.Component
{
    public readonly struct ClientId
    {
        public readonly Guid Value;
        public ClientId(in Guid value) { Value = value; }
    }
}
