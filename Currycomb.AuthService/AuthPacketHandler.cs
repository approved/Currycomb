using System;
using System.Security.Cryptography;
using System.Threading.Tasks;
using Currycomb.Common.Network.Game;
using Currycomb.Common.Network.Game.Packets;
using Currycomb.Common.Network.Game.Packets.Types;
using Currycomb.Common.Network.Meta;
using Currycomb.Common.Network.Meta.Packets;
using Serilog;

namespace Currycomb.AuthService
{
    public class AuthPacketHandler
    {
        private GamePacketRouter<Context>? _router;
        public GamePacketRouter<Context> Router => _router ??= GamePacketRouter<Context>.New()
            .On<PacketHandshake>(PacketHandshake)
            .On<PacketLoginStart>(PacketLoginStart)
            .On<PacketEncryptionResponse>(PacketEncryptionResponse)
            .On<PacketRequest>(PacketRequest)
            .On<PacketPing>(PacketPing)
            .Build();

        Task PacketHandshake(Context c, PacketHandshake pkt)
            => c.SetState(pkt.State);

        private async Task PacketLoginStart(Context c, PacketLoginStart pkt)
        {
            // TODO: Configuration - we should allow toggling this somewhere
            const bool useEncryption = false;

            if (useEncryption)
            {
                Log.Information("Starting Encryption");
                await c.SendPacket(new PacketEncryptionRequest(string.Empty, c.Rsa.ExportSubjectPublicKeyInfo(), c.VerifyToken));
            }
            else
            {
                Log.Warning("Starting In Insecure Mode: No Encryption");
                await SendLoginSuccess(c);
            }
            Log.Information("Replied to PacketLoginStart");
        }

        Task PacketPing(Context c, PacketPing pkt)
            => c.SendPacket(new PacketPong(pkt.Timestamp));

        Task PacketRequest(Context c, PacketRequest pkt)
            => c.SendPacket(new PacketResponse("{\"version\":{\"name\": \"1.17.1\",\"protocol\": 756},\"players\":{\"max\":100,\"online\":5},\"description\":{\"text\":\"Hello world!\"}}"));

        async Task PacketEncryptionResponse(Context c, PacketEncryptionResponse pkt)
        {
            Log.Information("Received PacketEncryptionResponse: {@packet}", pkt);

            var secret = c.Rsa.Decrypt(pkt.SharedSecret, RSAEncryptionPadding.Pkcs1);
            var verify = c.Rsa.Decrypt(pkt.VerifyToken, RSAEncryptionPadding.Pkcs1);

            Log.Information("Secret: {@secret}", secret);
            Log.Information("Verify: {@verify}", verify);

            if (!new Span<byte>(c.VerifyToken).SequenceEqual(verify))
            {
                Log.Error("VerifyToken mismatch");
                await c.SendPacket(new PacketDisconnectLogin(new Chat("VerifyToken mismatch")));
                // TODO: Force kick the client, something went wrong.
            }

            // TODO: Authenticate with Mojang.

            await c.SendMetaPacket(new PacketSetAesKey(secret));
            await SendLoginSuccess(c);
        }

        private async Task SendLoginSuccess(Context c)
        {
            await c.SendPacket(new PacketLoginSuccess(Guid.NewGuid(), "Fiskpinne"));
            await c.SetState(State.Play);
        }
    }

    public class MetaPacketHandler
    {
        private MetaPacketRouter<Context>? _router;
        public MetaPacketRouter<Context> Router => _router ??= MetaPacketRouter<Context>.New()
            .Build();
    }
}
