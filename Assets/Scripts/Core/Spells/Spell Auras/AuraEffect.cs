namespace Core
{
    public abstract class AuraEffect
    {
        private int index;
        private float baseValue;

        public float Value { get; }

        public Aura Aura { get; }
        public AuraEffectInfo EffectInfo { get; }
        public abstract AuraEffectHandleGroup HandleGroup { get; }

        public AuraEffect(Aura aura, AuraEffectInfo effectInfo, int index, float baseValue)
        {
            Aura = aura;
            EffectInfo = effectInfo;

            this.index = index;
            this.baseValue = baseValue;

            Value = baseValue;
        }

        public void Update(int deltaTime)
        {
        }

        public abstract void HandleEffect(AuraApplication auraApplication, AuraEffectHandleMode mode, bool apply);
    }
}