using JetBrains.Annotations;
using UnityEngine;

namespace Core.AuraEffects
{
    [UsedImplicitly, CreateAssetMenu(fileName = "Aura Effect Change Display Model", menuName = "Game Data/Spells/Auras/Effects/Change Display Model", order = 1)]
    public class AuraEffectInfoChangeDisplayModel : AuraEffectInfo
    {
        [SerializeField, UsedImplicitly, Range(1, 100)] private int modelId = 1;

        public int ModelId => modelId;
        public override float Value => modelId;
        public override AuraEffectType AuraEffectType => AuraEffectType.ChangeDisplayModel;

        internal override AuraEffect CreateEffect(Aura aura, Unit caster, int index)
        {
            return new AuraEffectChangeDisplayModel(aura, this, index, Value);
        }
    }
}
