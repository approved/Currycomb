namespace Currycomb.Common.Network.Minecraft
{
    public enum State : uint
    {
        Handshake = 0 << 30,
        Status = 1u << 30,
        Login = 2u << 30,
        Play = 3u << 30,
    }

    public static class StateExt
    {
        public static State FromRaw(uint state) => (State)(state << 30);
        public static uint ToRaw(this State state) => (uint)state >> 30;
    }
}
