#pragma warning disable format // @formatter:off

using System;
using System.IO;
using System.Runtime.CompilerServices;

namespace Currycomb.Nbt
{
    public struct Cloak
    {
        public static readonly Cloak Empty = new Cloak();
    }

    public struct Writer
    {
        internal readonly RawWriter _raw;
        private Writer(RawWriter raw) => _raw = raw;

        [MethodImpl(MethodImplOptions.AggressiveInlining)] public static Writer ToBinaryWriter(BinaryWriter bw) => new Writer(new(bw));
        [MethodImpl(MethodImplOptions.AggressiveInlining)] public static Writer ToStream(Stream s) => ToBinaryWriter(new(s));

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public CompoundWriter<Writer> Begin()
        {
            _raw.WriteHeader(TagType.Compound);
            _raw.BeginCompound();
            return new CompoundWriter<Writer>(_raw, this);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public CompoundWriter<Writer> Begin(string name)
        {
            _raw.WriteHeader(TagType.Compound, name);
            _raw.BeginCompound();
            return new CompoundWriter<Writer>(_raw, this);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)] public void Compound(Action<CompoundWriter<Writer>> action)              { var c = Begin();     action(c); c.End(); }
        [MethodImpl(MethodImplOptions.AggressiveInlining)] public void Compound(string name, Action<CompoundWriter<Writer>> action) { var c = Begin(name); action(c); c.End(); }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void Finish()
        {
            // TODO: Validation?
            // For now this exists to make it easier to ensure everything is as it should be.
            // If you can call finish, you're done.
        }
    }

    public struct CompoundWriter<T>
    {
        internal readonly RawWriter _raw;
        private readonly T _parent;

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        internal CompoundWriter(RawWriter writer, T parent)
        {
            _raw = writer;
            _parent = parent;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)] public T End() { _raw.EndCompound(); return _parent; }

        [MethodImpl(MethodImplOptions.AggressiveInlining)] public CompoundWriter<T> Write(byte value)              { _raw.WriteHeader(TagType.Byte);       _raw.WriteValue(value); return this; }
        [MethodImpl(MethodImplOptions.AggressiveInlining)] public CompoundWriter<T> Write(string name, byte value) { _raw.WriteHeader(TagType.Byte, name); _raw.WriteValue(value); return this; }

        [MethodImpl(MethodImplOptions.AggressiveInlining)] public CompoundWriter<T> Write(short value)              { _raw.WriteHeader(TagType.Short);       _raw.WriteValue(value); return this; }
        [MethodImpl(MethodImplOptions.AggressiveInlining)] public CompoundWriter<T> Write(string name, short value) { _raw.WriteHeader(TagType.Short, name); _raw.WriteValue(value); return this; }

        [MethodImpl(MethodImplOptions.AggressiveInlining)] public CompoundWriter<T> Write(int value)              { _raw.WriteHeader(TagType.Int);       _raw.WriteValue(value); return this; }
        [MethodImpl(MethodImplOptions.AggressiveInlining)] public CompoundWriter<T> Write(string name, int value) { _raw.WriteHeader(TagType.Int, name); _raw.WriteValue(value); return this; }
        
        [MethodImpl(MethodImplOptions.AggressiveInlining)] public CompoundWriter<T> Write(long value)              { _raw.WriteHeader(TagType.Long);       _raw.WriteValue(value); return this; }
        [MethodImpl(MethodImplOptions.AggressiveInlining)] public CompoundWriter<T> Write(string name, long value) { _raw.WriteHeader(TagType.Long, name); _raw.WriteValue(value); return this; }

        [MethodImpl(MethodImplOptions.AggressiveInlining)] public CompoundWriter<T> Write(float value)              { _raw.WriteHeader(TagType.Float);       _raw.WriteValue(value); return this; }
        [MethodImpl(MethodImplOptions.AggressiveInlining)] public CompoundWriter<T> Write(string name, float value) { _raw.WriteHeader(TagType.Float, name); _raw.WriteValue(value); return this; }

        [MethodImpl(MethodImplOptions.AggressiveInlining)] public CompoundWriter<T> Write(double value)              { _raw.WriteHeader(TagType.Double);       _raw.WriteValue(value); return this; }
        [MethodImpl(MethodImplOptions.AggressiveInlining)] public CompoundWriter<T> Write(string name, double value) { _raw.WriteHeader(TagType.Double, name); _raw.WriteValue(value); return this; }

        [MethodImpl(MethodImplOptions.AggressiveInlining)] public CompoundWriter<T> Write(byte[] value)              { _raw.WriteHeader(TagType.ByteArray);       _raw.WriteValue(value); return this; }
        [MethodImpl(MethodImplOptions.AggressiveInlining)] public CompoundWriter<T> Write(string name, byte[] value) { _raw.WriteHeader(TagType.ByteArray, name); _raw.WriteValue(value); return this; }

        [MethodImpl(MethodImplOptions.AggressiveInlining)] public CompoundWriter<T> Write(string value)              { _raw.WriteHeader(TagType.String);       _raw.WriteValue(value); return this; }
        [MethodImpl(MethodImplOptions.AggressiveInlining)] public CompoundWriter<T> Write(string name, string value) { _raw.WriteHeader(TagType.String, name); _raw.WriteValue(value); return this; }

        [MethodImpl(MethodImplOptions.AggressiveInlining)] public CompoundWriter<T> Write(int[] value)              { _raw.WriteHeader(TagType.IntArray);       _raw.WriteValue(value); return this; }
        [MethodImpl(MethodImplOptions.AggressiveInlining)] public CompoundWriter<T> Write(string name, int[] value) { _raw.WriteHeader(TagType.IntArray, name); _raw.WriteValue(value); return this; }

        [MethodImpl(MethodImplOptions.AggressiveInlining)] public CompoundWriter<T> Write(long[] value)              { _raw.WriteHeader(TagType.LongArray);       _raw.WriteValue(value); return this; }
        [MethodImpl(MethodImplOptions.AggressiveInlining)] public CompoundWriter<T> Write(string name, long[] value) { _raw.WriteHeader(TagType.LongArray, name); _raw.WriteValue(value); return this; }

        [MethodImpl(MethodImplOptions.AggressiveInlining)] public CompoundWriter<CompoundWriter<T>> Compound()            { _raw.WriteHeader(TagType.Compound);        _raw.BeginCompound(); return new(_raw, this); }
        [MethodImpl(MethodImplOptions.AggressiveInlining)] public CompoundWriter<CompoundWriter<T>> Compound(string name) { _raw.WriteHeader(TagType.Compound, name);  _raw.BeginCompound(); return new(_raw, this); }

        [MethodImpl(MethodImplOptions.AggressiveInlining)] public ListWriter<CompoundWriter<T>>.Bytes ListByte(int count)              { _raw.WriteHeader(TagType.List);       _raw.BeginList(TagType.Byte, count); return new(_raw, this, count); }
        [MethodImpl(MethodImplOptions.AggressiveInlining)] public ListWriter<CompoundWriter<T>>.Bytes ListByte(string name, int count) { _raw.WriteHeader(TagType.List, name); _raw.BeginList(TagType.Byte, count); return new(_raw, this, count); }

        [MethodImpl(MethodImplOptions.AggressiveInlining)] public ListWriter<CompoundWriter<T>>.Shorts ListShort(int count)              { _raw.WriteHeader(TagType.List);       _raw.BeginList(TagType.Short, count); return new(_raw, this, count); }
        [MethodImpl(MethodImplOptions.AggressiveInlining)] public ListWriter<CompoundWriter<T>>.Shorts ListShort(string name, int count) { _raw.WriteHeader(TagType.List, name); _raw.BeginList(TagType.Short, count); return new(_raw, this, count); }

        [MethodImpl(MethodImplOptions.AggressiveInlining)] public ListWriter<CompoundWriter<T>>.Ints ListInt(int count)              { _raw.WriteHeader(TagType.List);       _raw.BeginList(TagType.Int, count); return new(_raw, this, count); }
        [MethodImpl(MethodImplOptions.AggressiveInlining)] public ListWriter<CompoundWriter<T>>.Ints ListInt(string name, int count) { _raw.WriteHeader(TagType.List, name); _raw.BeginList(TagType.Int, count); return new(_raw, this, count); }

        [MethodImpl(MethodImplOptions.AggressiveInlining)] public ListWriter<CompoundWriter<T>>.Longs ListLong(int count)              { _raw.WriteHeader(TagType.List);       _raw.BeginList(TagType.Long, count); return new(_raw, this, count); }
        [MethodImpl(MethodImplOptions.AggressiveInlining)] public ListWriter<CompoundWriter<T>>.Longs ListLong(string name, int count) { _raw.WriteHeader(TagType.List, name); _raw.BeginList(TagType.Long, count); return new(_raw, this, count); }

        [MethodImpl(MethodImplOptions.AggressiveInlining)] public ListWriter<CompoundWriter<T>>.Floats ListFloat(int count)              { _raw.WriteHeader(TagType.List);       _raw.BeginList(TagType.Float, count); return new(_raw, this, count); }
        [MethodImpl(MethodImplOptions.AggressiveInlining)] public ListWriter<CompoundWriter<T>>.Floats ListFloat(string name, int count) { _raw.WriteHeader(TagType.List, name); _raw.BeginList(TagType.Float, count); return new(_raw, this, count); }

        [MethodImpl(MethodImplOptions.AggressiveInlining)] public ListWriter<CompoundWriter<T>>.Doubles ListDouble(int count)              { _raw.WriteHeader(TagType.List);       _raw.BeginList(TagType.Double, count); return new(_raw, this, count); }
        [MethodImpl(MethodImplOptions.AggressiveInlining)] public ListWriter<CompoundWriter<T>>.Doubles ListDouble(string name, int count) { _raw.WriteHeader(TagType.List, name); _raw.BeginList(TagType.Double, count); return new(_raw, this, count); }

        [MethodImpl(MethodImplOptions.AggressiveInlining)] public ListWriter<CompoundWriter<T>>.Strings ListString(int count)              { _raw.WriteHeader(TagType.List);       _raw.BeginList(TagType.String, count); return new(_raw, this, count); }
        [MethodImpl(MethodImplOptions.AggressiveInlining)] public ListWriter<CompoundWriter<T>>.Strings ListString(string name, int count) { _raw.WriteHeader(TagType.List, name); _raw.BeginList(TagType.String, count); return new(_raw, this, count); }

        [MethodImpl(MethodImplOptions.AggressiveInlining)] public ListWriter<CompoundWriter<T>>.Lists ListList(int count)              { _raw.WriteHeader(TagType.List);       _raw.BeginList(TagType.List, count); return new(_raw, this, count); }
        [MethodImpl(MethodImplOptions.AggressiveInlining)] public ListWriter<CompoundWriter<T>>.Lists ListList(string name, int count) { _raw.WriteHeader(TagType.List, name); _raw.BeginList(TagType.List, count); return new(_raw, this, count); }

        [MethodImpl(MethodImplOptions.AggressiveInlining)] public ListWriter<CompoundWriter<T>>.Compounds ListCompound(int count)              { _raw.WriteHeader(TagType.List);       _raw.BeginList(TagType.Compound, count); return new(_raw, this, count); }
        [MethodImpl(MethodImplOptions.AggressiveInlining)] public ListWriter<CompoundWriter<T>>.Compounds ListCompound(string name, int count) { _raw.WriteHeader(TagType.List, name); _raw.BeginList(TagType.Compound, count); return new(_raw, this, count); }

        [MethodImpl(MethodImplOptions.AggressiveInlining)] public ListWriter<CompoundWriter<T>>.ByteArrays ListByteArray(int count)              { _raw.WriteHeader(TagType.List);       _raw.BeginList(TagType.ByteArray, count); return new(_raw, this, count); }
        [MethodImpl(MethodImplOptions.AggressiveInlining)] public ListWriter<CompoundWriter<T>>.ByteArrays ListByteArray(string name, int count) { _raw.WriteHeader(TagType.List, name); _raw.BeginList(TagType.ByteArray, count); return new(_raw, this, count); }

        [MethodImpl(MethodImplOptions.AggressiveInlining)] public ListWriter<CompoundWriter<T>>.IntArrays ListIntArray(int count)              { _raw.WriteHeader(TagType.List);       _raw.BeginList(TagType.IntArray, count); return new(_raw, this, count); }
        [MethodImpl(MethodImplOptions.AggressiveInlining)] public ListWriter<CompoundWriter<T>>.IntArrays ListIntArray(string name, int count) { _raw.WriteHeader(TagType.List, name); _raw.BeginList(TagType.IntArray, count); return new(_raw, this, count); }

        [MethodImpl(MethodImplOptions.AggressiveInlining)] public ListWriter<CompoundWriter<T>>.LongArrays ListLongArray(int count)              { _raw.WriteHeader(TagType.List);       _raw.BeginList(TagType.LongArray, count); return new(_raw, this, count); }
        [MethodImpl(MethodImplOptions.AggressiveInlining)] public ListWriter<CompoundWriter<T>>.LongArrays ListLongArray(string name, int count) { _raw.WriteHeader(TagType.List, name); _raw.BeginList(TagType.LongArray, count); return new(_raw, this, count); }

        [MethodImpl(MethodImplOptions.AggressiveInlining)] public CompoundWriter<T> Compound(Action<CompoundWriter<CompoundWriter<T>>> action)              { var c = Compound();     action(c); return c.End(); }
        [MethodImpl(MethodImplOptions.AggressiveInlining)] public CompoundWriter<T> Compound(string name, Action<CompoundWriter<CompoundWriter<T>>> action) { var c = Compound(name); action(c); return c.End(); }

        [MethodImpl(MethodImplOptions.AggressiveInlining)] public CompoundWriter<T> ListShort(int count, Action<ListWriter<CompoundWriter<T>>.Shorts> action)              { var l = ListShort(count);       action(l); return l.End(); }
        [MethodImpl(MethodImplOptions.AggressiveInlining)] public CompoundWriter<T> ListShort(string name, int count, Action<ListWriter<CompoundWriter<T>>.Shorts> action) { var l = ListShort(name, count); action(l); return l.End(); }

        [MethodImpl(MethodImplOptions.AggressiveInlining)] public CompoundWriter<T> ListInt(int count, Action<ListWriter<CompoundWriter<T>>.Ints> action)              { var l = ListInt(count);       action(l); return l.End(); }
        [MethodImpl(MethodImplOptions.AggressiveInlining)] public CompoundWriter<T> ListInt(string name, int count, Action<ListWriter<CompoundWriter<T>>.Ints> action) { var l = ListInt(name, count); action(l); return l.End(); }

        [MethodImpl(MethodImplOptions.AggressiveInlining)] public CompoundWriter<T> ListLong(int count, Action<ListWriter<CompoundWriter<T>>.Longs> action)              { var l = ListLong(count);       action(l); return l.End(); }
        [MethodImpl(MethodImplOptions.AggressiveInlining)] public CompoundWriter<T> ListLong(string name, int count, Action<ListWriter<CompoundWriter<T>>.Longs> action) { var l = ListLong(name, count); action(l); return l.End(); }

        [MethodImpl(MethodImplOptions.AggressiveInlining)] public CompoundWriter<T> ListFloat(int count, Action<ListWriter<CompoundWriter<T>>.Floats> action)              { var l = ListFloat(count);       action(l); return l.End(); }
        [MethodImpl(MethodImplOptions.AggressiveInlining)] public CompoundWriter<T> ListFloat(string name, int count, Action<ListWriter<CompoundWriter<T>>.Floats> action) { var l = ListFloat(name, count); action(l); return l.End(); }

        [MethodImpl(MethodImplOptions.AggressiveInlining)] public CompoundWriter<T> ListDouble(int count, Action<ListWriter<CompoundWriter<T>>.Doubles> action)              { var l = ListDouble(count);       action(l); return l.End(); }
        [MethodImpl(MethodImplOptions.AggressiveInlining)] public CompoundWriter<T> ListDouble(string name, int count, Action<ListWriter<CompoundWriter<T>>.Doubles> action) { var l = ListDouble(name, count); action(l); return l.End(); }

        [MethodImpl(MethodImplOptions.AggressiveInlining)] public CompoundWriter<T> ListString(int count, Action<ListWriter<CompoundWriter<T>>.Strings> action)              { var l = ListString(count);       action(l); return l.End(); }
        [MethodImpl(MethodImplOptions.AggressiveInlining)] public CompoundWriter<T> ListString(string name, int count, Action<ListWriter<CompoundWriter<T>>.Strings> action) { var l = ListString(name, count); action(l); return l.End(); }

        [MethodImpl(MethodImplOptions.AggressiveInlining)] public CompoundWriter<T> ListList(int count, Action<ListWriter<CompoundWriter<T>>.Lists> action)              { var l = ListList(count);       action(l); return l.End(); }
        [MethodImpl(MethodImplOptions.AggressiveInlining)] public CompoundWriter<T> ListList(string name, int count, Action<ListWriter<CompoundWriter<T>>.Lists> action) { var l = ListList(name, count); action(l); return l.End(); }

        [MethodImpl(MethodImplOptions.AggressiveInlining)] public CompoundWriter<T> ListCompound(int count, Action<ListWriter<CompoundWriter<T>>.Compounds> action)              { var l = ListCompound(count);       action(l); return l.End(); }
        [MethodImpl(MethodImplOptions.AggressiveInlining)] public CompoundWriter<T> ListCompound(string name, int count, Action<ListWriter<CompoundWriter<T>>.Compounds> action) { var l = ListCompound(name, count); action(l); return l.End(); }

        [MethodImpl(MethodImplOptions.AggressiveInlining)] public CompoundWriter<T> ListByteArray(int count, Action<ListWriter<CompoundWriter<T>>.ByteArrays> action)              { var l = ListByteArray(count);       action(l); return l.End(); }
        [MethodImpl(MethodImplOptions.AggressiveInlining)] public CompoundWriter<T> ListByteArray(string name, int count, Action<ListWriter<CompoundWriter<T>>.ByteArrays> action) { var l = ListByteArray(name, count); action(l); return l.End(); }

        [MethodImpl(MethodImplOptions.AggressiveInlining)] public CompoundWriter<T> ListIntArray(int count, Action<ListWriter<CompoundWriter<T>>.IntArrays> action)              { var l = ListIntArray(count);       action(l); return l.End(); }
        [MethodImpl(MethodImplOptions.AggressiveInlining)] public CompoundWriter<T> ListIntArray(string name, int count, Action<ListWriter<CompoundWriter<T>>.IntArrays> action) { var l = ListIntArray(name, count); action(l); return l.End(); }

        [MethodImpl(MethodImplOptions.AggressiveInlining)] public CompoundWriter<T> ListLongArray(int count, Action<ListWriter<CompoundWriter<T>>.LongArrays> action)              { var l = ListLongArray(count);       action(l); return l.End(); }
        [MethodImpl(MethodImplOptions.AggressiveInlining)] public CompoundWriter<T> ListLongArray(string name, int count, Action<ListWriter<CompoundWriter<T>>.LongArrays> action) { var l = ListLongArray(name, count); action(l); return l.End(); }

        [MethodImpl(MethodImplOptions.AggressiveInlining)] public CompoundWriter<T> WithCloak(Action<CompoundWriter<Cloak>> action) { action(new CompoundWriter<Cloak>(_raw, Cloak.Empty)); return this; }
    }

    public class ListWriter<T>
    {
        internal readonly RawWriter _raw;
        protected readonly T _parent;
        protected int _count;

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        internal ListWriter(RawWriter writer, T parent, int count)
        {
            _raw = writer;
            _parent = parent;
            _count = count;
        }
        
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public T End()
        {
            if (_count > 0) throw new InvalidOperationException($"Not all values written (missing: {_count})");
            if (_count < 0) throw new InvalidOperationException($"Too many values written (extra: {-_count})");
            return _parent;
        }

        public class Bytes : ListWriter<T>
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)] internal Bytes(RawWriter writer, T parent, int count) : base(writer, parent, count) { }
            [MethodImpl(MethodImplOptions.AggressiveInlining)] public Bytes Write(byte value) { _count--; _raw.WriteValue(value); return this; }
        }
        
        public class Shorts : ListWriter<T>
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)] internal Shorts(RawWriter writer, T parent, int count) : base(writer, parent, count) { }
            [MethodImpl(MethodImplOptions.AggressiveInlining)] public Shorts Write(short value) { _count--; _raw.WriteValue(value); return this; }
        }

        public class Ints : ListWriter<T>
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)] internal Ints(RawWriter writer, T parent, int count) : base(writer, parent, count) { }
            [MethodImpl(MethodImplOptions.AggressiveInlining)] public Ints Write(int value) { _count--; _raw.WriteValue(value); return this; }
        }

        public class Strings : ListWriter<T>
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)] internal Strings(RawWriter writer, T parent, int count) : base(writer, parent, count) { }
            [MethodImpl(MethodImplOptions.AggressiveInlining)] public Strings Write(string value) { _count--; _raw.WriteValue(value); return this; }
        }

        public class Longs : ListWriter<T>
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)] internal Longs(RawWriter writer, T parent, int count) : base(writer, parent, count) { }
            [MethodImpl(MethodImplOptions.AggressiveInlining)] public Longs Write(long value) { _count--; _raw.WriteValue(value); return this; }
        }

        public class Floats : ListWriter<T>
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)] internal Floats(RawWriter writer, T parent, int count) : base(writer, parent, count) { }
            [MethodImpl(MethodImplOptions.AggressiveInlining)] public Floats Write(float value) { _count--; _raw.WriteValue(value); return this; }
        }

        public class Doubles : ListWriter<T>
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)] internal Doubles(RawWriter writer, T parent, int count) : base(writer, parent, count) { }
            [MethodImpl(MethodImplOptions.AggressiveInlining)] public Doubles Write(double value) { _count--; _raw.WriteValue(value); return this; }
        }

        public class ByteArrays : ListWriter<T>
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)] internal ByteArrays(RawWriter writer, T parent, int count) : base(writer, parent, count) { }
            [MethodImpl(MethodImplOptions.AggressiveInlining)] public ByteArrays Write(byte[] value) { _count--; _raw.WriteValue(value); return this; }
        }

        public class IntArrays : ListWriter<T>
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)] internal IntArrays(RawWriter writer, T parent, int count) : base(writer, parent, count) { }
            [MethodImpl(MethodImplOptions.AggressiveInlining)] public IntArrays Write(int[] value) { _count--; _raw.WriteValue(value); return this; }
        }

        public class LongArrays : ListWriter<T>
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)] internal LongArrays(RawWriter writer, T parent, int count) : base(writer, parent, count) { }
            [MethodImpl(MethodImplOptions.AggressiveInlining)] public LongArrays Write(long[] value) { _count--; _raw.WriteValue(value); return this; }
        }

        public class Compounds : ListWriter<T> {
            [MethodImpl(MethodImplOptions.AggressiveInlining)] internal Compounds(RawWriter writer, T parent, int count) : base(writer, parent, count) { }
            [MethodImpl(MethodImplOptions.AggressiveInlining)] public CompoundWriter<Compounds> Compound() { _count--; _raw.BeginCompound(); return new(_raw, this); }
            [MethodImpl(MethodImplOptions.AggressiveInlining)] public Compounds Compound(Action<CompoundWriter<Compounds>> action) { var c = Compound(); action(c); return c.End(); }
        }

        public class Lists : ListWriter<T>
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)] internal Lists(RawWriter writer, T parent, int count) : base(writer, parent, count) { }
            
            [MethodImpl(MethodImplOptions.AggressiveInlining)] public ListWriter<Lists> ListByte(int count)      { _count--; _raw.BeginList(TagType.Byte, count);      return new(_raw, this, count); }
            [MethodImpl(MethodImplOptions.AggressiveInlining)] public ListWriter<Lists> ListShort(int count)     { _count--; _raw.BeginList(TagType.Short, count);     return new(_raw, this, count); }
            [MethodImpl(MethodImplOptions.AggressiveInlining)] public ListWriter<Lists> ListInt(int count)       { _count--; _raw.BeginList(TagType.Int, count);       return new(_raw, this, count); }
            [MethodImpl(MethodImplOptions.AggressiveInlining)] public ListWriter<Lists> ListLong(int count)      { _count--; _raw.BeginList(TagType.Long, count);      return new(_raw, this, count); }
            [MethodImpl(MethodImplOptions.AggressiveInlining)] public ListWriter<Lists> ListFloat(int count)     { _count--; _raw.BeginList(TagType.Float, count);     return new(_raw, this, count); }
            [MethodImpl(MethodImplOptions.AggressiveInlining)] public ListWriter<Lists> ListDouble(int count)    { _count--; _raw.BeginList(TagType.Double, count);    return new(_raw, this, count); }
            [MethodImpl(MethodImplOptions.AggressiveInlining)] public ListWriter<Lists> ListString(int count)    { _count--; _raw.BeginList(TagType.String, count);    return new(_raw, this, count); }
            [MethodImpl(MethodImplOptions.AggressiveInlining)] public ListWriter<Lists> ListList(int count)      { _count--; _raw.BeginList(TagType.List, count);      return new(_raw, this, count); }
            [MethodImpl(MethodImplOptions.AggressiveInlining)] public ListWriter<Lists> ListCompound(int count)  { _count--; _raw.BeginList(TagType.Compound, count);  return new(_raw, this, count); }
            [MethodImpl(MethodImplOptions.AggressiveInlining)] public ListWriter<Lists> ListByteArray(int count) { _count--; _raw.BeginList(TagType.ByteArray, count); return new(_raw, this, count); }
            [MethodImpl(MethodImplOptions.AggressiveInlining)] public ListWriter<Lists> ListIntArray(int count)  { _count--; _raw.BeginList(TagType.IntArray, count);  return new(_raw, this, count); }
            [MethodImpl(MethodImplOptions.AggressiveInlining)] public ListWriter<Lists> ListLongArray(int count) { _count--; _raw.BeginList(TagType.LongArray, count); return new(_raw, this, count); }

            [MethodImpl(MethodImplOptions.AggressiveInlining)] public Lists ListByte(int count, Action<ListWriter<Lists>> action)      { var l = ListByte(count);      action(l); return l.End(); }
            [MethodImpl(MethodImplOptions.AggressiveInlining)] public Lists ListShort(int count, Action<ListWriter<Lists>> action)     { var l = ListShort(count);     action(l); return l.End(); }
            [MethodImpl(MethodImplOptions.AggressiveInlining)] public Lists ListInt(int count, Action<ListWriter<Lists>> action)       { var l = ListInt(count);       action(l); return l.End(); }
            [MethodImpl(MethodImplOptions.AggressiveInlining)] public Lists ListLong(int count, Action<ListWriter<Lists>> action)      { var l = ListLong(count);      action(l); return l.End(); }
            [MethodImpl(MethodImplOptions.AggressiveInlining)] public Lists ListFloat(int count, Action<ListWriter<Lists>> action)     { var l = ListFloat(count);     action(l); return l.End(); }
            [MethodImpl(MethodImplOptions.AggressiveInlining)] public Lists ListDouble(int count, Action<ListWriter<Lists>> action)    { var l = ListDouble(count);    action(l); return l.End(); }
            [MethodImpl(MethodImplOptions.AggressiveInlining)] public Lists ListString(int count, Action<ListWriter<Lists>> action)    { var l = ListString(count);    action(l); return l.End(); }
            [MethodImpl(MethodImplOptions.AggressiveInlining)] public Lists ListList(int count, Action<ListWriter<Lists>> action)      { var l = ListList(count);      action(l); return l.End(); }
            [MethodImpl(MethodImplOptions.AggressiveInlining)] public Lists ListCompound(int count, Action<ListWriter<Lists>> action)  { var l = ListCompound(count);  action(l); return l.End(); }
            [MethodImpl(MethodImplOptions.AggressiveInlining)] public Lists ListByteArray(int count, Action<ListWriter<Lists>> action) { var l = ListByteArray(count); action(l); return l.End(); }
            [MethodImpl(MethodImplOptions.AggressiveInlining)] public Lists ListIntArray(int count, Action<ListWriter<Lists>> action)  { var l = ListIntArray(count);  action(l); return l.End(); }
            [MethodImpl(MethodImplOptions.AggressiveInlining)] public Lists ListLongArray(int count, Action<ListWriter<Lists>> action) { var l = ListLongArray(count); action(l); return l.End(); }
        }
    }
}

#pragma warning restore format // @formatter:on