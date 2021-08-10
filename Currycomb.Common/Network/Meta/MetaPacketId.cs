namespace Currycomb.Common.Network.Meta
{
    public enum MetaPacketId : byte
    {
        SetState = 0,
        SetAesKey = 1,
    }

    public static class MetaPacketIdExt
    {
        public static MetaPacketId FromRaw(byte id) => (MetaPacketId)id;
        public static byte ToRaw(this MetaPacketId id) => (byte)id;
    }
}
