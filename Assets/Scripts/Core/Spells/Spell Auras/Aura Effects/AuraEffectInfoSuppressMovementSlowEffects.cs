using Common;
using JetBrains.Annotations;
using UnityEngine;

namespace Core.AuraEffects
{
    [UsedImplicitly, CreateAssetMenu(fileName = "Aura Effect Suppress Movement Slow Effects", menuName = "Game Data/Spells/Auras/Effects/Suppress Movement Slow", order = 2)]
    public class AuraEffectInfoSuppressMovementSlowEffects : AuraEffectInfo
    {
        public override float Value => 0.0f;
        public override AuraEffectType AuraEffectType => AuraEffectType.SpeedSupressSlowEffects;

        internal override AuraEffect CreateEffect(Aura aura, Unit caster, int index)
        {
            return new AuraEffectSuppressMovementSlowEffects(aura, this, index, Value);
        }
    }

    public class AuraEffectSuppressMovementSlowEffects : AuraEffect
    {
        public AuraEffectSuppressMovementSlowEffects(Aura aura, AuraEffectInfoSuppressMovementSlowEffects effectInfo, int index, float value) : base(aura, effectInfo, index, value)
        {
        }

        public override void HandleEffect(AuraApplication auraApplication, AuraEffectHandleMode mode, bool apply)
        {
            Logging.LogAura($"Handle aura effect {EffectInfo.name} for target: {auraApplication.Target.Name} in mode {mode}, applying: {apply}");

            auraApplication.Target.UpdateSpeed(UnitMoveType.Run);
        }
    }
}