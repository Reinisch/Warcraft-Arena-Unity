using System;

using BasicRpgEngine.Characters;

namespace BasicRpgEngine.Spells
{
    public delegate bool ModifierCondition(ITargetable caster, ITargetable target);

    public enum SpellModifierType { CriticalChanceMultiplier, CriticalChanceAddition,
        CriticalDamageMultiplier, DamageMultiplier, SpellCast, AdditionalDamage,
        InstantCast, CastFailed};

    public abstract class SpellModifier: IDisposable
    {
        protected ModifierCondition modifierCondition;

        public int Id { get; protected set; }
        public SpellModifierType ModifierType { get; protected set; }
        public bool NeedRemoval { get; set; }

        public SpellModifier(int id, ModifierCondition condition, SpellModifierType modifierType)
        {
            Id = id;
            modifierCondition = condition;
            ModifierType = modifierType;
            NeedRemoval = false;
        }

        public abstract void ModifySpell(ITargetable caster, ITargetable target, SpellModificationInformation spellInformation, Spell spell);
        public abstract bool CheckForFailure(ITargetable caster, ITargetable target, SpellModificationInformation spellInformation, Spell spell);
        public abstract bool CheckForInstantCast(ITargetable caster, ITargetable target, SpellModificationInformation spellInformation, Spell spell);

        public virtual void Dispose()
        {
            NeedRemoval = true;
        }
    }
}