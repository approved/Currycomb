using System;
using System.IO;
using Currycomb.Common.Network.Game;

namespace Currycomb.Common.Network.Meta.Packets
{
    // This is a special packet and SHOULD always be the first packet sent by a service.
    // If an acknowledgement is requested for this packet it will be sent when the service is put in use, not when the packet is first received.
    public record PacketAnnounce(
        // This only serves as a display name.
        string Name,
        // This is a per-service ID, it can be set to anything but only one service per ID will be in-use, others will be queued.
        Guid ServiceId,
        // Filter for which packets should be sent to the service.
        MetaPacketId[] SupportedMetaPacketIds,
        GamePacketId[] SupportedGamePacketIds
    ) : IMetaPacket
    {
        public static PacketAnnounce Read(BinaryReader reader)
        {
            string friendlyName = reader.ReadString();
            Guid serviceId = new Guid(reader.ReadBytes(16));

            MetaPacketId[] supportedMetaPacketIds = new MetaPacketId[reader.Read7BitEncodedInt()];
            for (var i = 0; i < supportedMetaPacketIds.Length; i++)
                supportedMetaPacketIds[i] = (MetaPacketId)reader.ReadByte();

            GamePacketId[] supportedGamePacketIds = new GamePacketId[reader.Read7BitEncodedInt()];
            for (var i = 0; i < supportedGamePacketIds.Length; i++)
                supportedGamePacketIds[i] = (GamePacketId)reader.ReadUInt32();

            return new(
                friendlyName,
                serviceId,
                supportedMetaPacketIds,
                supportedGamePacketIds
            );
        }

        public void Write(BinaryWriter writer)
        {
            writer.Write(Name);
            writer.Write(ServiceId.ToByteArray());

            writer.Write7BitEncodedInt(SupportedMetaPacketIds?.Length ?? 0);
            if (SupportedMetaPacketIds != null)
                foreach (var id in SupportedMetaPacketIds)
                    writer.Write((byte)id);

            writer.Write7BitEncodedInt(SupportedGamePacketIds?.Length ?? 0);
            if (SupportedGamePacketIds != null)
                foreach (var id in SupportedGamePacketIds)
                    writer.Write((uint)id);
        }
    }
}
