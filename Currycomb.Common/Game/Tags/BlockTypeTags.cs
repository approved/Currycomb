using System.Collections.Generic;
using System.Runtime.CompilerServices;

namespace Currycomb.Common.Game.Tags
{
    public class BlockTypeTags
    {
        public static List<string> RequiredTags = new();

        // TODO: Replace These with Identifiers
        public static string Wool;
        public static string Planks;
        public static string StoneBricks;
        public static string WoodenButtons;
        public static string Buttons;
        public static string Carpets;
        public static string WoodenDoors;
        public static string WoodenStairs;
        public static string WoodenSlabs;
        public static string WoodenFences;
        public static string PressurePlates;
        public static string WoodenPressurePlates;
        public static string StonePressurePlates;
        public static string WoodenTrapdoors;
        public static string Doors;
        public static string Saplings;
        public static string LogsThatBurn;
        public static string Logs;
        public static string DarkOakLogs;
        public static string OakLogs;
        public static string BirchLogs;
        public static string AcaciaLogs;
        public static string JungleLogs;
        public static string SpruceLogs;
        public static string CrimsonStems;
        public static string WarpedStems;
        public static string Banners;
        public static string Sand;
        public static string Stairs;
        public static string Slabs;
        public static string Walls;
        public static string Anvil;
        public static string Rails;
        public static string Leaves;
        public static string Trapdoors;
        public static string SmallFlowers;
        public static string Beds;
        public static string Fences;
        public static string TallFlowers;
        public static string Flowers;
        public static string PiglinRepellents;
        public static string GoldOres;
        public static string IronOres;
        public static string DiamondOres;
        public static string RedstoneOres;
        public static string LapisOres;
        public static string CoalOres;
        public static string EmeraldOres;
        public static string CopperOres;
        public static string NonFlammableWood;
        public static string Candles;
        public static string Dirt;
        public static string FlowerPots;
        public static string EndermanHoldable;
        public static string Ice;
        public static string ValidSpawn;
        public static string Impermeable;
        public static string Underwater_bonemeals;
        public static string CoralBlocks;
        public static string WallCorals;
        public static string CoralPlants;
        public static string Corals;
        public static string BambooPlantableOn;
        public static string StandingSigns;
        public static string WallSigns;
        public static string Signs;
        public static string DragonImmune;
        public static string WitherImmune;
        public static string WitherSummonBaseBlocks;
        public static string Beehives;
        public static string Crops;
        public static string BeeGrowables;
        public static string Portals;
        public static string Fire;
        public static string Nylium;
        public static string WartBlocks;
        public static string BeaconBaseBlocks;
        public static string SoulSpeedBlocks;
        public static string WallPostOverride;
        public static string Climbable;
        public static string ShulkerBoxes;
        public static string HoglinRepellents;
        public static string SoulFireBaseBlocks;
        public static string StriderWarmBlocks;
        public static string Campfires;
        public static string GuardedByPiglins;
        public static string PreventMobSpawningInside;
        public static string FenceGates;
        public static string UnstableBottomCenter;
        public static string MushroomGrowBlock;
        public static string InfiniburnOverworld;
        public static string InfiniburnNether;
        public static string InfiniburnEnd;
        public static string BaseStoneOverworld;
        public static string StoneOreReplaceables;
        public static string DeepslateOreReplaceables;
        public static string BaseStoneNether;
        public static string CandleCakes;
        public static string Cauldrons;
        public static string CrystalSoundBlocks;
        public static string InsideStepSoundBlocks;
        public static string OccludesVibrationSignals;
        public static string DripstoneReplaceableBlocks;
        public static string CaveVines;
        public static string MossReplaceable;
        public static string LushGroundReplaceable;
        public static string SmallDripleafPlaceable;
        public static string Snow;
        public static string AxeMineable;
        public static string HoeMineable;
        public static string PickaxeMineable;
        public static string ShovelMineable;
        public static string NeedsDiamondTool;
        public static string NeedsIronTool;
        public static string NeedsStoneTool;
        public static string FeaturesCannotReplace;
        public static string LavaPoolStoneReplaceables;
        public static string GeodeInvalidBlocks;

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static string Register(string brand, string id)
        {
            string tag = $"{brand}:{id}";
            RequiredTags.Add(tag);
            return tag;
        }

        static BlockTypeTags()
        {
            Wool = Register("minecraft", "wool");
            Planks = Register("minecraft", "planks");
            StoneBricks = Register("minecraft", "stone_bricks");
            WoodenButtons = Register("minecraft", "wooden_buttons");
            Buttons = Register("minecraft", "buttons");
            Carpets = Register("minecraft", "carpets");
            WoodenDoors = Register("minecraft", "wooden_doors");
            WoodenStairs = Register("minecraft", "wooden_stairs");
            WoodenSlabs = Register("minecraft", "wooden_slabs");
            WoodenFences = Register("minecraft", "wooden_fences");
            PressurePlates = Register("minecraft", "pressure_plates");
            WoodenPressurePlates = Register("minecraft", "wooden_pressure_plates");
            StonePressurePlates = Register("minecraft", "stone_pressure_plates");
            WoodenTrapdoors = Register("minecraft", "wooden_trapdoors");
            Doors = Register("minecraft", "doors");
            Saplings = Register("minecraft", "saplings");
            LogsThatBurn = Register("minecraft", "logs_that_burn");
            Logs = Register("minecraft", "logs");
            DarkOakLogs = Register("minecraft", "dark_oak_logs");
            OakLogs = Register("minecraft", "oak_logs");
            BirchLogs = Register("minecraft", "birch_logs");
            AcaciaLogs = Register("minecraft", "acacia_logs");
            JungleLogs = Register("minecraft", "jungle_logs");
            SpruceLogs = Register("minecraft", "spruce_logs");
            CrimsonStems = Register("minecraft", "crimson_stems");
            WarpedStems = Register("minecraft", "warped_stems");
            Banners = Register("minecraft", "banners");
            Sand = Register("minecraft", "sand");
            Stairs = Register("minecraft", "stairs");
            Slabs = Register("minecraft", "slabs");
            Walls = Register("minecraft", "walls");
            Anvil = Register("minecraft", "anvil");
            Rails = Register("minecraft", "rails");
            Leaves = Register("minecraft", "leaves");
            Trapdoors = Register("minecraft", "trapdoors");
            SmallFlowers = Register("minecraft", "small_flowers");
            Beds = Register("minecraft", "beds");
            Fences = Register("minecraft", "fences");
            TallFlowers = Register("minecraft", "tall_flowers");
            Flowers = Register("minecraft", "flowers");
            PiglinRepellents = Register("minecraft", "piglin_repellents");
            GoldOres = Register("minecraft", "gold_ores");
            IronOres = Register("minecraft", "iron_ores");
            DiamondOres = Register("minecraft", "diamond_ores");
            RedstoneOres = Register("minecraft", "redstone_ores");
            LapisOres = Register("minecraft", "lapis_ores");
            CoalOres = Register("minecraft", "coal_ores");
            EmeraldOres = Register("minecraft", "emerald_ores");
            CopperOres = Register("minecraft", "copper_ores");
            NonFlammableWood = Register("minecraft", "non_flammable_wood");
            Candles = Register("minecraft", "candles");
            Dirt = Register("minecraft", "dirt");
            FlowerPots = Register("minecraft", "flower_pots");
            EndermanHoldable = Register("minecraft", "enderman_holdable");
            Ice = Register("minecraft", "ice");
            ValidSpawn = Register("minecraft", "valid_spawn");
            Impermeable = Register("minecraft", "impermeable");
            Underwater_bonemeals = Register("minecraft", "underwater_bonemeals");
            CoralBlocks = Register("minecraft", "coral_blocks");
            WallCorals = Register("minecraft", "wall_corals");
            CoralPlants = Register("minecraft", "coral_plants");
            Corals = Register("minecraft", "corals");
            BambooPlantableOn = Register("minecraft", "bamboo_plantable_on");
            StandingSigns = Register("minecraft", "standing_signs");
            WallSigns = Register("minecraft", "wall_signs");
            Signs = Register("minecraft", "signs");
            DragonImmune = Register("minecraft", "dragon_immune");
            WitherImmune = Register("minecraft", "wither_immune");
            WitherSummonBaseBlocks = Register("minecraft", "wither_summon_base_blocks");
            Beehives = Register("minecraft", "beehives");
            Crops = Register("minecraft", "crops");
            BeeGrowables = Register("minecraft", "bee_growables");
            Portals = Register("minecraft", "portals");
            Fire = Register("minecraft", "fire");
            Nylium = Register("minecraft", "nylium");
            WartBlocks = Register("minecraft", "wart_blocks");
            BeaconBaseBlocks = Register("minecraft", "beacon_base_blocks");
            SoulSpeedBlocks = Register("minecraft", "soul_speed_blocks");
            WallPostOverride = Register("minecraft", "wall_post_override");
            Climbable = Register("minecraft", "climbable");
            ShulkerBoxes = Register("minecraft", "shulker_boxes");
            HoglinRepellents = Register("minecraft", "hoglin_repellents");
            SoulFireBaseBlocks = Register("minecraft", "soul_fire_base_blocks");
            StriderWarmBlocks = Register("minecraft", "strider_warm_blocks");
            Campfires = Register("minecraft", "campfires");
            GuardedByPiglins = Register("minecraft", "guarded_by_piglins");
            PreventMobSpawningInside = Register("minecraft", "prevent_mob_spawning_inside");
            FenceGates = Register("minecraft", "fence_gates");
            UnstableBottomCenter = Register("minecraft", "unstable_bottom_center");
            MushroomGrowBlock = Register("minecraft", "mushroom_grow_block");
            InfiniburnOverworld = Register("minecraft", "infiniburn_overworld");
            InfiniburnNether = Register("minecraft", "infiniburn_nether");
            InfiniburnEnd = Register("minecraft", "infiniburn_end");
            BaseStoneOverworld = Register("minecraft", "base_stone_overworld");
            StoneOreReplaceables = Register("minecraft", "stone_ore_replaceables");
            DeepslateOreReplaceables = Register("minecraft", "deepslate_ore_replaceables");
            BaseStoneNether = Register("minecraft", "base_stone_nether");
            CandleCakes = Register("minecraft", "candle_cakes");
            Cauldrons = Register("minecraft", "cauldrons");
            CrystalSoundBlocks = Register("minecraft", "crystal_sound_blocks");
            InsideStepSoundBlocks = Register("minecraft", "inside_step_sound_blocks");
            OccludesVibrationSignals = Register("minecraft", "occludes_vibration_signals");
            DripstoneReplaceableBlocks = Register("minecraft", "dripstone_replaceable_blocks");
            CaveVines = Register("minecraft", "cave_vines");
            MossReplaceable = Register("minecraft", "moss_replaceable");
            LushGroundReplaceable = Register("minecraft", "lush_ground_replaceable");
            SmallDripleafPlaceable = Register("minecraft", "small_dripleaf_placeable");
            Snow = Register("minecraft", "snow");
            AxeMineable = Register("minecraft", "mineable/axe");
            HoeMineable = Register("minecraft", "mineable/hoe");
            PickaxeMineable = Register("minecraft", "mineable/pickaxe");
            ShovelMineable = Register("minecraft", "mineable/shovel");
            NeedsDiamondTool = Register("minecraft", "needs_diamond_tool");
            NeedsIronTool = Register("minecraft", "needs_iron_tool");
            NeedsStoneTool = Register("minecraft", "needs_stone_tool");
            FeaturesCannotReplace = Register("minecraft", "features_cannot_replace");
            LavaPoolStoneReplaceables = Register("minecraft", "lava_pool_stone_replaceables");
            GeodeInvalidBlocks = Register("minecraft", "geode_invalid_blocks");
        }
    }
}
