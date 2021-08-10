namespace Currycomb.Common.Game.Entity
{
    public class Abilities
    {
        public bool IsInvulnerable { get; set; }
        public bool IsFlying { get; set; }
        public bool CanFly { get; set; }
        public bool CanBuild { get; set; }
        public bool CanInstantBuild { get; set; }
        public float FlySpeed { get; set; }
        public float WalkSpeed { get; set; }

        // TODO: Write NBT Serializer
    }
}
