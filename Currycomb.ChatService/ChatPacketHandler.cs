using Currycomb.Common.Network.Game;
using Currycomb.Common.Network.Game.Packets;
using Newtonsoft.Json;
using System;
using System.Threading.Tasks;

namespace Currycomb.ChatService
{
    public class ChatPacketHandler
    {
        GamePacketRouter<Context>? _router;
        public GamePacketRouter<Context> Router => _router ??= GamePacketRouter<Context>.New()
            .On<PacketClientChat>(PacketClientChat)
            .Build();

        private async Task PacketClientChat(Context c, PacketClientChat pkt)
            => await c.BroadcastPacket(new PacketChatMessage(JsonConvert.SerializeObject(new { text = pkt.Message }), 0, Guid.Empty));
    }
}
