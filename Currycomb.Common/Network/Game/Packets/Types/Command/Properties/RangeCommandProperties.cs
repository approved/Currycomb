using System.IO;

namespace Currycomb.Common.Network.Game.Packets.Types.Command.Properties
{
    public readonly struct RangeCommandProperties : ICommandProperties
    {
        public readonly bool Decimals;

        public RangeCommandProperties(bool decimals)
        {
            Decimals = decimals;
        }

        public RangeCommandProperties(BinaryReader reader)
        {
            Decimals = reader.ReadBoolean();
        }

        public void Write(BinaryWriter writer)
        {
            writer.Write(Decimals);
        }
    }
}