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

    public bool IsAura()
    {
        return (IsUnitOwnedAuraEffect() || Effect == SpellEffectType.PERSISTENT_AREA_AURA) && AuraType != AuraType.SPELL_AURA_NONE;
    }

    public bool IsAura(AuraType aura)
    {
        return IsAura() && AuraType == aura;
    }

    public bool IsTargetingArea()
    {
        return TargetA.IsArea() || TargetB.IsArea();
    }

    public bool IsAreaAuraEffect()
    {
        if (Effect == SpellEffectType.APPLY_AREA_AURA_PARTY    ||
            Effect == SpellEffectType.APPLY_AREA_AURA_RAID     ||
            Effect == SpellEffectType.APPLY_AREA_AURA_FRIEND   ||
            Effect == SpellEffectType.APPLY_AREA_AURA_ENEMY    ||
            Effect == SpellEffectType.APPLY_AREA_AURA_PET      ||
            Effect == SpellEffectType.APPLY_AREA_AURA_OWNER)
            return true;
        return false;
    }

    public bool IsFarUnitTargetEffect()
    {
        return (Effect == SpellEffectType.SUMMON_PLAYER)
            || (Effect == SpellEffectType.SUMMON_RAF_FRIEND)
            || (Effect == SpellEffectType.RESURRECT)
            || (Effect == SpellEffectType.SKIN_PLAYER_CORPSE);
    }

    public bool IsFarDestTargetEffect()
    {
        return Effect == SpellEffectType.TELEPORT_UNITS;
    }

    public bool IsUnitOwnedAuraEffect()
    {
        return IsAreaAuraEffect() || Effect == SpellEffectType.APPLY_AURA;
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

    public int CalcValue(Unit caster, int bp, Unit target, float variance)
    {
        int basePoints = bp > 0 ? bp : BasePoints;
        float comboDamage = PointsPerResource;

        // base amount modification based on spell lvl vs caster lvl
        if (BonusCoefficient != 0.0f && caster != null)
        {
            float effectValue = caster.Character[StatType.SpellPower];

            effectValue *= BonusCoefficient;
            if (effectValue != 0.0f && effectValue < 1.0f)
                effectValue = 1.0f;

            basePoints = (int)effectValue;
        }
        else
        {
            // roll in a range <1;EffectDieSides> as of patch 3.3.3
            int randomPoints = DieSides;
            switch (randomPoints)
            {
                case 0: break;
                case 1: basePoints += 1; break;                     // range 1..1
                default:
                {
                    // range can have positive (1..rand) and negative (rand..1) values, so order its for irand
                    int randvalue = (randomPoints >= 1)
                        ? Random.Range(1, randomPoints)
                        : Random.Range(randomPoints, 1);

                    basePoints += randvalue;
                    break;
                }
            }
        }

        float value = basePoints;

        // random damage
        if (caster)
            value = caster.ApplyEffectModifiers(spellInfo, EffectIndex, value);

        return (int)value;
    }
}
