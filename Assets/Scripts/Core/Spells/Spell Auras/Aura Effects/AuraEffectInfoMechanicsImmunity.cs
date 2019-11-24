using System.Collections.Generic;
using JetBrains.Annotations;
using UnityEngine;

namespace Core.AuraEffects
{
    [UsedImplicitly, CreateAssetMenu(fileName = "Aura Effect Mechanics Immunity", menuName = "Game Data/Spells/Auras/Effects/Mechanics Immunity", order = 4)]
    public class AuraEffectInfoMechanicsImmunity : AuraEffectInfo
    {
        [SerializeField, UsedImplicitly] private List<SpellMechanics> immuneMechanics;

        public override float Value => 1.0f;
        public override AuraEffectType AuraEffectType => AuraEffectType.MechanicImmunity;
        public IReadOnlyList<SpellMechanics> ImmuneMechanics => immuneMechanics;

        internal override AuraEffect CreateEffect(Aura aura, Unit caster, int index)
        {
            return new AuraEffectMechanicsImmunity(aura, this, index, Value);
        }
    }
}