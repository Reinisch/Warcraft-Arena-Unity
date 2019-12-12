using JetBrains.Annotations;
using UnityEngine;

namespace Core.AuraEffects
{
    [UsedImplicitly, CreateAssetMenu(fileName = "Aura Effect Modify Stealth Detection", menuName = "Game Data/Spells/Auras/Effects/Modify Stealth Detection", order = 3)]
    public class AuraEffectInfoModifyStealthDetection : AuraEffectInfo
    {
        [SerializeField, UsedImplicitly, Range(-100.0f, 100.0f)]
        private float stealthDetectValue;

        public override float Value => stealthDetectValue;
        public override AuraEffectType AuraEffectType => AuraEffectType.ModifyStealthDetect;

        internal override AuraEffect CreateEffect(Aura aura, Unit caster, int index)
        {
            return new AuraEffectModifyStealthDetection(aura, this, index, Value);
        }
    }
}