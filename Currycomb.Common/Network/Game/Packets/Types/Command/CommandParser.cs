using System;
using System.IO;
using Currycomb.Common.Network.Game.Packets.Types.Command.Properties;

namespace Currycomb.Common.Network.Game.Packets.Types.Command
{
    public enum CommandParser
    {
        BrigadierBool,
        BrigadierDouble,
        BrigadierFloat,
        BrigadierInteger,
        BrigadierLong,
        BrigadierString,
        MinecraftEntity,
        MinecraftGameProfile,
        MinecraftBlockPos,
        MinecraftColumnPos,
        MinecraftVec3,
        MinecraftVec2,
        MinecraftBlockState,
        MinecraftBlockPredicate,
        MinecraftItemStack,
        MinecraftItemPredicate,
        MinecraftColor,
        MinecraftComponent,
        MinecraftMessage,
        MinecraftNbt,
        MinecraftNbtPath,
        MinecraftObjective,
        MinecraftObjectiveCriteria,
        MinecraftOperation,
        MinecraftParticle,
        MinecraftRotation,
        MinecraftAngle,
        MinecraftScoreboardSlot,
        MinecraftScoreHolder,
        MinecraftSwizzle,
        MinecraftTeam,
        MinecraftItemSlot,
        MinecraftResourceLocation,
        MinecraftMobEffect,
        MinecraftFunction,
        MinecraftEntityAnchor,
        MinecraftRange,
        MinecraftIntRange,
        MinecraftFloatRange,
        MinecraftItemEnchantment,
        MinecraftEntitySummon,
        MinecraftDimension,
        MinecraftUuid,
        MinecraftNbtTag,
        MinecraftNbtCompoundTag,
        MinecraftTime,
        ForgeModid,
        ForgeEnum,
    }

    public static class CommandParserExt
    {
        public static CommandParser? FromString(string str) => str switch
        {
            "brigadier:bool" => CommandParser.BrigadierBool,
            "brigadier:double" => CommandParser.BrigadierDouble,
            "brigadier:float" => CommandParser.BrigadierFloat,
            "brigadier:integer" => CommandParser.BrigadierInteger,
            "brigadier:long" => CommandParser.BrigadierLong,
            "brigadier:string" => CommandParser.BrigadierString,
            "minecraft:entity" => CommandParser.MinecraftEntity,
            "minecraft:game_profile" => CommandParser.MinecraftGameProfile,
            "minecraft:block_pos" => CommandParser.MinecraftBlockPos,
            "minecraft:column_pos" => CommandParser.MinecraftColumnPos,
            "minecraft:vec3" => CommandParser.MinecraftVec3,
            "minecraft:vec2" => CommandParser.MinecraftVec2,
            "minecraft:block_state" => CommandParser.MinecraftBlockState,
            "minecraft:block_predicate" => CommandParser.MinecraftBlockPredicate,
            "minecraft:item_stack" => CommandParser.MinecraftItemStack,
            "minecraft:item_predicate" => CommandParser.MinecraftItemPredicate,
            "minecraft:color" => CommandParser.MinecraftColor,
            "minecraft:component" => CommandParser.MinecraftComponent,
            "minecraft:message" => CommandParser.MinecraftMessage,
            "minecraft:nbt" => CommandParser.MinecraftNbt,
            "minecraft:nbt_path" => CommandParser.MinecraftNbtPath,
            "minecraft:objective" => CommandParser.MinecraftObjective,
            "minecraft:objective_criteria" => CommandParser.MinecraftObjectiveCriteria,
            "minecraft:operation" => CommandParser.MinecraftOperation,
            "minecraft:particle" => CommandParser.MinecraftParticle,
            "minecraft:rotation" => CommandParser.MinecraftRotation,
            "minecraft:angle" => CommandParser.MinecraftAngle,
            "minecraft:scoreboard_slot" => CommandParser.MinecraftScoreboardSlot,
            "minecraft:score_holder" => CommandParser.MinecraftScoreHolder,
            "minecraft:swizzle" => CommandParser.MinecraftSwizzle,
            "minecraft:team" => CommandParser.MinecraftTeam,
            "minecraft:item_slot" => CommandParser.MinecraftItemSlot,
            "minecraft:resource_location" => CommandParser.MinecraftResourceLocation,
            "minecraft:mob_effect" => CommandParser.MinecraftMobEffect,
            "minecraft:function" => CommandParser.MinecraftFunction,
            "minecraft:entity_anchor" => CommandParser.MinecraftEntityAnchor,
            "minecraft:range" => CommandParser.MinecraftRange,
            "minecraft:int_range" => CommandParser.MinecraftIntRange,
            "minecraft:float_range" => CommandParser.MinecraftFloatRange,
            "minecraft:item_enchantment" => CommandParser.MinecraftItemEnchantment,
            "minecraft:entity_summon" => CommandParser.MinecraftEntitySummon,
            "minecraft:dimension" => CommandParser.MinecraftDimension,
            "minecraft:uuid" => CommandParser.MinecraftUuid,
            "minecraft:nbt_tag" => CommandParser.MinecraftNbtTag,
            "minecraft:nbt_compound_tag" => CommandParser.MinecraftNbtCompoundTag,
            "minecraft:time" => CommandParser.MinecraftTime,
            "forge:modid" => CommandParser.ForgeModid,
            "forge:enum" => CommandParser.ForgeEnum,
            _ => throw new NotSupportedException($"Unsupported command parser: {str}"),
        };

        public static string ToString(CommandParser parser) => parser switch
        {
            CommandParser.BrigadierBool => "brigadier:bool",
            CommandParser.BrigadierDouble => "brigadier:double",
            CommandParser.BrigadierFloat => "brigadier:float",
            CommandParser.BrigadierInteger => "brigadier:integer",
            CommandParser.BrigadierLong => "brigadier:long",
            CommandParser.BrigadierString => "brigadier:string",
            CommandParser.MinecraftEntity => "minecraft:entity",
            CommandParser.MinecraftGameProfile => "minecraft:game_profile",
            CommandParser.MinecraftBlockPos => "minecraft:block_pos",
            CommandParser.MinecraftColumnPos => "minecraft:column_pos",
            CommandParser.MinecraftVec3 => "minecraft:vec3",
            CommandParser.MinecraftVec2 => "minecraft:vec2",
            CommandParser.MinecraftBlockState => "minecraft:block_state",
            CommandParser.MinecraftBlockPredicate => "minecraft:block_predicate",
            CommandParser.MinecraftItemStack => "minecraft:item_stack",
            CommandParser.MinecraftItemPredicate => "minecraft:item_predicate",
            CommandParser.MinecraftColor => "minecraft:color",
            CommandParser.MinecraftComponent => "minecraft:component",
            CommandParser.MinecraftMessage => "minecraft:message",
            CommandParser.MinecraftNbt => "minecraft:nbt",
            CommandParser.MinecraftNbtPath => "minecraft:nbt_path",
            CommandParser.MinecraftObjective => "minecraft:objective",
            CommandParser.MinecraftObjectiveCriteria => "minecraft:objective_criteria",
            CommandParser.MinecraftOperation => "minecraft:operation",
            CommandParser.MinecraftParticle => "minecraft:particle",
            CommandParser.MinecraftRotation => "minecraft:rotation",
            CommandParser.MinecraftAngle => "minecraft:angle",
            CommandParser.MinecraftScoreboardSlot => "minecraft:scoreboard_slot",
            CommandParser.MinecraftScoreHolder => "minecraft:score_holder",
            CommandParser.MinecraftSwizzle => "minecraft:swizzle",
            CommandParser.MinecraftTeam => "minecraft:team",
            CommandParser.MinecraftItemSlot => "minecraft:item_slot",
            CommandParser.MinecraftResourceLocation => "minecraft:resource_location",
            CommandParser.MinecraftMobEffect => "minecraft:mob_effect",
            CommandParser.MinecraftFunction => "minecraft:function",
            CommandParser.MinecraftEntityAnchor => "minecraft:entity_anchor",
            CommandParser.MinecraftRange => "minecraft:range",
            CommandParser.MinecraftIntRange => "minecraft:int_range",
            CommandParser.MinecraftFloatRange => "minecraft:float_range",
            CommandParser.MinecraftItemEnchantment => "minecraft:item_enchantment",
            CommandParser.MinecraftEntitySummon => "minecraft:entity_summon",
            CommandParser.MinecraftDimension => "minecraft:dimension",
            CommandParser.MinecraftUuid => "minecraft:uuid",
            CommandParser.MinecraftNbtTag => "minecraft:nbt_tag",
            CommandParser.MinecraftNbtCompoundTag => "minecraft:nbt_compound_tag",
            CommandParser.MinecraftTime => "minecraft:time",
            CommandParser.ForgeModid => "forge:modid",
            CommandParser.ForgeEnum => "forge:enum",
            _ => throw new ArgumentOutOfRangeException(nameof(parser), parser, null),
        };

        public static ICommandProperties ReadProperties(this CommandParser parser, BinaryReader reader) => parser switch
        {
            CommandParser.BrigadierDouble => new DoubleCommandProperties(reader),
            CommandParser.BrigadierFloat => new FloatCommandProperties(reader),
            CommandParser.BrigadierInteger => new IntegerCommandProperties(reader),
            CommandParser.BrigadierLong => new LongCommandProperties(reader),
            CommandParser.BrigadierString => new StringCommandProperties(reader),
            CommandParser.MinecraftEntity => new EntityCommandProperties(reader),
            CommandParser.MinecraftScoreHolder => new ScoreHolderCommandProperties(reader),
            CommandParser.MinecraftRange => new RangeCommandProperties(reader),
            // CommandParser.ForgeModid => new ModidCommandProperty(reader),
            // CommandParser.ForgeEnum => new EnumCommandProperty(reader),
            _ => throw new NotSupportedException(parser.ToString()),
        };
    }
}