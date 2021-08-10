using System;
using System.IO;
using Serilog;

namespace Currycomb.Common.Network.Game
{
    public static class GamePacketExt
    {
        public static Memory<byte> ToBytes<T>(this T packet) where T : IGamePacket
            => ToBytes(packet, GamePacketIdMap<T>.Id);

        public static Memory<byte> ToBytes<T>(this T packet, GamePacketId id) where T : IGamePacket
        {
            using MemoryStream ms = new();
            using BinaryWriter bw = new(ms);

            Log.Information("Sending packet: {@packetId}", id);

            // Placeholder space for packet Length
            bw.Write7BitEncodedInt(unchecked((int)uint.MaxValue));
            var packetStart = ms.Position;

            bw.Write7BitEncodedInt(unchecked((int)id.ToRaw()));

            packet.Write(bw);
            var packetEnd = (uint)ms.Position;
            var packetSize = (uint)(packetEnd - packetStart);

            var packetSizeByteCount = packetSize switch
            {
                < 0x______80 => 1,
                < 0x____4000 => 2,
                < 0x__200000 => 3,
                < 0x10000000 => 4,
                _ => 5,
            };

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
