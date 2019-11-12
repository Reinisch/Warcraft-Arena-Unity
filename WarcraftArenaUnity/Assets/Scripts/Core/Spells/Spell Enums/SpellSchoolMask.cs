using System;

namespace Core
{
    [Flags]
    public enum SpellSchoolMask
    {
        Normal = (1 << SpellSchools.Normal),
        Holy = (1 << SpellSchools.Holy),
        Fire = (1 << SpellSchools.Fire),
        Nature = (1 << SpellSchools.Nature),
        Frost = (1 << SpellSchools.Frost),
        Shadow = (1 << SpellSchools.Shadow),
        Arcane = (1 << SpellSchools.Arcane),

        SpellMask = (Fire | Nature | Frost | Shadow | Arcane),
        MagicMask = (Holy | SpellMask),
    }
}
