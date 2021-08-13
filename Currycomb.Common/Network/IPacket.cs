using System;
using System.IO;

namespace Currycomb.Common.Network
{
    public interface IPacket
    {
        public void Write(BinaryWriter writer)
            => throw new NotImplementedException("Attempted to write packet without WriteAsync implemented.");
    }
}
