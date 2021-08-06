using Currycomb.Attributes;
using Serilog;
using System;

namespace Currycomb.Packets.Client
{
    [ClientPacket(typeof(C2SConnect), 0x00)]
    public class C2SConnect : IClientPacket
    {
        public C2SConnect() { }

        public void ProcessInformation(PacketReader packetReader)
        {
            int protocolVer = packetReader.Read7BitEncodedInt();
            string serverAddress = packetReader.ReadSizedString(255);
            ushort port = packetReader.ReadPort();
            int nextState = packetReader.Read7BitEncodedInt();

            if (nextState == 2)
            {
                int extPackLength = packetReader.Read7BitEncodedInt();
                int extPackId = packetReader.Read7BitEncodedInt();
                string name = packetReader.ReadSizedString(16);
                Log.Information($"{name} trying to connect to {serverAddress}:{port} with protocol version: {protocolVer}");
            }
        }
    }
}
