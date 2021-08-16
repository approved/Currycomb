using System;
using System.IO;

namespace Currycomb.Common.Network.Game.Packets
{
    // TODO: Implement.

    [GamePacket(GamePacketId.UpdateRecipes)]
    public readonly struct PacketUpdateRecipes : IGamePacket
    {
        public PacketUpdateRecipes(BinaryReader reader)
            => throw new NotImplementedException();

        public void Write(BinaryWriter writer)
        {
            writer.Write7BitEncodedInt(0);
        }
    }
}
