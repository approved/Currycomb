using System.IO;

namespace Currycomb.Common.Network.Game.Packets.Types.Player
{
    public interface IPlayerInfoAction
    {
        abstract void Serialize(BinaryWriter writer);
    }
}
