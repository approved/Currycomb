using System;

namespace Currycomb.Common.Network.Game.Packets.Types.Command
{
    public enum CommandSuggestionsType
    {
        MinecraftAskServer, // Sends the Tab-Complete packet to the server to request tab completions.
        MinecraftAllRecipes,
        MinecraftAvailableSounds,
        MinecraftSummonableEntities,
    }

    public static class SuggestionsTypeExt
    {
        public static string ToString(this CommandSuggestionsType type) => type switch
        {
            CommandSuggestionsType.MinecraftAskServer => "minecraft:ask_server",
            CommandSuggestionsType.MinecraftAllRecipes => "minecraft:all_recipes",
            CommandSuggestionsType.MinecraftAvailableSounds => "minecraft:available_sounds",
            CommandSuggestionsType.MinecraftSummonableEntities => "minecraft:summonable_entities",
            _ => throw new ArgumentOutOfRangeException(nameof(type), type, null),
        };

        public static CommandSuggestionsType FromString(this string type) => type switch
        {
            "minecraft:ask_server" => CommandSuggestionsType.MinecraftAskServer,
            "minecraft:all_recipes" => CommandSuggestionsType.MinecraftAllRecipes,
            "minecraft:available_sounds" => CommandSuggestionsType.MinecraftAvailableSounds,
            "minecraft:summonable_entities" => CommandSuggestionsType.MinecraftSummonableEntities,
            _ => throw new NotSupportedException($"Unsupported suggestions type: {type}"),
        };
    }
}
