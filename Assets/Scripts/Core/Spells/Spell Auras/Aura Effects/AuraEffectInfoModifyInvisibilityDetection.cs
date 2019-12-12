using JetBrains.Annotations;
using UnityEngine;

namespace Core.AuraEffects
{
    [UsedImplicitly, CreateAssetMenu(fileName = "Aura Effect Modify Invisibility Detection", menuName = "Game Data/Spells/Auras/Effects/Modify Invisibility Detection", order = 3)]
    public class AuraEffectInfoModifyInvisibilityDetection : AuraEffectInfo
    {
        [SerializeField, UsedImplicitly, Range(-100.0f, 100.0f)]
        private float invisibilityDetectValue;

        public override float Value => invisibilityDetectValue;
        public override AuraEffectType AuraEffectType => AuraEffectType.ModifyInvisibilityDetect;

        internal override AuraEffect CreateEffect(Aura aura, Unit caster, int index)
        {
            return new AuraEffectModifyInvisibilityDetection(aura, this, index, Value);
        }
    }
}