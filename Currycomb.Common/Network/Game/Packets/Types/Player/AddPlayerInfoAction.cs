using Currycomb.Common.Game;
using System;
using System.IO;

namespace Currycomb.Common.Network.Game.Packets.Types.Player
{
    public record AddPlayerInfoAction(Guid UUID, string Player, InfoActionProperty[] Properties, GameMode GameMode, int Ping, bool ShowDisplayName = false, string DisplayName = "") : IPlayerInfoAction
    {
        public void Serialize(BinaryWriter writer)
        {
            writer.Write(UUID.ToByteArray());
            writer.Write(Player);
            writer.Write7BitEncodedInt(Properties.Length);
            foreach(InfoActionProperty property in Properties)
            {
                writer.Write(property.Name);
                writer.Write(property.Value);
                writer.Write(property.IsSigned);
                writer.Write(property.Signature);
            }
            writer.Write7BitEncodedInt((int)GameMode);
            writer.Write7BitEncodedInt(Ping);
            writer.Write(ShowDisplayName);
            if (ShowDisplayName)
            {
                writer.Write(DisplayName);
            }
        }
    }
}
