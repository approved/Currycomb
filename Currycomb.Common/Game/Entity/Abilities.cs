namespace Currycomb.Common.Game.Entity
{
    public class Abilities
    {
        public bool IsInvulnerable { get; set; } = false;
        public bool IsFlying { get; set; } = false;
        public bool CanFly { get; set; } = false;
        public bool CanBuild { get; set; } = false;
        public bool CanInstantBuild { get; set; } = false;
        public float FlySpeed { get; set; } = 0.05f;
        public float WalkSpeed { get; set; } = 1.0f;

        // TODO: Write NBT Serializer
    }
}
