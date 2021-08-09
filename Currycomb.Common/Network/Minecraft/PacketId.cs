using System.Runtime.CompilerServices;

namespace Currycomb.Common.Network.Minecraft
{
    public enum PacketId : uint
    {
#pragma warning disable format // @formatter:off
        // Top 3 bits are in-use for our own metadata, if packet ids ever go that high
        // (and they're not likely to) then we need to rewrite how we handle this.
        //
        // Bit 31      = Client / Server bound
        // Bit 29 & 30 = State

        // Handshake
        Handshake  = 0x00 | BoundTo.Server | State.Handshake,
        LegacyPing = 0xFE | BoundTo.Server | State.Handshake,
        Kick       = 0xFF | BoundTo.Client | State.Handshake,

        // Login
        Disconnect          = 0x00 | BoundTo.Client | State.Login,
        EncryptionRequest   = 0x01 | BoundTo.Client | State.Login,
        LoginSuccess        = 0x02 | BoundTo.Client | State.Login,
        SetCompression      = 0x03 | BoundTo.Client | State.Login,
        LoginPluginRequest  = 0x04 | BoundTo.Client | State.Login,
        LoginStart          = 0x00 | BoundTo.Server | State.Login,
        EncryptionResponse  = 0x01 | BoundTo.Server | State.Login,
        LoginPluginResponse = 0x02 | BoundTo.Server | State.Login,

        // Status
        Request  = 0x00 | BoundTo.Server | State.Status,
        Response = 0x00 | BoundTo.Client | State.Status,
        Ping     = 0x01 | BoundTo.Server | State.Status,
        Pong     = 0x01 | BoundTo.Client | State.Status,

        // Play
        JoinGame             = 0x26 | BoundTo.Client | State.Play,
        PacketPlayerPosition = 0x38 | BoundTo.Client | State.Play,
        PacketSpawnPosition  = 0x4B | BoundTo.Client | State.Play,

#pragma warning restore format // @formatter:on
    }

    public static class PacketIdExt
    {
        public static PacketId FromRaw(BoundTo bound, State state, uint id) => (PacketId)((uint)bound | (uint)state | id);
        public static uint ToRaw(this PacketId id) => (uint)id & 0b00011111111111111111111111111111;
        public static byte ToMetaBits(this PacketId id) => (byte)(((uint)id & 0b11100000000000000000000000000000) >> 29);

        public static BoundTo BoundTo(this PacketId id) => (BoundTo)((uint)id & 0x80000000);
        public static State State(this PacketId id) => (State)((uint)id & 0x60000000);
    }
}
