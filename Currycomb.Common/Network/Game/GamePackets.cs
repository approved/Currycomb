using System;
using System.Linq;
using System.Collections.Generic;
using System.Reflection;
using System.IO;
using System.Linq.Expressions;

namespace Currycomb.Common.Network.Game
{
    public static class GamePackets<T>
        where T : IGamePacket
    {
        public static readonly GamePacketId Id = GamePackets.Meta<T>.Id;
        public static readonly Func<BinaryReader, T> Read = GamePackets.Meta<T>.Read;
    }

    public static class GamePackets
    {
        static readonly Dictionary<GamePacketId, Type> MapIdToType = new();
        static readonly Dictionary<Type, GamePacketId> MapTypeToId = new();
        static readonly Dictionary<Type, ConstructorInfo> TypeCtor = new();
        static readonly Dictionary<Type, Func<BinaryReader, IGamePacket>> TypeRead = new();
        static readonly Dictionary<GamePacketId, Func<BinaryReader, IGamePacket>> IdRead = new();

        public static Type TypeFromId(GamePacketId id) => MapIdToType[id];
        public static GamePacketId IdFromType(Type type) => MapTypeToId[type];

        public static IGamePacket Read(GamePacketId id, BinaryReader reader) => IdRead[id](reader);

        static GamePackets()
        {
            Type[] ctorArgs = new[] { typeof(BinaryReader) };
            Assembly assembly = typeof(GamePackets).GetTypeInfo().Assembly;

            foreach (var type in assembly.GetTypes())
            {
                TypeInfo typeInfo = type.GetTypeInfo();

                if (typeInfo.GetCustomAttribute(typeof(GamePacketAttribute)) is GamePacketAttribute meta)
                {
                    if (!typeInfo.ImplementedInterfaces.Contains(typeof(IGamePacket)))
                        throw new Exception($"{type.FullName} must implement IGamePacket");

                    ConstructorInfo? ctor = type.GetConstructor(BindingFlags.CreateInstance | BindingFlags.Public | BindingFlags.Instance, null, ctorArgs, null);
                    if (ctor == null)
                        throw new Exception($"{type.FullName} must have a public constructor that takes a BinaryReader");

                    var input = Expression.Parameter(typeof(BinaryReader), "reader");
                    TypeRead[type] = IdRead[meta.PacketId] = Expression.Lambda<Func<BinaryReader, IGamePacket>>(Expression.Convert(Expression.New(ctor, new[] { input }), typeof(IGamePacket)), input).Compile();
                    TypeCtor[type] = ctor;

                    MapIdToType[meta.PacketId] = type;
                    MapTypeToId[type] = meta.PacketId;
                }
            }
        }

        public readonly struct Meta<T>
            where T : IGamePacket
        {
            public static readonly GamePacketId Id;
            public static readonly Func<BinaryReader, T> Read;

            static Meta()
            {
                Type t = typeof(T);

                Id = MapTypeToId[t];

                var input = Expression.Parameter(typeof(BinaryReader), "reader");
                Read = Expression.Lambda<Func<BinaryReader, T>>(Expression.New(TypeCtor[t], new[] { input }), input).Compile();
            }
        }
    }
}
