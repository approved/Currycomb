using System.IO;
using System.Threading.Tasks;

namespace Currycomb.Common.Network.Game.Packets
{
    public record PacketClientMovePlayerPos(double XPos,  double YPos, double ZPos, float Yaw, float Pitch, bool OnGround) : IGamePacket
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
