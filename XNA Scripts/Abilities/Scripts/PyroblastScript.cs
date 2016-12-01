using System;

using BasicRpgEngine.Characters;
using BasicRpgEngine.Spells;

namespace BasicRpgEngine.Spells
{
    public class PyroblastScript: BasicScript
    {
        Spell pyroblastTriggered;
        Spell heatingUpTriggered;

        public PyroblastScript(Spell pyroblastTriggerSpell, Spell heatingUpTriggerSpell)
            :base(heatingUpTriggerSpell)
        {
            if (pyroblastTriggerSpell != null)
                pyroblastTriggered = (Spell)pyroblastTriggerSpell.Clone();
            heatingUpTriggered = heatingUpTriggerSpell;
        }

        public override bool Update(TimeSpan elapsedTime, Buff buff, SpellData spellData, ITargetable scriptOwner)
        {
            Triggered = false;
            if (spellData == null || scriptOwner == null)
                return false;
            if (11 == spellData.ID || 10 == spellData.ID || 19 == spellData.ID || 9 == spellData.ID)
            {
                Buff newBuff;
                Triggered = true;
                newBuff = scriptOwner.Character.Entity.Buffs.Find(item => item.Id == 17);
                if (newBuff != null)
                {
                    Spell = pyroblastTriggered;
                    newBuff.NeedsRemoval = true;
                }
                else
                    Spell = heatingUpTriggered;
                return true;
            }
            return false;
        }

        public override object Clone(ITargetable scriptApplier)
        {
            PyroblastScript script = new PyroblastScript(pyroblastTriggered, heatingUpTriggered);
            script.ScriptApplier = scriptApplier;
            return script;
        }
        public override void Dispose()
        {
            ScriptApplier = null;
            pyroblastTriggered.Dispose();
            heatingUpTriggered.Dispose();
        }
    }
}