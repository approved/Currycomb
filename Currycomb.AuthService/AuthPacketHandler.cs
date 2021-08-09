using System;
using System.Threading.Tasks;
using Currycomb.Common.Network.Broadcast;
using Currycomb.Common.Network.Minecraft;
using Currycomb.Common.Network.Minecraft.Packets;
using Serilog;

namespace Currycomb.AuthService
{
    public class AuthPacketHandler
    {
        PacketRouter<Context>? _router;
        public PacketRouter<Context> Router => _router ??= PacketRouter<Context>.New()
            .On<PacketHandshake>(PacketHandshake)
            .On<PacketLoginStart>(PacketLoginStart)
            .On<PacketRequest>(PacketRequest)
            .On<PacketPing>(PacketPing)
            .Build();

        Task PacketRequest(Context c, PacketRequest pkt)
            => c.SendPacket(new PacketResponse("{\"version\":{\"name\": \"1.17.1\",\"protocol\": 756},\"players\":{\"max\":100,\"online\":5},\"description\":{\"text\":\"Hello world!\"}}"));

        Task PacketPing(Context c, PacketPing pkt)
            => c.SendPacket(new PacketPong(pkt.Timestamp));

        Task PacketHandshake(Context c, PacketHandshake pkt)
            => c.SetState(pkt.State);

        async Task PacketLoginStart(Context c, PacketLoginStart pkt)
        {
            await c.SendPacket(new PacketLoginSuccess(Guid.NewGuid(), "Fiskpinne"));
            await c.SetState(State.Play);

            Log.Information("Replied to PacketLoginStart");
        }
    }
}
