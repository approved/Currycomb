using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace Currycomb.Common.Network.Game
{
    [JsonConverter(typeof(StringEnumConverter))]
    public enum State : uint
    {
        Handshake = 0 << 29,
        Status = 1u << 29,
        Login = 2u << 29,
        Play = 3u << 29,
    }

    public static class StateExt
    {
        public static State FromRaw(uint state) => (State)(state << 29);
        public static uint ToRaw(this State state) => (uint)state >> 29;
    }
}
