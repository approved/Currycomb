using System;
using System.IO;
using System.Threading.Tasks;

namespace Currycomb.Common.Network.Minecraft.Packets
{
    public record PacketLoginSuccess(Guid Uuid, String Username) : IPacket
    {
        public void Write(BinaryWriter writer)
        {
            writer.Write(Uuid.ToByteArray());
            writer.Write(Username);
        }
    }
}
