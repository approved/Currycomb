using System.IO;
using Currycomb.Common.Network.Game.Packets.Types.Command;

namespace Currycomb.Common.Network.Game.Packets
{
    [GamePacket(GamePacketId.CommandList)]
    public readonly struct PacketCommandList : IGamePacket
    {
        public readonly CommandNode[] Nodes;
        public readonly int RootIndex; // Index of the root node in the node array.

        public PacketCommandList(BinaryReader reader)
        {
            Nodes = new CommandNode[reader.Read7BitEncodedInt()];
            for (int i = 0; i < Nodes.Length; i++)
                Nodes[i] = new CommandNode(reader);

            RootIndex = reader.Read7BitEncodedInt();
        }

        public void Write(BinaryWriter writer)
        {
            writer.Write7BitEncodedInt(Nodes.Length);
            foreach (var node in Nodes)
                node.Write(writer);

            writer.Write7BitEncodedInt(RootIndex);
        }
    }
}
