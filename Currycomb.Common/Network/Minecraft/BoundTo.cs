namespace Currycomb.Common.Network.Minecraft
{
    public enum BoundTo : uint
    {
        Client = 1u << 31,
        Server = 0u << 31,
    }
}
