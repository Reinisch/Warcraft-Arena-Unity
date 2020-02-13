using JetBrains.Annotations;
using UnityEngine;

namespace Core.AuraEffects
{
    [UsedImplicitly, CreateAssetMenu(fileName = "Aura Effect Control Vehicle", menuName = "Game Data/Spells/Auras/Effects/Control Vehicle", order = 4)]
    public class AuraEffectInfoControlVehicle : AuraEffectInfo
    {
        public override float Value => 1.0f;
        public override AuraEffectType AuraEffectType => AuraEffectType.ControlVehicle;

        internal override AuraEffect CreateEffect(Aura aura, Unit caster, int index)
        {
            return new AuraEffectControlVehicle(aura, this, index, Value);
        }
    }
}