using System;
using System.Linq;
using System.Collections.Generic;
using Currycomb.Common.Network.Game.Packets;

namespace Currycomb.Common.Network.Game
{
    public struct GamePacketIdMap<T> where T : IGamePacket
    {
        public static GamePacketId Id { get; } = GamePacketIdMap.FromType(typeof(T));
    }

    public static class GamePacketIdMap
    {
        static readonly Dictionary<GamePacketId, Type> MapIdToType = new()
        {

            #region Handshake
            { GamePacketId.Handshake, typeof(PacketHandshake) },
            #endregion

            #region Login - ServerBound
            { GamePacketId.LoginStart, typeof(PacketLoginStart) },
            { GamePacketId.ClientEncryption, typeof(PacketEncryptionResponse) },
            #endregion

            #region Login - ClientBound
            { GamePacketId.ServerEncryption, typeof(PacketEncryptionRequest) },
            { GamePacketId.LoginSuccess, typeof(PacketLoginSuccess) },
            #endregion

            #region Status - ServerBound
            { GamePacketId.Request, typeof(PacketRequest) },
            { GamePacketId.Ping, typeof(PacketPing) },
            #endregion

            #region Status - ClientBound
            { GamePacketId.Response, typeof(PacketResponse) },
            { GamePacketId.Pong, typeof(PacketPong) },
            #endregion

            #region Play - ServerBound
            { GamePacketId.ClientInformation, typeof(PacketClientInformation) },

            { GamePacketId.ClientCustomPayload, typeof(PacketClientCustomPayload) },

            { GamePacketId.ClientKeepAlive, typeof(PacketClientKeepAlive) },

            // { GamePacketId.ClientMovePlayerPos, typeof(PacketClientMovePlayerPos) },
            // { GamePacketId.ClientMovePlayer, typeof(PacketClientMovePlayer) },

            { GamePacketId.ClientPlayerAbilities, typeof(PacketClientPlayerAbilities) },
            #endregion

            #region Play - ClientBound
            { GamePacketId.ChangeDifficulty, typeof(PacketChangeDifficulty) },

            { GamePacketId.ContainerSetContent, typeof(PacketSetContainer) },

            { GamePacketId.ServerCustomPayload, typeof(PacketServerCustomPayload) },

            { GamePacketId.CommandList, typeof(PacketCommandList) },

            { GamePacketId.DisconnectPlay, typeof(PacketDisconnect) },
            { GamePacketId.EntityEvent, typeof(PacketEntityEvent) },

            { GamePacketId.ServerKeepAlive, typeof(PacketServerKeepAlive) },
            { GamePacketId.WorldChunk, typeof(PacketWorldChunk) },

            { GamePacketId.UpdateLight, typeof(PacketUpdateLight) },

            { GamePacketId.JoinGame, typeof(PacketJoinGame) },

            { GamePacketId.ServerPlayerAbilities, typeof(PacketServerPlayerAbilities) },

            { GamePacketId.PlayerInfo, typeof(PacketPlayerInfo) },
            //{ GamePacketId.PlayerLookAt, typeof(PacketPlayerLookAt) },
            { GamePacketId.PlayerPosition, typeof(PacketPlayerPosition) },
            { GamePacketId.Recipe, typeof(PacketRecipe) },

            { GamePacketId.SetHeldItem, typeof(PacketSetHeldItem) },
            { GamePacketId.ChunkCacheCenter, typeof(PacketChunkCacheCenter) },

            { GamePacketId.SpawnPosition, typeof(PacketSpawnPosition) },

            { GamePacketId.SetTime, typeof(PacketSetTime) },

            //{ GamePacketId.UpdateAdvancements, typeof(PacketUpdateAdvancements) },
            //{ GamePacketId.UpdateAttributes, typeof(PacketUpdateAttributes) },
            //{ GamePacketId.UpdateMobEffects, typeof(PacketUpdateMobEffects) },
            { GamePacketId.UpdateRecipes, typeof(PacketUpdateRecipes) },
            { GamePacketId.UpdateTags, typeof(PacketUpdateTags) },
            #endregion
        };

        static readonly Dictionary<Type, GamePacketId> MapTypeToId = MapIdToType.ToDictionary(kvp => kvp.Value, kvp => kvp.Key);

        public static Type FromId(GamePacketId id) => MapIdToType[id];
        public static GamePacketId FromType(Type type) => MapTypeToId[type];
    }
}
