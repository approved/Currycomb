namespace Currycomb.Common.Network.Minecraft
{
    public enum PacketId : uint
    {
#pragma warning disable format // @formatter:off
        // Bit 32      = Client / Server bound
        // Bit 30 & 31 = State

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

#pragma warning restore format // @formatter:on
    }

    public static class PacketIdExt
    {
        public static PacketId FromRaw(BoundTo bound, State state, uint id) => (PacketId)((uint)bound | (uint)state | id);
        public static uint ToRaw(this PacketId id) => (uint)id & 0x1FFFFFFF;

        public static BoundTo BoundTo(this PacketId id) => (BoundTo)((uint)id & 0x80000000);
        public static State State(this PacketId id) => (State)((uint)id & 0x60000000);
    }
}
