using System;
using System.IO;
using System.Threading.Tasks;

namespace Currycomb.Common.Network.Game.Packets
{
    public class PacketKeepAlive : IGamePacket
    {
        private static readonly Random _random = new Random();
        private static readonly byte[] _randBuffer = new byte[8];

        public void Write(BinaryWriter writer)
        {
            _random.NextBytes(_randBuffer);
            writer.Write(_randBuffer);
        }
    }

    public class PacketClientKeepAlive : PacketKeepAlive
    {

        // TODO: Implement Keep-Alive to ensure client activity
        public static async Task<PacketClientKeepAlive> ReadAsync(Stream stream) => new();
    }
    public class PacketServerKeepAlive : PacketKeepAlive { }
}
