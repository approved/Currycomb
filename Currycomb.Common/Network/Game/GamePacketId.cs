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
        LoginStart        = 0x00 | BoundTo.Server | State.Login,
        ClientEncryption  = 0x01 | BoundTo.Server | State.Login,
        ClientCustomQuery = 0x02 | BoundTo.Server | State.Login,
        #endregion

        #region Login - ClientBound
        DisconnectLogin   = 0x00 | BoundTo.Client | State.Login,
        ServerEncryption  = 0x01 | BoundTo.Client | State.Login,
        LoginSuccess      = 0x02 | BoundTo.Client | State.Login,
        SetCompression    = 0x03 | BoundTo.Client | State.Login,
        ServerCustomQuery = 0x04 | BoundTo.Client | State.Login,
        Kick              = 0xFF | BoundTo.Client | State.Login,
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
        ClientAcceptTeleport           = 0x00 | BoundTo.Server | State.Play,
        ClientBlockEntityTagQuery      = 0x01 | BoundTo.Server | State.Play,
        ClientChangeDifficulty         = 0x02 | BoundTo.Server | State.Play,
        ClientChat                     = 0x03 | BoundTo.Server | State.Play,
        ClientCommand                  = 0x04 | BoundTo.Server | State.Play,
        ClientInformation              = 0x05 | BoundTo.Server | State.Play,
        ClientCommandSuggestion        = 0x06 | BoundTo.Server | State.Play,
        ClientContainerButtonClick     = 0x07 | BoundTo.Server | State.Play,
        ClientContainerClick           = 0x08 | BoundTo.Server | State.Play,
        ClientContainerClose           = 0x09 | BoundTo.Server | State.Play,
        ClientCustomPayload            = 0x0A | BoundTo.Server | State.Play,
        ClientEditBook                 = 0x0B | BoundTo.Server | State.Play,
        ClientEntityTagQuery           = 0x0C | BoundTo.Server | State.Play,
        ClientInteract                 = 0x0D | BoundTo.Server | State.Play,
        ClientJigsawGenerate           = 0x0E | BoundTo.Server | State.Play,
        ClientKeepAlive                = 0x0F | BoundTo.Server | State.Play,
        ClientLockDifficulty           = 0x10 | BoundTo.Server | State.Play,
        ClientMovePlayerPos            = 0x11 | BoundTo.Server | State.Play,
        ClientMovePlayer               = 0x12 | BoundTo.Server | State.Play,
        ClientMovePlayerRot            = 0x13 | BoundTo.Server | State.Play,
        ClientMovePlayerStatus         = 0x14 | BoundTo.Server | State.Play, // TODO: Add more descriptive comment
        ClientMoveVehicle              = 0x15 | BoundTo.Server | State.Play,
        ClientPaddleBoat               = 0x16 | BoundTo.Server | State.Play,
        ClientPickItem                 = 0x17 | BoundTo.Server | State.Play, // Create Mode (Default Button: Middle Click)
        ClientPlaceRecipe              = 0x18 | BoundTo.Server | State.Play,
        ClientPlayerAbilities          = 0x19 | BoundTo.Server | State.Play,
        ClientPlayerAction             = 0x1A | BoundTo.Server | State.Play,
        ClientPlayerCommand            = 0x1B | BoundTo.Server | State.Play,
        ClientPlayerInput              = 0x1C | BoundTo.Server | State.Play,
        ClientPongPlay                 = 0x1D | BoundTo.Server | State.Play,
        ClientRecipeBookChangeSettings = 0x1E | BoundTo.Server | State.Play,
        ClientSetRecipeBookScreen      = 0x1F | BoundTo.Server | State.Play,
        ClientRenameItem               = 0x20 | BoundTo.Server | State.Play,
        ClientResourcePack             = 0x21 | BoundTo.Server | State.Play,
        ClientAdvancementTabStatus     = 0x22 | BoundTo.Server | State.Play,
        ClientSelectTradeTab           = 0x23 | BoundTo.Server | State.Play,
        ClientSetBeacon                = 0x24 | BoundTo.Server | State.Play,
        ClientSetHeldItem              = 0x25 | BoundTo.Server | State.Play,
        ClientSetCommandBlock          = 0x26 | BoundTo.Server | State.Play,
        ClientSetCommandMinecart       = 0x27 | BoundTo.Server | State.Play,
        ClientSetCreateModeSlot        = 0x28 | BoundTo.Server | State.Play,
        ClientSetJigsawBlock           = 0x29 | BoundTo.Server | State.Play,
        ClientSignUpdate               = 0x2A | BoundTo.Server | State.Play,
        ClientSwing                    = 0x2B | BoundTo.Server | State.Play,
        ClientTeleportToEntity         = 0x2C | BoundTo.Server | State.Play,
        ClientUseItemOn                = 0x2D | BoundTo.Server | State.Play,
        ClientUseItem                  = 0x2E | BoundTo.Server | State.Play,
        #endregion

        #region Play - ClientBound
        SpawnEntity              = 0x00 | BoundTo.Client | State.Play,
        SpawnExperienceOrb       = 0x01 | BoundTo.Client | State.Play,
        SpawnMob                 = 0x02 | BoundTo.Client | State.Play,
        SpawnPainting            = 0x03 | BoundTo.Client | State.Play,
        SpawnPlayer              = 0x04 | BoundTo.Client | State.Play,
        SpawnVibrations          = 0x05 | BoundTo.Client | State.Play,
        EntityAnimation          = 0x06 | BoundTo.Client | State.Play,
        GiveStats                = 0x07 | BoundTo.Client | State.Play,
        BlockBreakAck            = 0x08 | BoundTo.Client | State.Play,
        BlockBreakDestroy        = 0x09 | BoundTo.Client | State.Play,
        BlockEntityData          = 0x0A | BoundTo.Client | State.Play,
        BlockEvent               = 0x0B | BoundTo.Client | State.Play,
        BlockUpdate              = 0x0C | BoundTo.Client | State.Play,
        BossEvent                = 0x0D | BoundTo.Client | State.Play,
        ChangeDifficulty         = 0x0E | BoundTo.Client | State.Play,
        ChatMessage              = 0x0F | BoundTo.Client | State.Play,
        ClearTitles              = 0x10 | BoundTo.Client | State.Play,
        CommandSuggestions       = 0x11 | BoundTo.Client | State.Play,
        CommandList              = 0x12 | BoundTo.Client | State.Play,
        ContainerClose           = 0x13 | BoundTo.Client | State.Play,
        ContainerSetContent      = 0x14 | BoundTo.Client | State.Play,
        ContainerSetData         = 0x15 | BoundTo.Client | State.Play,
        ContainerSetSlot         = 0x16 | BoundTo.Client | State.Play,
        SetCooldown              = 0x17 | BoundTo.Client | State.Play,
        ServerCustomPayload      = 0x18 | BoundTo.Client | State.Play, // Also known as PluginMessage
        CustomSound              = 0x19 | BoundTo.Client | State.Play,
        DisconnectPlay           = 0x1A | BoundTo.Client | State.Play,
        EntityEvent              = 0x1B | BoundTo.Client | State.Play,
        Explosion                = 0x1C | BoundTo.Client | State.Play,
        UnloadWorldChunk         = 0x1D | BoundTo.Client | State.Play,
        GameEvent                = 0x1E | BoundTo.Client | State.Play,
        OpenHorseScreen          = 0x1F | BoundTo.Client | State.Play,
        InitializeWorldBorder    = 0x20 | BoundTo.Client | State.Play,
        ServerKeepAlive          = 0x21 | BoundTo.Client | State.Play,
        WorldChunk               = 0x22 | BoundTo.Client | State.Play,
        WorldEvent               = 0x23 | BoundTo.Client | State.Play,
        WorlParticle             = 0x24 | BoundTo.Client | State.Play,
        UpdateLight              = 0x25 | BoundTo.Client | State.Play,
        JoinGame                 = 0x26 | BoundTo.Client | State.Play,
        MapItemData              = 0x27 | BoundTo.Client | State.Play,
        MerchantOffers           = 0x28 | BoundTo.Client | State.Play,
        MoveEntityPos            = 0x29 | BoundTo.Client | State.Play,
        MoveEntity               = 0x2A | BoundTo.Client | State.Play,
        MoveEntityRot            = 0x2B | BoundTo.Client | State.Play,
        MoveVehicle              = 0x2C | BoundTo.Client | State.Play,
        OpenBook                 = 0x2D | BoundTo.Client | State.Play,
        OpenScreen               = 0x2E | BoundTo.Client | State.Play,
        OpenSignEditor           = 0x2F | BoundTo.Client | State.Play,
        ServerPlayPing           = 0x30 | BoundTo.Client | State.Play,
        PlayGhostRecipe          = 0x31 | BoundTo.Client | State.Play,
        ServerPlayerAbilities    = 0x32 | BoundTo.Client | State.Play,
        PlayerCombatEnd          = 0x33 | BoundTo.Client | State.Play,
        PlayerCombatStart        = 0x34 | BoundTo.Client | State.Play,
        PlayerCombatKill         = 0x35 | BoundTo.Client | State.Play,
        PlayerInfo               = 0x36 | BoundTo.Client | State.Play,
        PlayerLookAt             = 0x37 | BoundTo.Client | State.Play,
        PlayerPosition           = 0x38 | BoundTo.Client | State.Play,
        Recipe                   = 0x39 | BoundTo.Client | State.Play,
        RemoveEntities           = 0x3A | BoundTo.Client | State.Play,
        RemoveMobEffects         = 0x3B | BoundTo.Client | State.Play,
        ResourcePack             = 0x3C | BoundTo.Client | State.Play, // Rename to something more descript?
        Respawn                  = 0x3D | BoundTo.Client | State.Play,
        RotateHead               = 0x3E | BoundTo.Client | State.Play,
        ChunkBlocksUpdate        = 0x3F | BoundTo.Client | State.Play,
        SetAdvancementTab        = 0x40 | BoundTo.Client | State.Play,
        SetActionBarText         = 0x41 | BoundTo.Client | State.Play,
        SetBorderCenter          = 0x42 | BoundTo.Client | State.Play,
        SetBorderLerpSize        = 0x43 | BoundTo.Client | State.Play,
        SetBorderSize            = 0x44 | BoundTo.Client | State.Play,
        SetBorderWarningDelay    = 0x45 | BoundTo.Client | State.Play,
        SetBorderWarningDistance = 0x46 | BoundTo.Client | State.Play,
        SetCamera                = 0x47 | BoundTo.Client | State.Play,
        SetHeldItem              = 0x48 | BoundTo.Client | State.Play,
        ChunkCacheCenter         = 0x49 | BoundTo.Client | State.Play,
        ChunkCacheRadius         = 0x4A | BoundTo.Client | State.Play,
        SpawnPosition            = 0x4B | BoundTo.Client | State.Play,
        SetDisplayObjective      = 0x4C | BoundTo.Client | State.Play,
        SetEntityData            = 0x4D | BoundTo.Client | State.Play,
        SetEntityLink            = 0x4E | BoundTo.Client | State.Play,
        SetEntityMotion          = 0x4F | BoundTo.Client | State.Play,
        SetEquipment             = 0x50 | BoundTo.Client | State.Play,
        SetExperience            = 0x51 | BoundTo.Client | State.Play,
        SetHealth                = 0x52 | BoundTo.Client | State.Play,
        SetObjective             = 0x53 | BoundTo.Client | State.Play,
        SetPassengers            = 0x54 | BoundTo.Client | State.Play,
        SetPlayerTeam            = 0x55 | BoundTo.Client | State.Play,
        SetScore                 = 0x56 | BoundTo.Client | State.Play,
        SetSubtitleText          = 0x57 | BoundTo.Client | State.Play,
        SetTime                  = 0x58 | BoundTo.Client | State.Play,
        SetTitleText             = 0x59 | BoundTo.Client | State.Play,
        SetTitlesAnimation       = 0x5A | BoundTo.Client | State.Play,
        PlayEntitySound          = 0x5B | BoundTo.Client | State.Play,
        PlaySound                = 0x5C | BoundTo.Client | State.Play,
        StopSound                = 0x5D | BoundTo.Client | State.Play,
        PlayerListDecoration     = 0x5E | BoundTo.Client | State.Play,
        TagQuery                 = 0x5F | BoundTo.Client | State.Play,
        TakeItem                 = 0x60 | BoundTo.Client | State.Play,
        TeleportEntity           = 0x61 | BoundTo.Client | State.Play,
        UpdateAdvancements       = 0x62 | BoundTo.Client | State.Play,
        UpdateAttributes         = 0x63 | BoundTo.Client | State.Play,
        UpdateMobEffects         = 0x64 | BoundTo.Client | State.Play,
        UpdateRecipes            = 0x65 | BoundTo.Client | State.Play,
        UpdateTags               = 0x66 | BoundTo.Client | State.Play,
        #endregion

#pragma warning restore format // @formatter:on
    }

    public static class GamePacketIdExt
    {
        public static GamePacketId FromRaw(BoundTo bound, State state, uint id) => (GamePacketId)((uint)bound | (uint)state | id);
        public static uint ToRaw(this GamePacketId id) => (uint)id & 0b00011111111111111111111111111111;
        public static byte ToMetaBits(this GamePacketId id) => (byte)(((uint)id & 0b11100000000000000000000000000000) >> 29);

        public static BoundTo BoundTo(this GamePacketId id) => (BoundTo)((uint)id & 0x80000000);
        public static State State(this GamePacketId id) => (State)((uint)id & 0x60000000);
    }
}
