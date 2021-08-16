using System.IO;
using System.Text;
using System.Threading.Tasks;
using Currycomb.Common.Extensions;

namespace Currycomb.Common.Network.Game.Packets
{
    public readonly struct PacketClientChat : IGamePacket
    {
        public readonly string Message;

        public PacketClientChat(string message) => Message = message;
        public static async Task<PacketClientChat> ReadAsync(Stream stream)
        {
            int length = await stream.Read7BitEncodedIntAsync();
            byte[] buffer = new byte[length];

            if (await stream.ReadAsync(buffer, 0, length) != length)
                throw new InvalidDataException("Packet incomplete");

            string message = Encoding.UTF8.GetString(buffer);
            return new PacketClientChat(message);
        }
    }
}
