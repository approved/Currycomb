using System;
using System.IO;

namespace Currycomb.Common.Network.Game.Packets.Types.Player
{
    public record PingPlayerInfoAction(Guid UUID, int Ping) : IPlayerInfoAction
    {
        public void Serialize(BinaryWriter writer)
        {
            writer.Write(UUID.ToByteArray());
            writer.Write7BitEncodedInt(Ping);
        }
    }
}
