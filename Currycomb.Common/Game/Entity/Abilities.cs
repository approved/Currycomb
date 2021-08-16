namespace Currycomb.Common.Game.Entity
{
    public readonly struct Abilities
    {
        public readonly bool IsInvulnerable;
        public readonly bool IsFlying;
        public readonly bool CanFly;
        public readonly bool CanBuild;
        public readonly bool CanInstantBuild;
        public readonly float FlySpeed;
        public readonly float WalkSpeed;

        public Abilities(
            bool isInvulnerable = false,
            bool isFlying = false,
            bool canFly = false,
            bool canBuild = false,
            bool canInstantBuild = false,
            float flySpeed = 0.05f,
            float walkSpeed = 1.0f)
        {
            IsInvulnerable = isInvulnerable;
            IsFlying = isFlying;
            CanFly = canFly;
            CanBuild = canBuild;
            CanInstantBuild = canInstantBuild;
            FlySpeed = flySpeed;
            WalkSpeed = walkSpeed;
        }

        // TODO: Write NBT Serializer
    }
}
