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
            { GamePacketId.LoginStart, typeof(PacketLoginStart) },
            { GamePacketId.LoginSuccess, typeof(PacketLoginSuccess) },

            { GamePacketId.Handshake, typeof(PacketHandshake) },

            { GamePacketId.Request, typeof(PacketRequest) },
            { GamePacketId.Response, typeof(PacketResponse) },

            { GamePacketId.Ping, typeof(PacketPing) },
            { GamePacketId.Pong, typeof(PacketPong) },

            { GamePacketId.EncryptionRequest, typeof(PacketEncryptionRequest) },
            { GamePacketId.EncryptionResponse, typeof(PacketEncryptionResponse) },

            // Play - Client
            { GamePacketId.ClientInformation, typeof(PacketClientInformation) },
            { GamePacketId.ClientCustomPayload, typeof(PacketClientCustomPayload) },
            { GamePacketId.ClientPlayerAbilities, typeof(PacketClientPlayerAbilities) },

            // Play - Server
            { GamePacketId.ChangeDifficulty, typeof(PacketChangeDifficulty) },
            { GamePacketId.ServerCustomPayload, typeof(PacketServerCustomPayload) },
            { GamePacketId.DisconnectPlay, typeof(PacketDisconnect) },
            { GamePacketId.JoinGame, typeof(PacketJoinGame) },
            { GamePacketId.ServerPlayerAbilities, typeof(PacketServerPlayerAbilities) },
            { GamePacketId.PacketPlayerPosition, typeof(PacketPlayerPosition) },
            { GamePacketId.SetHeldItem, typeof(PacketSetHeldItem) },
            { GamePacketId.PacketSpawnPosition, typeof(PacketSpawnPosition) },
        };

        static readonly Dictionary<Type, GamePacketId> MapTypeToId = MapIdToType.ToDictionary(kvp => kvp.Value, kvp => kvp.Key);

        public static Type FromId(GamePacketId id) => MapIdToType[id];
        public static GamePacketId FromType(Type type) => MapTypeToId[type];
    }
}
