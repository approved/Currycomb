using System;

namespace Currycomb.Common.Network
{
    public record WrappedPacketContainer(Guid? AckGuid, bool IsMetaPacket, WrappedPacket Packet);
}
