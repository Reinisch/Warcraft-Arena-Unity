using JetBrains.Annotations;
using UnityEngine;

namespace Core.AuraEffects
{
    [UsedImplicitly, CreateAssetMenu(fileName = "Aura Effect Detect All Stealth", menuName = "Game Data/Spells/Auras/Effects/Detect All Stealth", order = 3)]
    public class AuraEffectInfoDetectAllStealth : AuraEffectInfo
    {
        public override float Value => 1.0f;
        public override AuraEffectType AuraEffectType => AuraEffectType.DetectAllStealth;

        internal override AuraEffect CreateEffect(Aura aura, Unit caster, int index)
        {
            return new AuraEffectDetectAllStealth(aura, this, index, Value);
        }
    }
}