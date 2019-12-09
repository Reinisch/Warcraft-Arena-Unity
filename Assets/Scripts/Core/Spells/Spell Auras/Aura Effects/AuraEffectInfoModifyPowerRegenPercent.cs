using JetBrains.Annotations;
using UnityEngine;

namespace Core.AuraEffects
{
    [UsedImplicitly, CreateAssetMenu(fileName = "Aura Effect Modify Power Regen Percent", menuName = "Game Data/Spells/Auras/Effects/Modify Power Regen Percent", order = 3)]
    public class AuraEffectInfoModifyPowerRegenPercent : AuraEffectInfo
    {
        [SerializeField, UsedImplicitly, Range(-100f, 500f)] private float powerRegenPercent;
        [SerializeField, UsedImplicitly] private SpellPowerType powerType;

        public override float Value => powerRegenPercent;
        public override float SecondaryValue => (int)powerType;
        public override AuraEffectType AuraEffectType => AuraEffectType.ModifyPowerRegenPercent;

        internal override AuraEffect CreateEffect(Aura aura, Unit caster, int index)
        {
            return new AuraEffectSharedBasicModifer(aura, this, index, Value);
        }
    }
}