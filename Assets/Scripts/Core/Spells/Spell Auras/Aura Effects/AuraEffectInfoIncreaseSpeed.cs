using Common;
using JetBrains.Annotations;
using UnityEngine;

namespace Core.AuraEffects
{
    [UsedImplicitly, CreateAssetMenu(fileName = "Aura Effect Increase Speed", menuName = "Game Data/Spells/Auras/Effects/Speed Increase", order = 1)]
    public class AuraEffectInfoIncreaseSpeed : AuraEffectInfo
    {
        [SerializeField, UsedImplicitly] private float increasePercent;

        public override float Value => increasePercent;
        public override AuraEffectType AuraEffectType => AuraEffectType.SpeedIncreaseModifier;

        internal override AuraEffect CreateEffect(Aura aura, Unit caster, int index)
        {
            return new AuraEffectIncreaseSpeed(aura, this, index, Value);
        }
    }

    public class AuraEffectIncreaseSpeed : AuraEffect
    {
        public AuraEffectIncreaseSpeed(Aura aura, AuraEffectInfoIncreaseSpeed effectInfo, int index, float value) : base(aura, effectInfo, index, value)
        {
        }

        public override void HandleEffect(AuraApplication auraApplication, AuraEffectHandleMode mode, bool apply)
        {
            Logging.LogAura($"Handle aura effect {EffectInfo.name} for target: {auraApplication.Target.Name} in mode {mode}, applying: {apply}");

            auraApplication.Target.UpdateSpeed(UnitMoveType.Run);
        }
    }
}