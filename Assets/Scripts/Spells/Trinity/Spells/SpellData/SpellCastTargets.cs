using UnityEngine;

public class SpellCastTargets
{
    public SpellCastTargetFlags TargetMask { get; set; }

    public Unit OrigTarget { get; set; }
    public Unit UnitTarget { get; set; }

    public Vector3 Source { get; set; }
    public Vector3 Dest { get; set; }

    public float Speed { get; set; }
    public float Pitch { get; set; }

    public float Distance2D { get { return Vector2.Distance(Source, Dest); } }
    public float SpeedXY { get { return Speed * Mathf.Cos(Pitch); } }
    public float SpeedZ { get { return Speed * Mathf.Sin(Pitch); } }

    public bool HasSource { get { return (TargetMask & SpellCastTargetFlags.SourceLocation) != 0; } }
    public bool HasDest { get { return (TargetMask & SpellCastTargetFlags.DestLocation) != 0; } }
    public bool HasTrajectory { get { return Speed != 0; } }


    public SpellCastTargets()
    {

    }

    public SpellCastTargets(Unit caster, Unit target)
    {
    }


    public void SetTargetFlag(SpellCastTargetFlags flag) { TargetMask |= flag; }
}
