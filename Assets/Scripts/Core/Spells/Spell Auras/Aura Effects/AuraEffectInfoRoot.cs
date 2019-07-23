using JetBrains.Annotations;
using UnityEngine;

namespace Core.AuraEffects
{
    [UsedImplicitly, CreateAssetMenu(fileName = "Aura Effect Root State", menuName = "Game Data/Spells/Auras/Effects/State Root", order = 2)]
    public class AuraEffectInfoRoot : AuraEffectInfo
    {
        public override float Value => 1.0f;
        public override AuraEffectType AuraEffectType => AuraEffectType.RootState;

        internal override AuraEffect CreateEffect(Aura aura, Unit caster, int index)
        {
            return new AuraEffectRoot(aura, this, index, Value);
        }
    }

    public class AuraEffectRoot : AuraEffect
    {
        public override AuraEffectHandleGroup HandleGroup => AuraEffectHandleGroup.RootStateChange;

        public AuraEffectRoot(Aura aura, AuraEffectInfo effectInfo, int index, float value) : base(aura, effectInfo, index, value)
        {
        }

        public override void HandleEffect(AuraApplication auraApplication, AuraEffectHandleMode mode, bool apply)
        {
            base.HandleEffect(auraApplication, mode, apply);

            auraApplication.Target.UpdateControlState(UnitControlState.Root, apply);
        }
    }
}