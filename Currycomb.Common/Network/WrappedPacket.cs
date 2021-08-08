using System;
using System.IO;
using System.Threading.Tasks;
using Currycomb.Common.Extensions;

namespace Currycomb.Common.Network
{
    public record WrappedPacket(Guid ClientId, ReadOnlyMemory<byte> Packet)
    {
        public static WrappedPacket Read(BinaryReader reader) => new(
            new Guid(reader.ReadBytes(16)),
            reader.ReadBytes(reader.Read7BitEncodedInt())
        );


        public static async Task<WrappedPacket> ReadAsync(Stream reader) => new(
            new Guid(await reader.ReadBytesAsync(16)),
            await reader.ReadBytesAsync(reader.Read7BitEncodedInt())
        );

        public async Task WriteAsync(Stream stream)
        {
            await stream.WriteAsync(ClientId.ToByteArray());
            await stream.Write7BitEncodedIntAsync(Packet.Length);
            await stream.WriteAsync(Packet);
        }
    }
}
