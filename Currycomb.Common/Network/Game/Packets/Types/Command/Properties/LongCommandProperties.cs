using System.IO;

namespace Currycomb.Common.Network.Game.Packets.Types.Command.Properties
{
    public readonly struct LongCommandProperties : ICommandProperties
    {
        public readonly long? Min;
        public readonly long? Max;

        public LongCommandProperties(long? min, long? max)
        {
            Min = min;
            Max = max;
        }

        public LongCommandProperties(BinaryReader reader)
        {
            byte flags = reader.ReadByte();
            Min = ((flags & 0x01) != 0) ? reader.ReadInt64() : null;
            Max = ((flags & 0x02) != 0) ? reader.ReadInt64() : null;
        }

        public void Write(BinaryWriter writer)
        {
            byte flags = (byte)((Min.HasValue ? 0x01 : 0x00) | (Max.HasValue ? 0x02 : 0x00));
            writer.Write(flags);
            if (Min.HasValue) writer.Write(Min.Value);
            if (Max.HasValue) writer.Write(Max.Value);
        }
    }
}