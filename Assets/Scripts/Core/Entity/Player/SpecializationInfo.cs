using System.Collections.Generic;

namespace Core
{
    public class SpecializationInfo
    {
        public uint ResetTalentsCost;
        public long ResetTalentsTime;
        public uint PrimarySpecialization;
        public byte ActiveGroup;

        public List<Dictionary<uint, PlayerSpellState>> Talents;
        public List<List<uint>> Glyphs;


        public SpecializationInfo()
        {
            Talents = new List<Dictionary<uint, PlayerSpellState>>(UnitHelper.MaxSpecializations);
            Glyphs = new List<List<uint>>(UnitHelper.MaxSpecializations);
        }
    }
}