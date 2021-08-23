using Currycomb.Common.Network.Game.Packets.Types;

namespace Currycomb.PlayService.Game.Component
{
    public struct Rotation
    {
        public NetAngle Pitch;
        public NetAngle Yaw;

        public Rotation(NetAngle pitch, NetAngle yaw)
        {
            Pitch = pitch;
            Yaw = yaw;
        }
    }
}