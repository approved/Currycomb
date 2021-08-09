using System;
using System.Linq;
using System.Collections.Generic;
using Currycomb.Common.Network.Minecraft.Packets;

namespace Currycomb.Common.Network.Minecraft
{
    public static class PacketIdMap
    {
        static readonly Dictionary<PacketId, Type> MapIdToType = new()
        {
            { PacketId.LoginStart, typeof(PacketLoginStart) },
            { PacketId.LoginSuccess, typeof(PacketLoginSuccess) },
            { PacketId.Handshake, typeof(PacketHandshake) },
            { PacketId.Request, typeof(PacketRequest) },
            { PacketId.Ping, typeof(PacketPing) },
        };

        static readonly Dictionary<Type, PacketId> MapTypeToId = MapIdToType.ToDictionary(kvp => kvp.Value, kvp => kvp.Key);

        public static Type FromId(PacketId id) => MapIdToType[id];
        public static PacketId FromType<T>() => MapTypeToId[typeof(T)];
    }
}
