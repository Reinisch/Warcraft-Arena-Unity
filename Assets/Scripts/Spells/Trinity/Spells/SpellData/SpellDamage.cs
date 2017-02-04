using System;

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

    public SpellDamage(Unit attacker, Unit target, int spellId, SpellSchoolMask schoolMask, Guid castId = default(Guid))
    {
        Attacker = attacker;
        Target = target;
        CastId = castId;
        SpellID = spellId;

        SchoolMask = schoolMask;

        Damage = 0;
        Absorb = 0;
        Resist = 0;
        Crit = false;
        PeriodicLog = false;

        HitInfo = HitInfo.AffectsVictim;
    }
};