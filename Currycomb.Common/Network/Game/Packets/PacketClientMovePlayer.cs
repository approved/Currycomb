using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Currycomb.Common.Network.Game.Packets
{
    public record PacketClientMovePlayer(double XPos, double YPos, double ZPos, float Yaw, float Pitch, bool OnGround) : IGamePacket
    {
        //public async Task<PacketClientMovePlayerPos> ReadAsync(Stream stream) =>
        //    new(
        //        stream.ReadDouble(),
        //        stream.ReadDouble(),
        //        stream.ReadDouble(),
        //        stream.ReadSingle(),
        //        stream.ReadSingle(),
        //        stream.ReadBoolean());
    }
}
