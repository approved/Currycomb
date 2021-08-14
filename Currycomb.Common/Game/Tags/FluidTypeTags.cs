using System.Collections.Generic;
using System.Runtime.CompilerServices;

namespace Currycomb.Common.Game.Tags
{
    public class FluidTypeTags
    {
        public static List<string> RequiredTags = new();

        // TODO: Replace These with Identifiers
        public static string Water;
        public static string Lava;

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static string Register(string brand, string id)
        {
            string tag = $"{brand}:{id}";
            RequiredTags.Add(tag);
            return tag;
        }

        static FluidTypeTags()
        {
            Water = Register("minecraft", "water");
            Lava = Register("minecraft", "lava");
        }
    }
}
