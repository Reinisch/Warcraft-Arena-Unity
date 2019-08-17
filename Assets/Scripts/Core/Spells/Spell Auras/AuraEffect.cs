using Common;

namespace Core
{
    public abstract class AuraEffect
    {
        public int Index { get; }
        public float BaseValue { get; }
        public float Value { get; private set; }

        public Aura Aura { get; }
        public AuraEffectInfo EffectInfo { get; }
        public abstract AuraEffectHandleGroup HandleGroup { get; }

        protected AuraEffect(Aura aura, AuraEffectInfo effectInfo, int index, float baseValue)
        {
            Aura = aura;
            EffectInfo = effectInfo;

            Index = index;
            BaseValue = baseValue;
            Value = baseValue;
        }

        public virtual void Update(int deltaTime)
        {
        }

        public virtual void HandleEffect(AuraApplication auraApplication, AuraEffectHandleMode mode, bool apply)
        {
            Logging.LogAura($"Handle aura effect {EffectInfo.name} for target: {auraApplication.Target.Name} in mode {mode}, applying: {apply}");
        }

        public void ModifyValue(float delta)
        {
            Value += delta;
        }
    }
}