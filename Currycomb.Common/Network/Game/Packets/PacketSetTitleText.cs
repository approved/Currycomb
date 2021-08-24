using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Currycomb.Common.Network.Game.Packets
{
    [GamePacket(GamePacketId.SetTitleText)]
    public readonly struct PacketSetTitleText : IGamePacket
    {
        public readonly string TitleText;

        public PacketSetTitleText(string titleText)
        {
            TitleText = titleText;
        }

        public PacketSetTitleText(BinaryReader reader)
        {
            TitleText = reader.ReadString();
        }

        public void Write(BinaryWriter writer)
        {
            writer.Write(TitleText);
        }
    }
}
