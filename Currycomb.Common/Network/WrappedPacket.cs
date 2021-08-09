using System;
using System.IO;
using System.Threading.Tasks;
using Currycomb.Common.Extensions;

namespace Currycomb.Common.Network
{
    public record WrappedPacket(Guid ClientId, ReadOnlyMemory<byte> Data)
    {
        private byte[]? _bytes;

        public WrappedPacket(Guid clientId, byte[] bytes) : this(clientId, new ReadOnlyMemory<byte>(bytes))
            => _bytes = bytes;

        public static WrappedPacket Read(BinaryReader reader) => new(
            new Guid(reader.ReadBytes(16)),
            reader.ReadBytes(reader.Read7BitEncodedInt())
        );

        public static async Task<WrappedPacket> ReadAsync(Stream reader) => new(
            new Guid(await reader.ReadBytesAsync(16)),
            await reader.ReadBytesAsync(reader.Read7BitEncodedInt())
        );

        public byte[] GetOrCreatePacketByteArray() => _bytes ??= Data.ToArray();

        public void WriteTo(BinaryWriter stream)
        {
            stream.Write(ClientId.ToByteArray());
            stream.Write7BitEncodedInt(Data.Length);
            stream.Write(Data.Span);
        }
    }
}
