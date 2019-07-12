namespace Core
{
    public class AuraEffect
    {
        private int index;
        private int baseAmount;
        private int amount;

        public Unit Caster => Aura.Caster;
        public AuraEffectInfo EffectInfo { get; }
        public Aura Aura { get; }

        public AuraEffect(Aura aura, AuraEffectInfo effectInfo, int index, int baseAmount, Unit caster)
        {
            Aura = aura;
            EffectInfo = effectInfo;

            this.index = index;
            this.baseAmount = baseAmount;
        }

        public void HandleEffect(AuraApplication auraApplication, AuraEffectHandleMode mode, bool apply)
        {

        }

        public void Update(int deltaTime)
        {
        }

        public void HandleAuraModIncreaseSpeed(AuraApplication auraApplication, AuraEffectHandleMode mode, bool apply)
        {
        }

        public void HandleAuraModDecreaseSpeed(AuraApplication auraApplication, AuraEffectHandleMode mode, bool apply)
        {
        }
    }
}