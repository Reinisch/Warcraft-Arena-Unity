using System;
using System.Collections.Generic;

namespace Core
{
    public class TrainerSpellData
    {
        public Dictionary<uint, TrainerSpell> Spells;
        // req. for correct show non-prof. trainers like weaponmaster, allowed values 0 and 2.
        // trainer type based at trainer spells, can be different from creature_template value.
        public uint TrainerType;


        public TrainerSpellData()
        {
            TrainerType = 0;
        }

        public TrainerSpell Find(uint spellId) { throw new NotImplementedException(); }
    }
}