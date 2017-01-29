using System;

public struct SpellProcsPerMinuteModEntry
{
    public int Id;
    public float Coeff;
    public int Param;
    public int Type;
    public int SpellProcsPerMinuteID;
};

public struct SpellCooldownsEntry
{
    public int Id;
    public int SpellId;
    public int CategoryRecoveryTime;
    public int RecoveryTime;
    public int StartRecoveryTime;
    public int DifficultyId;
};

public struct SpellValue
{
    public int[] EffectBasePoints;
    public int MaxAffectedTargets;
    public float RadiusMod;
    public int AuraStackAmount;

    public SpellValue(TrinitySpellInfo spellInfo)
    {
        EffectBasePoints = new int[SpellHelper.MaxSpellEffects];

        for (int i = 0; i < spellInfo.SpellEffectInfos.Count; i++)
            if (spellInfo.SpellEffectInfos[i] != null)
                EffectBasePoints[spellInfo.SpellEffectInfos[i].EffectIndex] = spellInfo.SpellEffectInfos[i].BasePoints;

        MaxAffectedTargets = spellInfo.MaxAffectedTargets;
        RadiusMod = 1.0f;
        AuraStackAmount = 1;
    }
};

public struct PowerCostData
{
    Powers Power;
    int Amount;
};

public class SpellDamage
{
    public Unit Target;
    public Unit Attacker;
    public Guid CastId;
    public int SpellID;
    public int Damage;
    public int Absorb;
    public int Resist;
    public bool Crit;
    public bool PeriodicLog;

    public SpellSchoolMask SchoolMask;
    public HitInfo HitInfo;

    public SpellDamage(Unit attacker, Unit target, int _SpellID, SpellSchoolMask _schoolMask, Guid _castId = default(Guid))
    {
        Attacker = attacker;
        Target = target;
        CastId = _castId;
        SpellID = _SpellID;

        SchoolMask = _schoolMask;

        Damage = 0;
        Absorb = 0;
        Resist = 0;
        Crit = false;
        PeriodicLog = false;

        HitInfo = HitInfo.AffectsVictim;
    }
};

public struct CleanDamage
{
    public int AbsorbedDamage;
    public int MitigatedDamage;

    public CleanDamage(int absorb, int mitigated)
    {
        AbsorbedDamage = absorb;
        MitigatedDamage = mitigated;
    }
};