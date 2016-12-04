public enum VictimState
{
    Intact,
    Hi,
    Dodge,
    Parry,
    Interrupt,
    Evades,
    Immune,
    Deflects
};

public enum HitInfo
{
    Normalswing =       0x00000000,
    AffectsVictim =     0x00000001,
    Offhand =           0x00000002,
    Miss =              0x00000004,
    FullAbsorb =        0x00000008,
    PartialAbsorb =     0x00000010,
    CriticalHit =       0x00000020,
    RageGain =          0x00000040,
};

public enum DeathState
{
    Alive = 0,
    JustDied = 1,
    Corpse = 2,
    Dead = 3,
    JustRespawned = 4
};

public enum UnitState
{
    None =               0x00000000,
    Died =               0x00000001,
    MeleeAttacking =     0x00000002,
    Stunned =            0x00000008,
    Roaming =            0x00000010,
    Chase =              0x00000020,
    Fleeing =            0x00000080,
    Root =               0x00000400,
    Confused =           0x00000800,
    Distracted =         0x00001000,
    Casting =            0x00008000,
    Charging =           0x00020000,
    Jumping =            0x00040000,
    Move =               0x00100000,
    RoamingMove =        0x00800000,
    ConfusedMove =       0x01000000,
    FleeingMove =        0x02000000,
    ChaseMove =          0x04000000,
    IgnorePathfinding =  0x10000000,

    Moving = RoamingMove | ConfusedMove | FleeingMove | ChaseMove,
    Controlled = Confused | Stunned | Fleeing,
    LostControl = Controlled | Jumping | Charging,
    CannotAutoattack = LostControl | Casting,
    CantMove = Root | Stunned | Died | Distracted,
};

public enum CombatRating
{
    None,
    Dodge,
    CritSpell,
    MultiStrike,
    Speed,
    HasteSpell,
    ArmorPenetration,
    Mastery,
    PvpPower,
};

public enum UnitMoveType
{
    Run,
    RunBack,
    TurnRate,
    PitchRate,
};
