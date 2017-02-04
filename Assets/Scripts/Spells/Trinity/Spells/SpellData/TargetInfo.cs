using System;

public class TargetInfo
{
    public Guid TargetId { get; set; }
    public float Delay { get; set; }
    public SpellMissInfo MissCondition { get; set; }
    public SpellMissInfo ReflectResult { get; set; }
    public int EffectMask { get; set; }
    public bool Processed { get; set; }
    public bool Alive { get; set; }
    public bool Crit { get; set; }
    public bool ScaleAura { get; set; }
    public int Damage { get; set; }
};
