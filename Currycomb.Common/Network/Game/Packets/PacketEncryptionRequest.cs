using System.IO;

namespace Currycomb.Common.Network.Game.Packets
{
    [GamePacket(GamePacketId.ServerEncryption)]
    public readonly struct PacketEncryptionRequest : IGamePacket
    {
        public readonly byte[] PublicKey;   // The server's public key in bytes.
        public readonly string ServerId;    // Empty on 1.7.x and onwards.
        public readonly byte[] VerifyToken; // A sequence of random bytes generated by the server. Always 4 for Notchian servers.

        public PacketEncryptionRequest(string serverId, byte[] publicKey, byte[] verifyToken)
        {
            ServerId = serverId;
            PublicKey = publicKey;
            VerifyToken = verifyToken;
        }

        public PacketEncryptionRequest(BinaryReader reader)
        {
            ServerId = reader.ReadString();
            PublicKey = reader.ReadBytes(reader.Read7BitEncodedInt());
            VerifyToken = reader.ReadBytes(reader.Read7BitEncodedInt());
        }

        public void Write(BinaryWriter writer)
        {
            writer.Write(ServerId);
            writer.Write7BitEncodedInt(PublicKey.Length);
            writer.Write(PublicKey);
            writer.Write7BitEncodedInt(VerifyToken.Length);
            writer.Write(VerifyToken);
        }
    }
}
