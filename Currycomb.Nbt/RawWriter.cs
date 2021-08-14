using System.IO;
using System.Runtime.CompilerServices;
using System.Text;

namespace Currycomb.Nbt
{
    public struct RawWriter
    {
        private readonly BinaryWriter _writer;

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        internal RawWriter(BinaryWriter writer) => _writer = writer;

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private void WriteRawString(string str)
        {
            byte[] bytes = Encoding.UTF8.GetBytes(str);
            WriteRawLe16((ushort)bytes.Length);
            _writer.Write(bytes);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private void WriteRawEmptyString() => _writer.Write((short)0);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private void WriteRawType(TagType type) => _writer.Write((byte)type);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void WriteHeader(TagType type)
        {
            WriteRawType(type);
            WriteRawEmptyString();
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void WriteHeader(TagType type, string name)
        {
            WriteRawType(type);
            WriteRawString(name);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private void WriteRawLe16(ushort value) => _writer.Write((ushort)(value << 8 | value >> 8));

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private void WriteRawLe32(uint value) => _writer.Write(
            (value & 0x000000FF) << 24 |
            (value & 0x0000FF00) << 8 |
            (value & 0x00FF0000) >> 8 |
            (value & 0xFF000000) >> 24);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private void WriteRawLe64(ulong value) => _writer.Write(
            (value & 0x00000000000000FF) << 56 |
            (value & 0x000000000000FF00) << 40 |
            (value & 0x0000000000FF0000) << 24 |
            (value & 0x00000000FF000000) << 8 |
            (value & 0x000000FF00000000) >> 8 |
            (value & 0x0000FF0000000000) >> 24 |
            (value & 0x00FF000000000000) >> 40 |
            (value & 0xFF00000000000000) >> 56);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private unsafe ulong DoubleToULongBits(double value) => *(ulong*)&value;

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private unsafe uint FloatToUIntBits(float value) => *(uint*)&value;

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public RawWriter WriteValue(bool value)
        {
            _writer.Write(value ? 1 : 0);
            return this;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public RawWriter WriteValue(byte value)
        {
            _writer.Write(value);
            return this;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public RawWriter WriteValue(short value)
        {
            WriteRawLe16((ushort)value);
            return this;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public RawWriter WriteValue(int value)
        {
            WriteRawLe32((uint)value);
            return this;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public RawWriter WriteValue(long value)
        {
            WriteRawLe64((ulong)value);
            return this;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public RawWriter WriteValue(float value)
        {
            WriteRawLe32(FloatToUIntBits(value));
            return this;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public RawWriter WriteValue(double value)
        {
            WriteRawLe64(DoubleToULongBits(value));
            return this;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public RawWriter WriteValue(byte[] value)
        {
            WriteRawLe32((uint)value.Length);
            _writer.Write(value);
            return this;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public RawWriter WriteValue(string value)
        {
            WriteRawString(value);
            return this;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public RawWriter BeginList(TagType type, int size)
        {
            WriteRawType(type);
            WriteRawLe32((uint)size);
            return this;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public RawWriter EndList() => this;

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public RawWriter BeginCompound() => this;

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public RawWriter EndCompound()
        {
            // Yes, this is supposed to not be a "proper" header.
            WriteRawType(TagType.End);
            return this;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public RawWriter WriteValue(int[] value)
        {
            WriteRawLe32((uint)value.Length);
            foreach (int v in value)
                WriteRawLe32((uint)v);
            return this;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public RawWriter WriteValue(long[] value)
        {
            WriteRawLe32((uint)value.Length);
            foreach (long v in value)
                WriteRawLe64((ulong)v);
            return this;
        }
    }
}
