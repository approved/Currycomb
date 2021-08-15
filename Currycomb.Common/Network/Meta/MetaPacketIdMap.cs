using System;
using System.Linq;
using System.Collections.Generic;
using Currycomb.Common.Network.Meta.Packets;

namespace Currycomb.Common.Network.Meta
{
    public struct MetaPacketIdMap<T> where T : IMetaPacket
    {
        public static MetaPacketId Id { get; } = MetaPacketIdMap.FromType(typeof(T));
    }

    public static class MetaPacketIdMap
    {
        static readonly Dictionary<MetaPacketId, Type> MapIdToType = new()
        {
            { MetaPacketId.Announce, typeof(PacketAnnounce) },
            { MetaPacketId.SetState, typeof(PacketSetState) },
            { MetaPacketId.SetAesKey, typeof(PacketSetAesKey) },
        };

        static readonly Dictionary<Type, MetaPacketId> MapTypeToId = MapIdToType.ToDictionary(kvp => kvp.Value, kvp => kvp.Key);

        public static Type FromId(MetaPacketId id) => MapIdToType[id];
        public static MetaPacketId FromType(Type type) => MapTypeToId[type];
    }
}
