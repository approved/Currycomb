using System;
using Currycomb.Common.Network.Minecraft;

namespace Currycomb.Common.Network.Broadcast
{
    public record PayloadStateChange(Guid Client, State State);
}
