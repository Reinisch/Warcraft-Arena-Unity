namespace Core
{
    public class AISpellInfoType
    {
        public AITarget Target;
        public AICondition Condition;
        public uint Cooldown;
        public uint RealCooldown;
        public float MaxRange;

        public AISpellInfoType()
        {
            Target = AITarget.Self;
            Condition = AICondition.Combat;
            Cooldown = AIHelper.AIDefaultCooldown;
            RealCooldown = 0;
            MaxRange = 0.0f;
        }
    }
}