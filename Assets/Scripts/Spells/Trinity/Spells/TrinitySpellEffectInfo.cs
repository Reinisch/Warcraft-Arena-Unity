using UnityEngine;
using System.Collections;

public class TrinitySpellEffectInfo
{
    TrinitySpellInfo spellInfo;

    public int EffectIndex { get; set; }
    public SpellEffectType Effect { get; set; }
    public AuraType AuraType { get; set; }
    public int ApplyAuraPeriod { get; set; }
    public int DieSides { get; set; }

    public TargetTypes TargetA { get; set; }
    public TargetTypes TargetB { get; set; }

    public int BasePoints { get; set; }
    public float PointsPerResource { get; set; }
    public float Amplitude { get; set; }
    public float ChainAmplitude { get; set; }
    public float BonusCoefficient { get; set; }

    public int MiscValue { get; set; }
    public int MiscValueB { get; set; }
    public Mechanics Mechanic { get; set; }

    public float PositionFacing { get; set; }
    public SpellRadius Radius { get; set; }

    public int TriggerSpell { get; set; }
    public float BonusCoefficientFromAP { get; set; }

    public TrinitySpellEffectInfo(TrinitySpellInfo info, int effectIndex, TrinitySpellEffectInfoEntry effectEntry)
    {
        spellInfo = info;
        EffectIndex = effectIndex;

        Effect = effectEntry.Effect;
        AuraType = effectEntry.AuraType;
        ApplyAuraPeriod = effectEntry.ApplyAuraPeriod;
        DieSides = effectEntry.DieSides;

        TargetA = effectEntry.TargetA;
        TargetB = effectEntry.TargetB;

        BasePoints = effectEntry.BasePoints;
        PointsPerResource = effectEntry.PointsPerResource;
        Amplitude = effectEntry.Amplitude;
        ChainAmplitude = effectEntry.ChainAmplitude;

        MiscValue = effectEntry.MiscValue;
        MiscValueB = effectEntry.MiscValueB;
        Mechanic = effectEntry.Mechanic;

        PositionFacing = effectEntry.PositionFacing;
        Radius = effectEntry.RadiusEntryId > 0 ? WarcraftDatabase.SpellRadiuses[effectEntry.RadiusEntryId] : null;

        TriggerSpell = effectEntry.TriggerSpell;
        BonusCoefficient = effectEntry.BonusCoefficient;
        BonusCoefficientFromAP = effectEntry.BonusCoefficientFromAP;
    }

    public float CalcRadius(Unit caster, TrinitySpell spell)
    {
        if (Radius == null)
            return 0.0f;

        float radius = Radius.RadiusMin;

        if (radius == 0.0f)
            radius = Radius.RadiusMax;

        return radius;
    }
}
