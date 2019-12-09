using JetBrains.Annotations;
using UnityEngine;

namespace Core.AuraEffects
{
    [UsedImplicitly, CreateAssetMenu(fileName = "Aura Effect Shape Shift", menuName = "Game Data/Spells/Auras/Effects/Shape Shift", order = 1)]
    public class AuraEffectInfoShapeShift : AuraEffectInfo
    {
        [SerializeField, UsedImplicitly] 
        private ShapeShiftForm shapeShiftForm;

        public ShapeShiftForm ShapeShiftForm => shapeShiftForm;
        public override float Value => (int)shapeShiftForm;
        public override AuraEffectType AuraEffectType => AuraEffectType.ShapeShift;

        internal override AuraEffect CreateEffect(Aura aura, Unit caster, int index)
        {
            return new AuraEffectShapeShift(aura, this, index, Value);
        }
    }
}