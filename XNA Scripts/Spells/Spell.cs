using System;
using System.Collections.Generic;

namespace BasicRpgEngine.Spells
{
    public class Spell : ICloneable, IDisposable
    {
        public byte SpellDataId
        {
            get; private set;
        }
        public string Name
        {
            get; private set;
        }
        public Cooldown SpellCooldown
        {
            get; private set;
        }
        public List<SpellModifier> Modifiers
        {
            get; private set;
        }
        public List<SpellTrigger> Triggers
        {
            get; private set;
        }

        public Spell(byte spellDataId, string name, Cooldown spellCooldown)
        {
            SpellDataId = spellDataId;
            Name = name;
            SpellCooldown = (Cooldown)spellCooldown.Clone();
            Modifiers = new List<SpellModifier>();
            Triggers = new List<SpellTrigger>();
        }

        public object Clone()
        {
            return new Spell(SpellDataId, Name, SpellCooldown);
        }
        public void Dispose()
        {
            foreach (SpellModifier modifier in Modifiers)
                modifier.Dispose();
        }
    }
}