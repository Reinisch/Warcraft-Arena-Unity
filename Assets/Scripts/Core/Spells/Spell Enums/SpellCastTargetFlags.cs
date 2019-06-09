using System;
using UnityEngine;

namespace Core
{
    [Flags]
    public enum SpellCastTargetFlags
    {
        Unit = 1 << 0,
        SourceLocation = 1 << 1,
        DestLocation = 1 << 2,
        UnitEnemy = 1 << 3,
        UnitAlly = 1 << 4,
        GameEntity = 1 << 5,

        [HideInInspector]
        UnitMask = Unit | UnitEnemy | UnitAlly,
    }
}
