using Common;
using JetBrains.Annotations;
using UnityEngine;

namespace Core
{
    [UsedImplicitly, CreateAssetMenu(fileName = "Effect Trigger Spell", menuName = "Game Data/Spells/Effects/Trigger Spell", order = 4)]
    public class EffectTriggerSpell : SpellEffectInfo
    {
        [Header("Trigger Spell")]
        [SerializeField, UsedImplicitly] 
        private SpellInfo triggerSpell;
        [SerializeField, UsedImplicitly, EnumFlag] 
        private SpellCastFlags extraCastFlags;

        public SpellInfo TriggerSpell => triggerSpell;
        public SpellCastFlags ExtraCastFlags => extraCastFlags;

        public override float Value => 1.0f;
        public override SpellEffectType EffectType => SpellEffectType.TriggerSpell;

        internal override void Handle(Spell spell, int effectIndex, Unit target, SpellEffectHandleMode mode)
        {
            spell.EffectTriggerSpell(this, target, mode);
        }
    }

    public partial class Spell
    {
        internal void EffectTriggerSpell(EffectTriggerSpell effect, Unit target, SpellEffectHandleMode mode)
        {
            if (mode != SpellEffectHandleMode.HitFinal || target == null)
                return;

            Caster.Spells.TriggerSpell(effect.TriggerSpell, target, effect.ExtraCastFlags);
        }
    }
}
