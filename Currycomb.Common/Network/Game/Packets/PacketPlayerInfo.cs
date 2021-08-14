using Currycomb.Common.Network.Game.Packets.Types.Player;
using System.IO;

namespace Currycomb.Common.Network.Game.Packets
{
    public record PacketPlayerInfo(PlayerInfoAction Action, IPlayerInfoAction[] Actions) : IGamePacket
    {
        public void Write(BinaryWriter writer)
        {
            writer.Write7BitEncodedInt((int)Action);
            writer.Write7BitEncodedInt(Actions.Length);
            foreach (IPlayerInfoAction action in Actions)
            {
                action.Serialize(writer);
            }
        }
    }

    public enum PlayerInfoAction
    {
        AddPlayer,
        UpdateGameMode,
        UpdateLatency,
        UpdateDisplayName,
        RemovePlayer
    }
}
