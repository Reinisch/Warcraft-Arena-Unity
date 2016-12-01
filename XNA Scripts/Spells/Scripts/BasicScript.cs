using System;

using BasicRpgEngine.Characters;
using BasicRpgEngine.Spells;

namespace BasicRpgEngine.Spells
{
    public enum ScriptTriggerType { OnBuffExpired, OnSpellCast, OnSpellHit }

    public abstract class BasicScript : IDisposable
    {
        public Spell Spell { get; protected set; }
        public bool Triggered { get; protected set; }
        public ITargetable ScriptApplier { get; set; }

        public BasicScript(Spell spell)
        {
            if (spell != null)
                Spell = (Spell)spell.Clone();
            Triggered = false;
        }

        public abstract bool Update(TimeSpan elapsedTime, Buff buff, SpellData spellData, ITargetable scriptTarget);

        public abstract object Clone(ITargetable scriptApplier);
        public abstract void Dispose();
    }
}