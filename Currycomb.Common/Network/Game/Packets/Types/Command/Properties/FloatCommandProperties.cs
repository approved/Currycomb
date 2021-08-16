using System.IO;

namespace Currycomb.Common.Network.Game.Packets.Types.Command.Properties
{
    public readonly struct FloatCommandProperties : ICommandProperties
    {
        public readonly float? Min;
        public readonly float? Max;

        public FloatCommandProperties(float? min, float? max)
        {
            Min = min;
            Max = max;
        }

        public FloatCommandProperties(BinaryReader reader)
        {
            byte flags = reader.ReadByte();
            Min = ((flags & 0x01) != 0) ? reader.ReadSingle() : null;
            Max = ((flags & 0x02) != 0) ? reader.ReadSingle() : null;
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