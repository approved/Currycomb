namespace Currycomb.Common.Network.Game
{
    public enum GamePacketId : uint
    {
#pragma warning disable format // @formatter:off
        // Top 3 bits are in-use for our own metadata, if packet ids ever go that high
        // (and they're not likely to) then we need to rewrite how we handle this.
        //
        // Bit 31      = Client / Server bound
        // Bit 29 & 30 = State

        #region Handshake
        Handshake  = 0x00 | BoundTo.Server | State.Handshake,
        LegacyPing = 0xFE | BoundTo.Server | State.Handshake,
        #endregion

        #region Login - ServerBound
        LoginStart          = 0x00 | BoundTo.Server | State.Login,
        EncryptionResponse  = 0x01 | BoundTo.Server | State.Login,
        LoginPluginResponse = 0x02 | BoundTo.Server | State.Login,
        #endregion

        #region Login - ClientBound
        DisconnectLogin    = 0x00 | BoundTo.Client | State.Login,
        EncryptionRequest  = 0x01 | BoundTo.Client | State.Login,
        LoginSuccess       = 0x02 | BoundTo.Client | State.Login,
        SetCompression     = 0x03 | BoundTo.Client | State.Login,
        LoginPluginRequest = 0x04 | BoundTo.Client | State.Login,
        Kick               = 0xFF | BoundTo.Client | State.Login,
        #endregion

        #region Status - ServerBound
        Request = 0x00 | BoundTo.Server | State.Status,
        Ping    = 0x01 | BoundTo.Server | State.Status,
        #endregion

        #region Status - ClientBound
        Response = 0x00 | BoundTo.Client | State.Status,
        Pong     = 0x01 | BoundTo.Client | State.Status,
        #endregion

        #region Play - ServerBound
        ClientInformation     = 0x05 | BoundTo.Server | State.Play,

        ClientCustomPayload   = 0x0A | BoundTo.Server | State.Play,

        ClientKeepAlive       = 0x0F | BoundTo.Server | State.Play,

        ClientPlayerAbilities = 0x19 | BoundTo.Server | State.Play,
        #endregion

        #region Play - ClientBound
        SpawnEntity           = 0x00 | BoundTo.Client | State.Play,
        SpawnExperienceOrb    = 0x01 | BoundTo.Client | State.Play,
        SpawnMob              = 0x02 | BoundTo.Client | State.Play,
        SpawnPainting         = 0x03 | BoundTo.Client | State.Play,
        SpawnPlayer           = 0x04 | BoundTo.Client | State.Play,
        SpawnVibrations       = 0x05 | BoundTo.Client | State.Play,
        EntityAnimation       = 0x06 | BoundTo.Client | State.Play,
        GiveStats             = 0x07 | BoundTo.Client | State.Play,
        BlockBreakAck         = 0x08 | BoundTo.Client | State.Play,
        BlockBreakDestroy     = 0x09 | BoundTo.Client | State.Play,
        BlockEntityData       = 0x0A | BoundTo.Client | State.Play,
        BlockEvent            = 0x0B | BoundTo.Client | State.Play,
        BlockUpdate           = 0x0C | BoundTo.Client | State.Play,
        BossEvent             = 0x0D | BoundTo.Client | State.Play,
        ChangeDifficulty      = 0x0E | BoundTo.Client | State.Play,
        ChatMessage           = 0x0F | BoundTo.Client | State.Play,
        ClearTitles           = 0x10 | BoundTo.Client | State.Play,
        CommandSuggestions    = 0x11 | BoundTo.Client | State.Play,
        CommandList           = 0x12 | BoundTo.Client | State.Play,
        ContainerClose        = 0x13 | BoundTo.Client | State.Play,
        ContainerSetContent   = 0x14 | BoundTo.Client | State.Play,
        ContainerSetData      = 0x15 | BoundTo.Client | State.Play,
        ContainerSetSlot      = 0x16 | BoundTo.Client | State.Play,
        SetCooldown           = 0x17 | BoundTo.Client | State.Play,
        ServerCustomPayload   = 0x18 | BoundTo.Client | State.Play, // Also known as PluginMessage
        CustomSound           = 0x19 | BoundTo.Client | State.Play,
        DisconnectPlay        = 0x1A | BoundTo.Client | State.Play,
        EntityEvent           = 0x1B | BoundTo.Client | State.Play,

        ServerKeepAlive       = 0x21 | BoundTo.Client | State.Play,
        WorldChunk            = 0x22 | BoundTo.Client | State.Play,
        WorldEvent            = 0x23 | BoundTo.Client | State.Play,
        WorlParticle          = 0x24 | BoundTo.Client | State.Play,
        UpdateLight           = 0x25 | BoundTo.Client | State.Play,
        JoinGame              = 0x26 | BoundTo.Client | State.Play,

        ServerPlayerAbilities = 0x32 | BoundTo.Client | State.Play,
        PlayerCombatEnd       = 0x33 | BoundTo.Client | State.Play,
        PlayerCombatStart     = 0x34 | BoundTo.Client | State.Play,
        PlayerCombatKill      = 0x35 | BoundTo.Client | State.Play,
        PlayerInfo            = 0x36 | BoundTo.Client | State.Play,
        PlayerLookAt          = 0x37 | BoundTo.Client | State.Play,
        PlayerPosition        = 0x38 | BoundTo.Client | State.Play,
        Recipe                = 0x39 | BoundTo.Client | State.Play,

        SetHeldItem           = 0x48 | BoundTo.Client | State.Play,
        ChunkCacheCenter      = 0x49 | BoundTo.Client | State.Play,

        SpawnPosition         = 0x4B | BoundTo.Client | State.Play,

        SetTime               = 0x58 | BoundTo.Client | State.Play,

        UpdateAdvancements    = 0x63 | BoundTo.Client | State.Play,
        UpdateAttributes      = 0x64 | BoundTo.Client | State.Play,
        UpdateMobEffects      = 0x64 | BoundTo.Client | State.Play,
        UpdateRecipes         = 0x65 | BoundTo.Client | State.Play,
        UpdateTags            = 0x66 | BoundTo.Client | State.Play,
        #endregion

#pragma warning restore format // @formatter:on
    }

    public static class PacketIdExt
    {
        public static GamePacketId FromRaw(BoundTo bound, State state, uint id) => (GamePacketId)((uint)bound | (uint)state | id);
        public static uint ToRaw(this GamePacketId id) => (uint)id & 0b00011111111111111111111111111111;
        public static byte ToMetaBits(this GamePacketId id) => (byte)(((uint)id & 0b11100000000000000000000000000000) >> 29);

        public static BoundTo BoundTo(this GamePacketId id) => (BoundTo)((uint)id & 0x80000000);
        public static State State(this GamePacketId id) => (State)((uint)id & 0x60000000);
    }
}
