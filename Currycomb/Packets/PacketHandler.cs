using Currycomb.Packets.Client;
using Serilog;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace Currycomb.Packets
{
    public static class PacketHandler
    {
        public static readonly Dictionary<int, Type> ClientPackets = new();


        // TODO: Move to configuration file
        private const int BufferSize = 0x1FFFFF;
        public static void ProcessRemoteData(TcpClient client)
        {
            int i = 0;
            using MemoryStream ms = new();
            byte[] bytes = new byte[BufferSize];
            NetworkStream stream = client.GetStream();
            while (client.Connected && i < client.Available && (i = stream.Read(bytes, 0, BufferSize)) > 0)
            {
                ms.Write(bytes, 0, i);
            }

            byte[] packetBuffer = ms.ToArray();

            if (packetBuffer.Length <= 1)
            {
                throw new ArgumentOutOfRangeException(nameof(packetBuffer));
            }

            using PacketReader pr = new(new MemoryStream(packetBuffer));
            int length = pr.Read7BitEncodedInt();
            if (length <= 1)
            {
                throw new ArgumentOutOfRangeException(nameof(length), $"Invalid length of {length}");
            }

            foreach (byte b in packetBuffer)
            {
                Console.Write($"0x{b:X2} ");
            }
            Console.WriteLine();

            int packetId = pr.Read7BitEncodedInt();

            if (ClientPackets.ContainsKey(packetId))
            {
                ((IClientPacket)Activator.CreateInstance(ClientPackets[packetId])!).ProcessInformation(pr);
            }
            else
            {
                Log.Warning($"Packet ID '{packetId}' not implemented");
            }
        }
    }
}
