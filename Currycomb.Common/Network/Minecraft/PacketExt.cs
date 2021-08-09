using System;
using System.IO;
using Serilog;

namespace Currycomb.Common.Network.Minecraft
{
    public static class PacketExt
    {
        public static Memory<byte> ToBytes<T>(this T packet) where T : IPacket
            => ToBytes(packet, PacketIdMap<T>.Id);

        public static Memory<byte> ToBytes<T>(this T packet, PacketId id) where T : IPacket
        {
            using MemoryStream ms = new();
            using BinaryWriter bw = new(ms);

            // Placeholder space for packet Length
            bw.Write7BitEncodedInt(unchecked((int)uint.MaxValue));
            bw.Write7BitEncodedInt(unchecked((int)id.ToRaw()));

            // Max bytes for length is 5 (4 + 1 bit per byte as "continues next bit" flag)
            var packetStart = 5;

            packet.Write(bw);
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
            bw.Write7BitEncodedInt(unchecked((int)packetSize));

            // Return only the part of the stream that contains the packet (skipping any unused space at the start)
            return new Memory<byte>(ms.GetBuffer(), (int)sizeStart, (int)(packetEnd - sizeStart));
        }
    }
}
