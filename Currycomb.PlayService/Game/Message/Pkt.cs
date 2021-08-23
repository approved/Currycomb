using Currycomb.PlayService.Game.Component;

namespace Currycomb.PlayService.Game.Message
{
    public readonly struct Pkt<T>
    {
        public readonly ClientId Source;
        public readonly T Data;

        public Pkt(ClientId source, T packet)
        {
            Source = source;
            Data = packet;
        }
    }
}
