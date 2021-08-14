using System.Collections.Generic;
using System.Runtime.CompilerServices;

namespace Currycomb.Common.Game.Tags
{
    public class GameEventTags
    {
        public static List<string> RequiredTags = new();

        // TODO: Replace These with Identifiers
        public static string Vibrations;
        public static string IgnoreVibrationsSneaking;

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static string Register(string brand, string id)
        {
            string tag = $"{brand}:{id}";
            RequiredTags.Add(tag);
            return tag;
        }

        static GameEventTags()
        {
            Vibrations = Register("minecraft", "vibrations");
            IgnoreVibrationsSneaking = Register("minecraft", "ignore_vibrations_sneaking");
        }
    }
}
