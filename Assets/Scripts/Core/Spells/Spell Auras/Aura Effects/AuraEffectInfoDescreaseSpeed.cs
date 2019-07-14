using Common;
using JetBrains.Annotations;
using UnityEngine;

namespace Core.AuraEffects
{
    [UsedImplicitly, CreateAssetMenu(fileName = "Aura Effect Decrease Speed", menuName = "Game Data/Spells/Auras/Effects/Speed Decrease", order = 2)]
    public class AuraEffectInfoDescreaseSpeed : AuraEffectInfo
    {
        [SerializeField, UsedImplicitly, Range(1.0f, 99.9f)] private float decreasePercent;

        public override float Value => decreasePercent;
        public override AuraEffectType AuraEffectType => AuraEffectType.SpeedDecreaseModifier;

        internal override AuraEffect CreateEffect(Aura aura, Unit caster, int index)
        {
            return new AuraEffectDecreaseSpeed(aura, this, index, Value);
        }
    }

    public class AuraEffectDecreaseSpeed : AuraEffect
    {
        public AuraEffectDecreaseSpeed(Aura aura, AuraEffectInfoDescreaseSpeed effectInfo, int index, float value) : base(aura, effectInfo, index, value)
        {
        }

        public override void HandleEffect(AuraApplication auraApplication, AuraEffectHandleMode mode, bool apply)
        {
            Logging.LogAura($"Handle aura effect {EffectInfo.name} for target: {auraApplication.Target.Name} in mode {mode}, applying: {apply}");

            auraApplication.Target.UpdateSpeed(UnitMoveType.Run);
        }
    }
}