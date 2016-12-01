using BasicRpgEngine.Characters;

namespace BasicRpgEngine.Spells
{
    public class SpellTrigger : IBuffDependantModifier
    {
        public bool IsTriggered { get; protected set; }
        public float TriggerChance { get; private set; }
        public byte TriggeredSpellId { get; private set; }
        public string SpellName { get; private set; }
        public Buff BuffRef { get; set; }
        public bool NeedRemoval { get; set; }

        public SpellTrigger(string newSpellName, float triggerChance, byte triggeredSpellId)
        {
            SpellName = newSpellName;
            TriggerChance = triggerChance;
            TriggeredSpellId = triggeredSpellId;
            IsTriggered = false;
        }

        public bool CheckTrigger(ITargetable caster, ITargetable target, SpellModificationInformation spellInformation, Spell spell)
        {
            IsTriggered = false;
            if (BuffRef.NeedsRemoval)
            {
                caster.Character.Entity.Buffs.Remove(BuffRef);
                BuffRef = null;
                NeedRemoval = true;
                return false;
            }
            IsTriggered = Mechanics.Roll(0, 10000) < TriggerChance * 100;
            if (IsTriggered && BuffRef.HasStucks)
            {
                BuffRef.StuckRemoved();
                if (BuffRef.NeedsRemoval)
                {
                    caster.Character.Entity.Buffs.Remove(BuffRef);
                    NeedRemoval = true;
                    BuffRef = null;
                }
            }
            return IsTriggered;
        }

        public object Clone()
        {
            return new SpellTrigger(SpellName, TriggerChance, TriggeredSpellId);
        }
        public void Dispose()
        {
            BuffRef = null;
        }
    }
}