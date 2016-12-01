using BasicRpgEngine.Characters;

namespace BasicRpgEngine.Spells
{
    public class SpellInfoModifier : SpellModifier
    {
        float value;

        public SpellInfoModifier(int id, ModifierCondition condition, SpellModifierType modifierType, float newValue)
            : base(id, condition, modifierType)
        { value = newValue; }

        public override void ModifySpell(ITargetable caster, ITargetable target, SpellModificationInformation spellInformation, Spell spell)
        {
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
    }
}