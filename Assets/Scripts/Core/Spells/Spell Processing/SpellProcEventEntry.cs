namespace Core
{
    public struct SpellProcEventEntry
    {
        public uint SchoolMask;                                 // if nonzero - bit mask for matching proc condition based on spell candidate's school: Fire=2, Mask=1<<(2-1)=2
        public uint SpellFamilyName;                            // if nonzero - for matching proc condition based on candidate spell's SpellFamilyNamer value
        public ulong SpellFamilyMask;                            // if nonzero - for matching proc condition based on candidate spell's SpellFamilyFlags  (like auras 107 and 108 do)
        public uint ProcFlags;                                  // bitmask for matching proc event
        public uint ProcEx;                                     // proc Extend info (see ProcFlagsEx)
        public float PpmRate;                                    // for melee (ranged?) damage spells - proc rate per minute. if zero, falls back to flat chance from Spell.dbc
        public float CustomChance;                               // Owerride chance (in most cases for debug only)
        public uint Cooldown;                                   // hidden cooldown used for some spell proc events, applied to _triggered_spell_
    }
}