using JetBrains.Annotations;
using UnityEngine;

namespace Core.AuraEffects
{
    [UsedImplicitly, CreateAssetMenu(fileName = "Aura Effect Prevent Casting", menuName = "Game Data/Spells/Auras/Effects/Prevent Casting", order = 2)]
    public abstract class AuraEffectInfoPreventCasting : AuraEffectInfo
    {
        public abstract SpellPreventionType PreventionType { get; }
        public override float Value => 1.0f;

        internal override AuraEffect CreateEffect(Aura aura, Unit caster, int index)
        {
            return new AuraEffectPreventCasting(aura, this, index, Value);
        }
    }
}