using System;
using System.IO;
using System.Threading.Tasks;

namespace Currycomb.Common.Network.Minecraft
{
    public interface IPacket
    {
        public Task WriteAsync(Stream stream) => throw new NotImplementedException("Attempted to write packet without WriteAsync implemented.");
    }
}
