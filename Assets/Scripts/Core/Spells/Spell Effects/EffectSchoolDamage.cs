using System.Linq;
using Common;
using JetBrains.Annotations;
using UnityEngine;

namespace Core
{
    public class EffectSchoolDamage : SpellEffectInfo
    {
        [SerializeField, UsedImplicitly, Header("School Damage")] private int bonusDamage;

        public int BonusDamage => bonusDamage;
        public override SpellEffectType EffectType => SpellEffectType.SchoolDamage;
        public override SpellExplicitTargetType ExplicitTargetType => SpellExplicitTargetType.Explicit;
        public override SpellTargetEntities TargetEntityType => SpellTargetEntities.Unit;

        internal override void Handle(Spell spell, Unit target, SpellEffectHandleMode mode)
        {
            spell.EffectSchoolDamage(this, target, mode);
        }

        internal int CalculateSpellPower(SpellInfo spellInfo, Unit caster = null, Unit target = null, int basePoints = -1)
        {
            basePoints = basePoints == -1 ? BasePoints : basePoints;

            if (Mathf.Abs(RandomPoints) <= 1)
                basePoints += RandomPoints;
            else
                basePoints += RandomPoints > 0 ? RandomUtils.Next(1, RandomPoints + 1) : RandomUtils.Next(RandomPoints, 1);

            float value = basePoints;
            if (caster != null)
                value = caster.ApplyEffectModifiers(spellInfo, Index, value);

            return (int)value;
        }
    }

    public partial class Spell
    {
        internal void EffectSchoolDamage(EffectSchoolDamage effect, Unit target, SpellEffectHandleMode mode)
        {
            if (mode != SpellEffectHandleMode.HitTarget || target == null || !target.IsAlive)
                return;

            int spellPower = effect.CalculateSpellPower(SpellInfo, Caster, target);
            if (SpellInfo.HasAttribute(SpellCustomAttributes.ShareDamage))
                spellPower /= UniqueTargetInfo.Count(targetInfo => (targetInfo.EffectMask & (1 << effect.Index)) != 0);

            if (OriginalCaster != null)
            {
                int bonus = OriginalCaster.SpellDamageBonusDone(target, SpellInfo, spellPower, SpellDamageType.Direct, effect);
                spellPower = bonus + (int)(bonus * Variance);
                spellPower = target.SpellDamageBonusTaken(OriginalCaster, SpellInfo, spellPower, SpellDamageType.Direct, effect);
            }

            EffectDamage += spellPower + effect.BonusDamage;
        }
    }
}
