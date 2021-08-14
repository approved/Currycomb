using System;
using System.IO;
using System.Threading.Tasks;
using Currycomb.Common.Extensions;
using Currycomb.Common.Network.Game.Packets;

namespace Currycomb.Common.Network.Game
{
    public static class GamePacketReader
    {
        public static Task<IGamePacket> ReadAsync(GamePacketId id, Stream stream) => id switch
        {
            GamePacketId.Handshake => PacketHandshake.ReadAsync(stream).AsTask<PacketHandshake, IGamePacket>(),
            GamePacketId.LoginStart => PacketLoginStart.ReadAsync(stream).AsTask<PacketLoginStart, IGamePacket>(),
            GamePacketId.Request => PacketRequest.ReadAsync(stream).AsTask<PacketRequest, IGamePacket>(),
            GamePacketId.Ping => PacketPing.ReadAsync(stream).AsTask<PacketPing, IGamePacket>(),
            GamePacketId.EncryptionResponse => PacketEncryptionResponse.ReadAsync(stream).AsTask<PacketEncryptionResponse, IGamePacket>(),
            GamePacketId.ClientKeepAlive => PacketClientKeepAlive.ReadAsync(stream).AsTask<PacketClientKeepAlive, IGamePacket>(),
            _ => throw new NotSupportedException($"Packet not yet supported: {id} ({id.ToRaw()} / 0x{id.ToRaw():X} | BoundTo.{id.BoundTo()} | State.{id.State()})"),
        };
    }
}
