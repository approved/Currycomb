using System;

namespace Currycomb.Common.Network
{
    public record WrappedPacketContainer(Guid? AckGuid, WrappedPacket Packet, bool IsMetaPacket);
}
