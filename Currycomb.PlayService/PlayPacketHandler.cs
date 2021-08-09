using Currycomb.Common.Game;
using Currycomb.Common.Network.Minecraft;
using Currycomb.Common.Network.Minecraft.Packets;
using Serilog;
using System.Numerics;
using System.Threading.Tasks;

namespace Currycomb.PlayService
{
    public class PlayPacketHandler
    {
        PacketRouter<Context>? _router;
        public PacketRouter<Context> Router => _router ??= PacketRouter<Context>.New()
            .On<PacketJoinGame>(PacketJoinGame)
            .On<PacketSpawnPosition>(PacketSpawnPosition)
            .On<PacketPlayerPosition>(PacketPlayerPosition)
            .Build();

        private async Task PacketJoinGame(Context c, PacketJoinGame pkt)
        {
            await c.SendPacket(new PacketJoinGame(0, false, GameMode.Creative, GameMode.None, new[] { "World" }, "World", 0, 32, false, false, false, false));
            await c.SetState(State.Play);

            Log.Information("Replied to PacketLoginStart");
        }

        private async Task PacketSpawnPosition(Context c, PacketSpawnPosition pkt)
        {
            await c.SendPacket(new PacketSpawnPosition(Vector3.Zero, 0f));

            Log.Information("Replied to PacketSpawnPosition");
        }

        private async Task PacketPlayerPosition(Context c, PacketPlayerPosition pkt)
        {
            await c.SendPacket(new PacketPlayerPosition(0, 0, 0, 0, 0, 0, false));

            Log.Information("Replied to PacketPlayerPosition");
        }
    }
}
