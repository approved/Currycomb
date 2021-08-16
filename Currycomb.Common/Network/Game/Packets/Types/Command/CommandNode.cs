using System.IO;
using Currycomb.Common.Network.Game.Packets.Types.Command.Properties;

namespace Currycomb.Common.Network.Game.Packets.Types.Command
{
    public readonly struct CommandNode
    {
        public readonly CommandFlags Flags;
        public readonly int[] Children;
        public readonly int? Redirect; // Only present if HasRedirect is set. Index of redirect node.
        public readonly string? Name; // Only for `argument` and `literal` nodes. Max length 32767.
        public readonly CommandParser? Parser; // Only for `argument` nodes.
        // TODO: Make CommandParser a struct with properties that points to an enum instead to prevent sending invalid properties for a parser.
        public readonly ICommandProperties? Properties; // Only for `argument` nodes.
        public readonly CommandSuggestionsType? SuggestionsType; // Only if Flags.HasSuggestions is set.

        public CommandNode(BinaryReader reader)
        {
            Flags = (CommandFlags)reader.ReadByte();
            Children = new int[reader.Read7BitEncodedInt()];
            for (int i = 0; i < Children.Length; i++)
                Children[i] = reader.Read7BitEncodedInt();

            Redirect = null;
            if ((Flags & CommandFlags.HasRedirect) != 0)
                Redirect = reader.Read7BitEncodedInt();

            Name = null;
            if ((Flags & (CommandFlags.NodeTypeArgument | CommandFlags.NodeTypeLiteral)) != 0)
                Name = reader.ReadString(); // Max length: 32767.

            Parser = null;
            Properties = null;
            if ((Flags & CommandFlags.NodeTypeArgument) != 0)
            {
                Parser = CommandParserExt.FromString(reader.ReadString());
                Properties = Parser?.ReadProperties(reader);
            }

            SuggestionsType = null;
            if ((Flags & CommandFlags.HasSuggestions) != 0)
                SuggestionsType = SuggestionsTypeExt.FromString(reader.ReadString());
        }

        public void Write(BinaryWriter writer)
        {
            writer.Write((byte)Flags);
            writer.Write7BitEncodedInt(Children.Length);
            for (int i = 0; i < Children.Length; i++)
                writer.Write7BitEncodedInt(Children[i]);

            if ((Flags & CommandFlags.HasRedirect) != 0 && Redirect != null)
                writer.Write7BitEncodedInt(Redirect.Value);

            if ((Flags & (CommandFlags.NodeTypeArgument | CommandFlags.NodeTypeLiteral)) != 0 && Name != null)
                writer.Write(Name);

            if ((Flags & CommandFlags.NodeTypeArgument) != 0 && Parser != null && Properties != null && Parser.ToString() is string parser)
            {
                writer.Write(parser);
                Properties.Write(writer);
            }

            if ((Flags & CommandFlags.HasSuggestions) != 0 && SuggestionsType != null && SuggestionsType.ToString() is string type)
                writer.Write(type);
        }
    }


}