using Currycomb.Packets.Client;
using System;

namespace Currycomb.Attributes
{
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = false, Inherited = false)]
    public sealed class ClientPacketAttribute : Attribute
    {
        public Type ClassType;
        public int[] PacketIDs;

        public ClientPacketAttribute(Type classType, params int[] ids)
        {
            if (!typeof(IClientPacket).IsAssignableFrom(classType))
            {
                throw new ArgumentException($"Can not assign type '{classType.Name}' to '${nameof(IClientPacket)}'");
            }

            this.ClassType = classType;
            this.PacketIDs = ids;
        }
    }
}
