using System.IO;

namespace Currycomb.Common.Network.Game.Packets.Types.Command.Properties
{
    public interface ICommandProperties
    {
        void Write(BinaryWriter writer);
    }
}