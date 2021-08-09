using System;
using System.Linq;
using System.Collections.Generic;
using Currycomb.Common.Network.Minecraft.Packets;

namespace Currycomb.Common.Network.Minecraft
{
    public struct PacketIdMap<T> where T : IPacket
    {
        public static PacketId Id { get; } = PacketIdMap.FromType(typeof(T));
    }

    public static class PacketIdMap
    {
        static readonly Dictionary<PacketId, Type> MapIdToType = new()
        {
            { PacketId.LoginStart, typeof(PacketLoginStart) },
            { PacketId.LoginSuccess, typeof(PacketLoginSuccess) },

            { PacketId.Handshake, typeof(PacketHandshake) },

            { PacketId.Request, typeof(PacketRequest) },
            { PacketId.Response, typeof(PacketResponse) },

            { PacketId.Ping, typeof(PacketPing) },
            { PacketId.Pong, typeof(PacketPong) },
        };

        static readonly Dictionary<Type, PacketId> MapTypeToId = MapIdToType.ToDictionary(kvp => kvp.Value, kvp => kvp.Key);

        public static Type FromId(PacketId id) => MapIdToType[id];
        public static PacketId FromType(Type type) => MapTypeToId[type];
    }
}
