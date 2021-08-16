namespace Currycomb.Common.Network.Game.Packets.Types.Command
{
    public enum CommandFlags : byte
    {
        NodeTypeRoot = 0,
        NodeTypeLiteral = 1,
        NodeTypeArgument = 2,

        IsExecutable = 4,    // Set if the node stack to this point constitutes a valid command. 
        HasRedirect = 8,     // Set if the node redirects to another node. 
        HasSuggestions = 16, // Only present for `argument` nodes. 
    }


}