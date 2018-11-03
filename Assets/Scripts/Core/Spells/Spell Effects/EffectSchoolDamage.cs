using System.Linq;
using JetBrains.Annotations;
using UnityEngine;

namespace Core
{
    public class EffectSchoolDamage : SpellEffectInfo
    {
        [SerializeField, UsedImplicitly] private int bonusDamage;

        public int BonusDamage => bonusDamage;

        public override SpellEffectType EffectType => SpellEffectType.SchoolDamage;
        public override ExplicitTargetTypes ExplicitTargetType => ExplicitTargetTypes.Explicit;
        public override TargetEntities TargetEntityType => TargetEntities.Unit;

        public override void Handle(Spell spell, Unit target, SpellEffectHandleMode mode)
        {
            spell.EffectSchoolDMG(this, target, mode);
        }
    }

    public partial class Spell
    {
        public void EffectSchoolDMG(EffectSchoolDamage effect, Unit target, SpellEffectHandleMode mode)
        {
            if (mode != SpellEffectHandleMode.LaunchTarget)
                return;

            if (target == null || !target.IsAlive)
                return;

            switch (SpellInfo.SpellFamilyName)
            {
                case SpellFamilyNames.Generic:
                    if (SpellInfo.HasAttribute(SpellCustomAttributes.ShareDamage))
                        SpellDamage /= UniqueTargetInfo.Count(targetInfo => (targetInfo.EffectMask & (1 << effect.Index)) != 0);
                    break;
            }

            if (OriginalCaster != null)
            {
                int bonus = OriginalCaster.SpellDamageBonusDone(target, SpellInfo, SpellDamage, DamageEffectType.DirectDamage, effect);
                SpellDamage = bonus + (int)(bonus * Variance);
                SpellDamage = target.SpellDamageBonusTaken(OriginalCaster, SpellInfo, SpellDamage, DamageEffectType.DirectDamage, effect);
            }

            EffectDamage += SpellDamage + effect.BonusDamage;
        }
    }
}
