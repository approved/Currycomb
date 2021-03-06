using Currycomb.Common.Game;
using Currycomb.Common.Network.Game;
using Currycomb.Common.Network.Game.Packets;
using Serilog;
using System.Numerics;
using System.Threading.Tasks;

namespace Currycomb.PlayService
{
    public class PlayPacketHandler
    {
        GamePacketRouter<Context>? _router;
        public GamePacketRouter<Context> Router => _router ??= GamePacketRouter<Context>.New()
            .On<PacketJoinGame>(PacketJoinGame)
            .On<PacketSpawnPosition>(PacketSpawnPosition)
            .On<PacketPlayerPosition>(PacketPlayerPosition)
            .On<PacketClientKeepAlive>(PacketClientKeepAlive)
            .Build();

        private async Task PacketJoinGame(Context c, PacketJoinGame pkt)
        {
            await c.SendPacket(new PacketJoinGame(0, false, GameMode.Creative, GameMode.None, new[] { "world" }, "world", 0, 100, 32, false, false, false, false));
        }

        private async Task PacketSpawnPosition(Context c, PacketSpawnPosition pkt)
        {
            await c.SendPacket(new PacketSpawnPosition(Vector3.Zero, 0f));

            Log.Information("Replied to PacketSpawnPosition");
        }

        private async Task PacketPlayerPosition(Context c, PacketPlayerPosition pkt)
        {
            await c.SendPacket(new PacketPlayerPosition(0, 0, 0, 0, 0, 0, 0, false));

            Log.Information("Replied to PacketPlayerPosition");
        }

        private async Task PacketClientKeepAlive(Context c, PacketClientKeepAlive pkt)
        {
            Log.Information("Received PacketClientKeepAlive");
        }
    }
}
