using System;
using System.IO;
using System.Threading.Tasks;
using Currycomb.Common.Extensions;
using Serilog;

namespace Currycomb.Common.Network.Minecraft
{
    public static class PacketExt
    {
        public static async Task<Memory<byte>> ToBytes<T>(this T packet, PacketId id) where T : IPacket
        {
            using MemoryStream ms = new();

            // Placeholder space for packet Length
            ms.Write7BitEncodedUInt(uint.MaxValue);

            var packetStart = (uint)ms.Position;
            Log.Information($"Writing PacketId: {id} ({id.ToRaw()})");
            ms.Write7BitEncodedUInt(id.ToRaw());

            await packet.WriteAsync(ms);
            var packetEnd = (uint)ms.Position;
            var packetSize = packetEnd - packetStart;

            // Get the size of the packet size (8 bits / 7 bits = X bytes)
            var packetSizeBitCount = (ulong)(Math.Log(packetSize, 256) * 8);
            var packetSizeByteCount = (uint)(((packetSizeBitCount - 1) / 7) + 1);

            // Get the position we want to start writing size at
            var sizeStart = packetStart - packetSizeByteCount;

            Log.Information("packetSize: {@packetSize}, packetSizeByteCount: {@packetSizeByteCount}, sizeStart: {@sizeStart}", packetSize, packetSizeByteCount, sizeStart);

            // Write packet length
            ms.Seek(sizeStart, SeekOrigin.Begin);
            ms.Write7BitEncodedUInt(packetSize);

            // Return only the part of the stream that contains the packet (skipping any unused space at the start)
            var buffer = ms.GetBuffer();
            return new Memory<byte>(buffer, (int)sizeStart, (int)(packetEnd - sizeStart));
        }
    }
}
