using System;
using System.IO;
using System.Threading.Tasks;
using Currycomb.Common.Extensions;

namespace Currycomb.Common.Network.Minecraft
{
    public interface IPacket { }

    public static class PacketReader
    {
        public static Task<IPacket> ReadAsync(PacketId id, Stream stream) => id switch
        {
            PacketId.Handshake => PacketHandshake.ReadAsync(stream).AsTask<PacketHandshake, IPacket>(),
            PacketId.LoginStart => PacketLoginStart.ReadAsync(stream).AsTask<PacketLoginStart, IPacket>(),
            PacketId.Request => PacketRequest.ReadAsync(stream).AsTask<PacketRequest, IPacket>(),
            _ => throw new NotSupportedException($"Packet not yet supported: {id}"),
        };
    }

    public record PacketHandshake(uint ProtocolVersion, String ServerAddress, ushort Port, State State) : IPacket
    {
        public static async Task<PacketHandshake> ReadAsync(Stream stream) => new(
            await stream.Read7BitEncodedUIntAsync(),
            await stream.ReadStringAsync(),
            await stream.ReadUShortAsync(),
            StateExt.FromRaw(await stream.Read7BitEncodedUIntAsync())
        );
    }

    public record PacketLoginStart(String Username) : IPacket
    {
        public static async Task<PacketLoginStart> ReadAsync(Stream stream) => new(
            await stream.ReadStringAsync()
        );
    }

    public record PacketLoginSuccess(Guid Uuid, String Username) : IPacket
    {
        public async Task WriteAsync(Stream stream)
        {
            await stream.WriteAsync(Uuid);
            await stream.WriteStringAsync(Username);
        }
    }

    public record PacketRequest() : IPacket
    {
        public static Task<PacketRequest> ReadAsync(Stream stream) => Task.FromResult<PacketRequest>(new());
    }

    public record PacketResponse(String Json) : IPacket
    {
        public Task WriteAsync(Stream stream)
            => stream.WriteStringAsync(Json);
    }
}
