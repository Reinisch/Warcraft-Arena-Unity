using JetBrains.Annotations;
using UnityEngine;

namespace Core.AuraEffects
{
    [UsedImplicitly, CreateAssetMenu(fileName = "Aura Effect Stealth", menuName = "Game Data/Spells/Auras/Effects/Stealth", order = 3)]
    public class AuraEffectInfoStealth : AuraEffectInfo
    {
        [SerializeField, UsedImplicitly, Range(0.0f, 100f)]
        private float stealthValue;

        public override float Value => stealthValue;
        public override AuraEffectType AuraEffectType => AuraEffectType.Stealth;

        internal override AuraEffect CreateEffect(Aura aura, Unit caster, int index)
        {
            return new AuraEffectStealth(aura, this, index, Value);
        }
    }
}