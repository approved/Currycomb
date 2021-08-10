using System.IO;
using Currycomb.Common.Network.Game;

namespace Currycomb.Common.Network.Meta.Packets
{
    public record PacketSetState(State State) : IMetaPacket
    {
        public static PacketSetState Read(BinaryReader reader) => new(StateExt.FromRaw(reader.ReadUInt32()));
        public void Write(BinaryWriter writer) => writer.Write(State.ToRaw());
    }
}
