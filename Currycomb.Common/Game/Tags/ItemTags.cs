using System.Collections.Generic;
using System.Runtime.CompilerServices;

namespace Currycomb.Common.Game.Tags
{
    public class ItemTags
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
        public static string WoodenPressurePlates;
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
        public static string PiglinLoved;
        public static string IgnoredByPiglinBabies;
        public static string PiglinFood;
        public static string FoxFood;
        public static string GoldOres;
        public static string IronOres;
        public static string DiamondOres;
        public static string RedstoneOres;
        public static string LapisOres;
        public static string CoalOres;
        public static string EmeraldOres;
        public static string CopperOres;
        public static string NonFlammableWood;
        public static string SoulFireBaseBlock;
        public static string Candles;
        public static string Boats;
        public static string Fishes;
        public static string Signs;
        public static string MusicDiscs;
        public static string CreeperDropMusicDiscs;
        public static string Coals;
        public static string Arrows;
        public static string LecternBooks;
        public static string BeaconPaymentItems;
        public static string StoneToolMaterials;
        public static string StoneCraftingMaterials;
        public static string FreezeImmuneWearables;
        public static string AxolotlTemptItems;
        public static string OccludesVibrationSignals;
        public static string ClusterMaxHarvestable;

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static string Register(string brand, string id)
        {
            string tag = $"{brand}:{id}";
            RequiredTags.Add(tag);
            return tag;
        }

        static ItemTags()
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
            WoodenPressurePlates = Register("minecraft", "wooden_pressure_plates");
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
            PiglinLoved = Register("minecraft", "piglin_loved");
            IgnoredByPiglinBabies = Register("minecraft", "ignored_by_piglin_babies");
            PiglinFood = Register("minecraft", "piglin_food");
            FoxFood = Register("minecraft", "fox_food");
            GoldOres = Register("minecraft", "gold_ores");
            IronOres = Register("minecraft", "iron_ores");
            DiamondOres = Register("minecraft", "diamond_ores");
            RedstoneOres = Register("minecraft", "redstone_ores");
            LapisOres = Register("minecraft", "lapis_ores");
            CoalOres = Register("minecraft", "coal_ores");
            EmeraldOres = Register("minecraft", "emerald_ores");
            CopperOres = Register("minecraft", "copper_ores");
            NonFlammableWood = Register("minecraft", "non_flammable_wood");
            SoulFireBaseBlock = Register("minecraft", "soul_fire_base_blocks");
            Candles = Register("minecraft", "candles");
            Boats = Register("minecraft", "boats");
            Fishes = Register("minecraft", "fishes");
            Signs = Register("minecraft", "signs");
            MusicDiscs = Register("minecraft", "music_discs");
            CreeperDropMusicDiscs = Register("minecraft", "creeper_drop_music_discs");
            Coals = Register("minecraft", "coals");
            Arrows = Register("minecraft", "arrows");
            LecternBooks = Register("minecraft", "lectern_books");
            BeaconPaymentItems = Register("minecraft", "beacon_payment_items");
            StoneToolMaterials = Register("minecraft", "stone_tool_materials");
            StoneCraftingMaterials = Register("minecraft", "stone_crafting_materials");
            FreezeImmuneWearables = Register("minecraft", "freeze_immune_wearables");
            AxolotlTemptItems = Register("minecraft", "axolotl_tempt_items");
            OccludesVibrationSignals = Register("minecraft", "occludes_vibration_signals");
            ClusterMaxHarvestable = Register("minecraft", "cluster_max_harvestables");
        }
    }
}
