using System;
using System.IO;
using System.Threading.Tasks;
using Currycomb.Common.Extensions;
using Currycomb.Common.Network.Minecraft.Packets;

namespace Currycomb.Common.Network.Minecraft
{

    public static class PacketReader
    {
        public static Task<IPacket> ReadAsync(PacketId id, Stream stream) => id switch
        {
            PacketId.Handshake => PacketHandshake.ReadAsync(stream).AsTask<PacketHandshake, IPacket>(),
            PacketId.LoginStart => PacketLoginStart.ReadAsync(stream).AsTask<PacketLoginStart, IPacket>(),
            PacketId.Request => PacketRequest.ReadAsync(stream).AsTask<PacketRequest, IPacket>(),
            PacketId.Ping => PacketPing.ReadAsync(stream).AsTask<PacketPing, IPacket>(),
            _ => throw new NotSupportedException($"Packet not yet supported: {id}"),
        };
    }
}
