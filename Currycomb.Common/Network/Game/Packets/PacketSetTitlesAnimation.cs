using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Currycomb.Common.Network.Game.Packets
{
    [GamePacket(GamePacketId.SetTitlesAnimation)]
    public readonly struct PacketSetTitlesAnimation : IGamePacket
    {
        public readonly int FadeInTime;
        public readonly int StayTime;
        public readonly int FadeOutTime;

        public PacketSetTitlesAnimation(int fadeInTime, int stayTime, int fadeOutTime)
        {
            FadeInTime = fadeInTime;
            StayTime = stayTime;
            FadeOutTime = fadeOutTime;
        }

        public PacketSetTitlesAnimation(BinaryReader reader)
        {
            FadeInTime = reader.ReadInt32();
            StayTime = reader.ReadInt32();
            FadeOutTime = reader.ReadInt32();
        }

        public void Write(BinaryWriter writer)
        {
            writer.Write(FadeInTime);
            writer.Write(StayTime);
            writer.Write(FadeOutTime);
        }
    }
}
