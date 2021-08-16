using Currycomb.Common.Network.Game.Packets.Types.Player;
using System;
using System.IO;

namespace Currycomb.Common.Network.Game.Packets
{
    [GamePacket(GamePacketId.PlayerInfo)]
    public readonly struct PacketPlayerInfo : IGamePacket
    {
        public readonly PlayerInfoAction Action;
        public readonly IPlayerInfoAction[] Actions;

        public PacketPlayerInfo(PlayerInfoAction action, params IPlayerInfoAction[] actions)
        {
            Action = action;
            Actions = actions;
        }

        public PacketPlayerInfo(BinaryReader reader)
            => throw new NotImplementedException();

        public void Write(BinaryWriter writer)
        {
            writer.Write7BitEncodedInt((int)Action);

            writer.Write7BitEncodedInt(Actions.Length);
            foreach (IPlayerInfoAction action in Actions)
                action.Serialize(writer);
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
