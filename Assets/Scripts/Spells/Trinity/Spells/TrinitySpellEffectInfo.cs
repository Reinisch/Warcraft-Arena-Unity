using UnityEngine;
using System.Collections;

public class TrinitySpellEffectInfo
{
    TrinitySpellInfo spellInfo;

    public int EffectIndex { get; set; }
    public int Effect { get; set; }
    public int ApplyAuraName { get; set; }
    public int ApplyAuraPeriod { get; set; }
    public int DieSides { get; set; }

    public float RealPointsPerLevel { get; set; }
    public int BasePoints { get; set; }
    public float PointsPerResource { get; set; }
    public float Amplitude { get; set; }
    public float ChainAmplitude { get; set; }
    public float BonusCoefficient { get; set; }

    public int MiscValue { get; set; }
    public int MiscValueB { get; set; }
    public Mechanics Mechanic { get; set; }

    public float PositionFacing { get; set; }
    public SpellRadiusEntry RadiusEntry { get; set; }
    public SpellRadiusEntry MaxRadiusEntry { get; set; }

    public int ChainTargets { get; set; }
    public int ItemType { get; set; }
    public int TriggerSpell { get; set; }
    public float BonusCoefficientFromAP { get; set; }
}
