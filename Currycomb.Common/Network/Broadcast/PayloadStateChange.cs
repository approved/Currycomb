using System;
using Currycomb.Common.Network.Game;

namespace Currycomb.Common.Network.Broadcast
{
    public record PayloadStateChange(Guid Client, State State);
}
