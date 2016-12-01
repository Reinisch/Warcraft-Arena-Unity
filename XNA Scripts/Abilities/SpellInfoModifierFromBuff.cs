using System;

using BasicRpgEngine.Characters;

namespace BasicRpgEngine.Spells
{
    public class SpellInfoModifierFromBuff : SpellModifier, IBuffDependantModifier, IDisposable, ICloneable
    {
        float value;

        public string SpellName { get; private set; }
        public Buff BuffRef { get; set; }

        public SpellInfoModifierFromBuff(int id, ModifierCondition condition, SpellModifierType modifierType, float newValue, string newSpellName)
            : base(id, condition, modifierType)
        { value = newValue; SpellName = newSpellName; }

        public override void ModifySpell(ITargetable caster, ITargetable target, SpellModificationInformation spellInformation, Spell spell)
        {
            if (BuffRef.NeedsRemoval)
            {
                caster.Character.Entity.Buffs.Remove(BuffRef);
                BuffRef = null;
                NeedRemoval = true;
                return;
            }
            else if (BuffRef.HasStucks)
            {
                BuffRef.StuckRemoved();
                if (BuffRef.NeedsRemoval)
                {
                    caster.Character.Entity.Buffs.Remove(BuffRef);
                    NeedRemoval = true;
                    BuffRef = null;
                }
            }

            if (modifierCondition(caster, target))
                switch (ModifierType)
                {
                    case SpellModifierType.AdditionalDamage:
                        spellInformation.AdditionalDamage += value;
                        break;
                    case SpellModifierType.CriticalChanceAddition:
                        spellInformation.CriticalChanceAddMod += value;
                        break;
                    case SpellModifierType.CriticalChanceMultiplier:
                        spellInformation.CriticalChanceMulMod += value;
                        break;
                    case SpellModifierType.CriticalDamageMultiplier:
                        spellInformation.CriticalDamageMultiplierAddMod += value;
                        break;
                    case SpellModifierType.DamageMultiplier:
                        spellInformation.DamageMultiplier += value;
                        break;
                    case SpellModifierType.InstantCast:
                        spellInformation.InstantCast = true;
                        break;
                    case SpellModifierType.CastFailed:
                        spellInformation.IsCastFailed = true;
                        break;
                    default:
                        break;
                }
        }
        public override bool CheckForFailure(ITargetable caster, ITargetable target, SpellModificationInformation spellInformation, Spell spell)
        {
            if (modifierCondition(caster, target) && ModifierType == SpellModifierType.CastFailed)
                return true;
            return false;
        }
        public override bool CheckForInstantCast(ITargetable caster, ITargetable target, SpellModificationInformation spellInformation, Spell spell)
        {
            if (modifierCondition(caster, target) && ModifierType == SpellModifierType.InstantCast)
                return true;
            return false;
        }
        
        public object Clone()
        {
            return new SpellInfoModifierFromBuff(Id, modifierCondition, ModifierType, value, SpellName);
        }
        public override void Dispose()
        {
            base.Dispose();
            BuffRef = null;
        }
    }
}