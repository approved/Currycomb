using System.IO;

namespace Currycomb.Common.Network.Game.Packets.Types.Command.Properties
{
    public readonly struct DoubleCommandProperties : ICommandProperties
    {
        public readonly double? Min;
        public readonly double? Max;

        public DoubleCommandProperties(double? min, double? max)
        {
            Min = min;
            Max = max;
        }

        public DoubleCommandProperties(BinaryReader reader)
        {
            byte flags = reader.ReadByte();
            Min = ((flags & 0x01) != 0) ? reader.ReadDouble() : null;
            Max = ((flags & 0x02) != 0) ? reader.ReadDouble() : null;
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