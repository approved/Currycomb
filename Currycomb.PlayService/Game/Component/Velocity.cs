namespace Currycomb.PlayService.Game.Component
{
    public struct Velocity
    {
        // Velocity is believed to be in units of 1/8000 of a block per server tick (50ms); for example, -1343 would move (-1343 / 8000) = −0.167875 blocks per tick (or −3,3575 blocks per second). 

        public short X;
        public short Y;
        public short Z;
    }
}