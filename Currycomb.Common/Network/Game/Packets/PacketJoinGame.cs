using Currycomb.Common.Game;
using System.IO;
using System.Threading.Tasks;

namespace Currycomb.Common.Network.Game.Packets
{
    public record PacketJoinGame(
        int EntityID,
        bool IsHardcore,
        GameMode GameMode,
        GameMode PreviousGameMode,
        string[] WorldNames,
        // DimensionCodec,
        // Dimension
        string SpawnWorldIdentifier,
        long WorldSeed,
        int RenderDistance,
        bool ReducedDebugInfo,
        bool EnableRespawnScreen,
        bool IsDebug,
        bool IsFlat) : IGamePacket
    {
        public void Write(BinaryWriter writer)
        {
            writer.Write(EntityID);
            writer.Write(IsHardcore);
            writer.Write(GameMode.AsByte());
            writer.Write(PreviousGameMode.AsByte());
            writer.Write7BitEncodedInt(WorldNames.Length);
            foreach (string worldName in WorldNames)
            {
                writer.Write(worldName);
            }

            writer.Write((byte)0x0A);
            writer.Write((byte)0x00);
            writer.Write((byte)0x00);
            writer.Write((byte)0x00);

            writer.Write((byte)0x0A);
            writer.Write((byte)0x00);
            writer.Write((byte)0x00);
            writer.Write((byte)0x00);

            writer.Write(SpawnWorldIdentifier);
            writer.Write(WorldSeed);
            writer.Write7BitEncodedInt(0);
            writer.Write7BitEncodedInt(RenderDistance);
            writer.Write(ReducedDebugInfo);
            writer.Write(EnableRespawnScreen);
            writer.Write(IsDebug);
            writer.Write(IsFlat);
        }
    }
}
