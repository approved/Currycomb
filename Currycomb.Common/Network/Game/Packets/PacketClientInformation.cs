using Currycomb.Common.Extensions;
using System.IO;
using System.Threading.Tasks;

namespace Currycomb.Common.Network.Game.Packets
{
    public record PacketClientInformation(string Locale, byte ViewDistance, int ChatMode, bool EnableChatColors, byte ModelMask, int MainHand, bool EnableTextFiltering) : IGamePacket
    {
        public static async Task<PacketClientInformation> ReadAsync(Stream stream)
        {
            string locale = await stream.ReadStringAsync(16);
            byte viewDistance = (await stream.ReadBytesAsync(1))[0];
            int chatMode = await stream.Read7BitEncodedIntAsync();
            bool enableChatColors = (await stream.ReadBytesAsync(1))[0] != 0;
            byte modelMask = (await stream.ReadBytesAsync(1))[0];
            int mainHand = await stream.Read7BitEncodedIntAsync();
            bool enableTextFiltering = (await stream.ReadBytesAsync(1))[0] != 0;

            return new(locale, viewDistance, chatMode, enableChatColors, modelMask, mainHand, enableTextFiltering);
        }
    }
}
