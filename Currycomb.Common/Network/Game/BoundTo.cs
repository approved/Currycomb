namespace Currycomb.Common.Network.Game
{
    public enum BoundTo : uint
    {
        Client = 1u << 31,
        Server = 0u << 31,
    }
}
