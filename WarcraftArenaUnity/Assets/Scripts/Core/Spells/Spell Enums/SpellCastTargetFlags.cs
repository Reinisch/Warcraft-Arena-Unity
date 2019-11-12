using System;
using UnityEngine;

namespace Core
{
    [Flags]
    public enum SpellCastTargetFlags
    {
        SourceLocation = 1 << 0,
        DestLocation = 1 << 1,
        UnitEnemy = 1 << 2,
        UnitAlly = 1 << 3,

        [HideInInspector]
        UnitMask = UnitEnemy | UnitAlly,
    }
}
