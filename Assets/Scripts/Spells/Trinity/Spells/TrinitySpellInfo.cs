using UnityEngine;
using System.Collections.Generic;

public class TrinitySpellInfo
{
    public int Id { get; set; }
    public DispelType Dispel { get; set; }
    public Mechanics Mechanic { get; set; }
    public SpellAttributes Attributes { get; set; }
    public TargetTypes Targets { get; set; }
    public SpellSchoolMask SchoolMask { get; set; }
    public SpellDamageClass DamageClass { get; set; }
    public SpellPreventionType PreventionType { get; set; }
    public SpellCastTargetFlags ExplicitTargetMask { get; set; }
    public SpellInterruptFlags InterruptFlags { get; set; }
    public SpellFamilyNames FamilyName { get; set; }

    public SpellChargeCategory ChargeCategory { get; set; }
    public SpellCastTimes CastTime { get; set; }
    public SpellDuration Duration { get; set; }
    public SpellRange Range { get; set; }
    public List<SpellPowerCost> PowerCosts { get; set; }

    public float RecoveryTime { get; set; }
    public float StartRecoveryTime { get; set; }
    public int StartRecoveryCategory { get; set; }

    public float Speed { get; set; }

    public int StackAmount { get; set; }
    public int MaxAffectedTargets { get; set; }

    public int SpellIconId { get; set; }
    public int ActiveIconId { get; set; }
    public int VisualId { get; set; }

    public List<TrinitySpellEffectInfo> SpellEffectInfos { get; set; }

    public TrinitySpellInfo()
    {
        SpellEffectInfos = new List<TrinitySpellEffectInfo>();
    }


    public TrinitySpellEffectInfo GetEffect(int index)
    {
        if (index >= SpellEffectInfos.Count)
            return null;

        return SpellEffectInfos[index];
    }


    public bool HasAttribute(SpellAttributes attribute)
    {
        return (Attributes & attribute) == attribute;
    }
    public bool IsPassive()
    {
        return HasAttribute(SpellAttributes.Passive);
    }
    public bool IsPositive()
    {
        return !HasAttribute(SpellAttributes.Negative);
    }

    public float GetMinRange(bool positive)
    {
        if (Range == null)
            return 0.0f;
        if (positive)
            return Range.MinRangeFriend;
        return Range.MinRangeHostile;
    }

    public float GetMaxRange(bool positive, Unit caster = null, TrinitySpell spell = null)
    {
        if (Range == null)
            return 0.0f;
        float range;
        if (positive)
            range = Range.MaxRangeFriend;
        else
            range = Range.MaxRangeHostile;
        
        return range;
    }
}
