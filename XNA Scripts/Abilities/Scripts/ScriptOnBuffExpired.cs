using System;

using BasicRpgEngine.Characters;
using BasicRpgEngine.Spells;

namespace BasicRpgEngine.Spells
{
    public class ScriptOnBuffExpired : BasicScript
    {
        public ScriptOnBuffExpired(Spell spell)
            :base(spell)
        {}

        public override bool Update(TimeSpan elapsedTime, Buff buff, SpellData spellData, ITargetable scriptTarget)
        {
            Triggered = false;
            if (buff == null)
                return false;
            if (buff.TimeLeft == TimeSpan.Zero)
            {
                Triggered = true;
                return true;
            }
            return false;
        }

        public override object Clone(ITargetable scriptApplier)
        {
            ScriptOnBuffExpired script = new ScriptOnBuffExpired(Spell);
            script.ScriptApplier = scriptApplier;
            return script;
        }
        public override void Dispose()
        {
            ScriptApplier = null;
            Spell.Dispose();
        }
    }
}