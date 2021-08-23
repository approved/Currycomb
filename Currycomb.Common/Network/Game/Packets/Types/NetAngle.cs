using System.IO;

namespace Currycomb.Common.Network.Game.Packets.Types
{
    public readonly struct NetAngle
    {
        public readonly byte Value;

        public NetAngle(byte value)
        {
            Value = value;
        }

        public NetAngle(BinaryReader reader)
        {
            Value = reader.ReadByte();
        }

        public void Write(BinaryWriter writer)
        {
            writer.Write(Value);
        }
    }
}