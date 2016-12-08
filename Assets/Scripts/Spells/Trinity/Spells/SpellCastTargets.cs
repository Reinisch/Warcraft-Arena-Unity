using UnityEngine;
using System.Collections;
using System;

public class SpellCastTargets
{
    private Unit origTarget;
    private Unit unitTarget;

    public SpellCastTargetFlags TargetMask { get; set; }

    public Unit OrigTarget
    {
        get
        {
            return origTarget;
        }
        set
        {
            origTarget = value;

            if (origTarget != null)
                SourceTransform = origTarget.transform;
        }
    }
    public Unit UnitTarget
    {
        get
        {
            return unitTarget;
        }
        set
        {
            unitTarget = value;

            if (unitTarget != null)
                TargetTransform = unitTarget.transform;
        }
    }

    public Vector3 Source { get; set; }
    public Transform SourceTransform { get; private set; }
    public Transform TargetTransform { get; private set; }

    public float Speed { get; set; }
    public float Pitch { get; set; }

    public float Distance2D { get { return Vector2.Distance(SourceTransform.position, TargetTransform.position); } }
    public float SpeedXY { get { return Speed * Mathf.Cos(Pitch); } }
    public float SpeedZ { get { return Speed * Mathf.Sin(Pitch); } }

    public bool HasSource { get { return (TargetMask & SpellCastTargetFlags.SOURCE_LOCATION) != 0; } }
    public bool HasDest { get { return (TargetMask & SpellCastTargetFlags.DEST_LOCATION) != 0; } }
    public bool HasTrajectory { get { return Speed != 0; } }


    public SpellCastTargets()
    {
    }

    public SpellCastTargets(Unit caster, Unit target)
    {
    }


    public void SetTargetFlag(SpellCastTargetFlags flag) { TargetMask |= flag; }
}
