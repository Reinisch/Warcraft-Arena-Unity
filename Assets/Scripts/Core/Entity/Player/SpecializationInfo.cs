using System.Collections.Generic;

namespace Core
{
    public class SpecializationInfo
    {
        public uint ResetTalentsCost;
        public long ResetTalentsTime;
        public uint PrimarySpecialization;
        public byte ActiveGroup;

        public List<List<uint>> Glyphs;

        public SpecializationInfo()
        {
            Glyphs = new List<List<uint>>(UnitHelper.MaxSpecializations);
        }
    }
}