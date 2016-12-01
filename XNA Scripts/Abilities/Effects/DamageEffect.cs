using System;

using BasicRpgEngine.Characters;
using BasicRpgEngine.Spells;

namespace BasicRpgEngine.Spells
{
    public class DamageEffect : BaseEffect
    {
        DamageType damageType;
        int minValue;
        int maxValue;
        int modifier;

        public DamageEffect(DamageType DamageType, int MinValue, int MaxValue, int Modifier)
            : base(AoeMode.None)
        {
            damageType = DamageType;
            minValue = MinValue;
            maxValue = MaxValue;
            modifier = Modifier;
        }

        public override void Apply(ITargetable caster, ITargetable target, TimeSpan elapsedTime,
            SpellModificationInformation spellInfo, NetworkPlayerInterface playerUi)
        {
            int amount = modifier;

            amount += Mechanics.Roll(minValue, maxValue);

            if (amount < 1)
                amount = 1;

            if (spellInfo.DamageMultiplier != 0)
                amount = (int)(amount * spellInfo.DamageMultiplier);
            amount += (int)spellInfo.AdditionalDamage;
            if (spellInfo.IsCrit)
                amount = (int)(amount*spellInfo.CriticalDamageMultiplierAddMod);

            amount = (int)(amount*target.Character.Entity.IncomingDamageEfficiency);

            switch (damageType)
            {
                case DamageType.Physical:
                    amount = (int)(amount * caster.Character.Entity.PhysicalDamageMulMod);
                    break;
                case DamageType.Frost:
                    amount = (int)(amount * caster.Character.Entity.FrostDamageMulMod);
                    break;
                case DamageType.Fire:
                    amount = (int)(amount * caster.Character.Entity.FireDamageMulMod);
                    break;
                case DamageType.Arcane:
                    amount = (int)(amount * caster.Character.Entity.ArcaneDamageMulMod);
                    break;
                case DamageType.Nature:
                    amount = (int)(amount * caster.Character.Entity.NatureDamageMulMod);
                    break;
                case DamageType.Shadow:
                    amount = (int)(amount * caster.Character.Entity.ShadowDamageMulMod);
                    break;
                case DamageType.Holy:
                    amount = (int)(amount * caster.Character.Entity.HolyDamageMulMod);
                    break;
            }

            foreach (Buff buff in target.Character.Entity.Buffs)
            {
                if (buff.HasAbsorbAura)
                {
                    foreach (AuraBase auraAbsorb in buff.Auras)
                    {
                        amount = auraAbsorb.Absorb(damageType, amount, target.Character.Entity);
                        if (amount == 0)
                        {
                            if (caster.ID == playerUi.PlayerRef.ID)
                                playerUi.SkillDamageEvents.Add(new SkillDamageEvent(spellInfo.SpellName, "Absorb", 0, 0.7f, target, 50, false, spellInfo.IsCrit));
                            return;
                        }
                    }
                }
            }
            if (caster.ID == playerUi.PlayerRef.ID)
                playerUi.SkillDamageEvents.Add(new SkillDamageEvent(spellInfo.SpellName, "", amount, 0.7f, target, 30, false, spellInfo.IsCrit));
            target.Character.Entity.Health.Decrease((ushort)amount);
        }

        public override object Clone()
        {
            return new DamageEffect(damageType, minValue, maxValue, modifier);
        }
    }
}