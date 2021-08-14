using System.Collections.Generic;
using System.Runtime.CompilerServices;

namespace Currycomb.Common.Game.Tags
{
    public class EntityTypeTags
    {
        public static List<string> RequiredTags = new();

        // TODO: Replace These with Identifiers
        public static string Skeletons;
        public static string Raiders;
        public static string BeeHiveInhabitors;
        public static string Arrows;
        public static string ImpactProjectiles;
        public static string PowderSnowWalkableMobs;
        public static string AxolotlAlwaysHostile;
        public static string AxolotlHuntTargets;
        public static string FreezeImmuneEntityTypes;
        public static string FreezeHurtsExtraTypes;

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static string Register(string brand, string id)
        {
            string tag = $"{brand}:{id}";
            RequiredTags.Add(tag);
            return tag;
        }

        static EntityTypeTags()
        {
            Skeletons = Register("minecraft", "skeletons");
            Raiders = Register("minecraft", "raiders");
            BeeHiveInhabitors = Register("minecraft", "beehive_inhabitors");
            Arrows = Register("minecraft", "arrows");
            ImpactProjectiles = Register("minecraft", "impact_projectiles");
            PowderSnowWalkableMobs = Register("minecraft", "powder_snow_walkable_mobs");
            AxolotlAlwaysHostile = Register("minecraft", "axolotl_always_hostiles");
            AxolotlHuntTargets = Register("minecraft", "axolotl_hunt_targets");
            FreezeImmuneEntityTypes = Register("minecraft", "freeze_immune_entity_types");
            FreezeHurtsExtraTypes = Register("minecraft", "freeze_hurts_extra_types");
        }
    }
}
