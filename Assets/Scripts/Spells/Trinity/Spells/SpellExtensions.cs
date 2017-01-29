using System.Collections.Generic;

public static class SpellExtensions
{
    private struct StaticTargetData
    {
        public TargetObjects ObjectType { get; private set; }               // type of object returned by target type
        public TargetReferences ReferenceType { get; private set; }         // defines which object is used as a reference when selecting target
        public TargetSelections SelectionCategory { get; private set; }     // defines area selection type
        public TargetChecks SelectionCheckType { get; private set; }        // defines selection criteria
        public TargetDirections DirectionType { get; private set; }         // direction for cone and dest targets

        public StaticTargetData(TargetObjects objType, TargetReferences refType, TargetSelections selection, TargetChecks checkType, TargetDirections direction) : this()
        {
            ObjectType = objType;
            ReferenceType = refType;
            SelectionCategory = selection;
            SelectionCheckType = checkType;
            DirectionType = direction;
        }
    };

    #region Target Info

    #region All target types

    private static readonly Dictionary<TargetTypes, StaticTargetData> TargetTypeEntries = new Dictionary <TargetTypes, StaticTargetData>
    {       
        { TargetTypes.TargetUnitCaster,  new StaticTargetData(TargetObjects.Unit, TargetReferences.Caster, TargetSelections.Default, TargetChecks.Default, TargetDirections.None) },
        { TargetTypes.TargetUnitNearbyEnemy, new StaticTargetData(TargetObjects.Unit, TargetReferences.Caster, TargetSelections.Nearby,  TargetChecks.Enemy, TargetDirections.None) },
        { TargetTypes.TargetUnitNearbyAlly, new StaticTargetData(TargetObjects.Unit, TargetReferences.Caster, TargetSelections.Nearby,  TargetChecks.Ally, TargetDirections.None) },
        { TargetTypes.TargetUnitTargetEnemy, new StaticTargetData(TargetObjects.Unit, TargetReferences.Target, TargetSelections.Default, TargetChecks.Enemy, TargetDirections.None) },
        { TargetTypes.TargetUnitSrcAreaEntry, new StaticTargetData(TargetObjects.Unit, TargetReferences.Source, TargetSelections.Area, TargetChecks.Entry, TargetDirections.None) },
        { TargetTypes.TargetUnitDestAreaEntry, new StaticTargetData(TargetObjects.Unit, TargetReferences.Dest, TargetSelections.Area, TargetChecks.Entry, TargetDirections.None) },
        { TargetTypes.TargetUnitSrcAreaEnemy, new StaticTargetData(TargetObjects.Unit, TargetReferences.Source, TargetSelections.Area, TargetChecks.Enemy, TargetDirections.None) },
        { TargetTypes.TargetUnitDestAreaEnemy, new StaticTargetData(TargetObjects.Unit, TargetReferences.Dest, TargetSelections.Area, TargetChecks.Enemy, TargetDirections.None) },
        { TargetTypes.TargetDestCaster, new StaticTargetData(TargetObjects.Dest, TargetReferences.Caster, TargetSelections.Default, TargetChecks.Default, TargetDirections.None) },
        { TargetTypes.TargetUnitTargetAlly, new StaticTargetData(TargetObjects.Unit, TargetReferences.Target, TargetSelections.Default, TargetChecks.Ally, TargetDirections.None) },
        { TargetTypes.TargetSrcCaster, new StaticTargetData(TargetObjects.Source,  TargetReferences.Caster, TargetSelections.Default, TargetChecks.Default, TargetDirections.None) },
        { TargetTypes.TargetUnitConeEnemy24, new StaticTargetData(TargetObjects.Unit, TargetReferences.Caster, TargetSelections.Cone, TargetChecks.Enemy, TargetDirections.Front) },
        { TargetTypes.TargetUnitTargetAny, new StaticTargetData(TargetObjects.Unit, TargetReferences.Target, TargetSelections.Default, TargetChecks.Default, TargetDirections.None) },
        { TargetTypes.TargetUnitSrcAreaAlly, new StaticTargetData(TargetObjects.Unit, TargetReferences.Source, TargetSelections.Area, TargetChecks.Ally, TargetDirections.None) },
        { TargetTypes.TargetUnitDestAreaAlly, new StaticTargetData(TargetObjects.Unit, TargetReferences.Dest, TargetSelections.Area, TargetChecks.Ally, TargetDirections.None) },
        { TargetTypes.TargetUnitNearbyEntry, new StaticTargetData(TargetObjects.Unit, TargetReferences.Caster, TargetSelections.Nearby, TargetChecks.Entry, TargetDirections.None) },
        { TargetTypes.TargetDestCasterFrontRight, new StaticTargetData(TargetObjects.Dest, TargetReferences.Caster, TargetSelections.Default, TargetChecks.Default, TargetDirections.FrontRight) },
        { TargetTypes.TargetDestCasterBackRight, new StaticTargetData(TargetObjects.Dest, TargetReferences.Caster, TargetSelections.Default, TargetChecks.Default, TargetDirections.BackRight) },
        { TargetTypes.TargetDestCasterBackLeft, new StaticTargetData(TargetObjects.Dest, TargetReferences.Caster, TargetSelections.Default, TargetChecks.Default, TargetDirections.BackLeft) },
        { TargetTypes.TargetDestCasterFrontLeft, new StaticTargetData(TargetObjects.Dest, TargetReferences.Caster, TargetSelections.Default, TargetChecks.Default, TargetDirections.FrontLeft) },
        { TargetTypes.TargetUnitTargetChainhealAlly, new StaticTargetData(TargetObjects.Unit, TargetReferences.Target, TargetSelections.Default, TargetChecks.Ally, TargetDirections.None) },
        { TargetTypes.TargetDestNearbyEntry, new StaticTargetData(TargetObjects.Dest, TargetReferences.Caster, TargetSelections.Nearby, TargetChecks.Entry, TargetDirections.None) },
        { TargetTypes.TargetDestCasterFront, new StaticTargetData(TargetObjects.Dest, TargetReferences.Caster, TargetSelections.Default, TargetChecks.Default, TargetDirections.Front) },
        { TargetTypes.TargetDestCasterBack, new StaticTargetData(TargetObjects.Dest, TargetReferences.Caster, TargetSelections.Default, TargetChecks.Default, TargetDirections.Back) },
        { TargetTypes.TargetDestCasterRight, new StaticTargetData(TargetObjects.Dest, TargetReferences.Caster, TargetSelections.Default, TargetChecks.Default, TargetDirections.Right) },
        { TargetTypes.TargetDestCasterLeft, new StaticTargetData(TargetObjects.Dest, TargetReferences.Caster, TargetSelections.Default, TargetChecks.Default, TargetDirections.Left) },
        { TargetTypes.TargetDestTargetEnemy, new StaticTargetData(TargetObjects.Dest, TargetReferences.Target, TargetSelections.Default, TargetChecks.Enemy, TargetDirections.None) },
        { TargetTypes.TargetUnitConeEnemy54, new StaticTargetData(TargetObjects.Unit, TargetReferences.Caster, TargetSelections.Cone, TargetChecks.Enemy, TargetDirections.Front) },
        { TargetTypes.TargetDestCasterFrontLeap, new StaticTargetData(TargetObjects.Dest, TargetReferences.Caster, TargetSelections.Default, TargetChecks.Default, TargetDirections.None) },
        { TargetTypes.TargetUnitConeAlly, new StaticTargetData(TargetObjects.Unit, TargetReferences.Caster, TargetSelections.Cone, TargetChecks.Ally, TargetDirections.Front) },
        { TargetTypes.TargetUnitConeEntry, new StaticTargetData(TargetObjects.Unit, TargetReferences.Caster, TargetSelections.Cone, TargetChecks.Entry, TargetDirections.Front) },
        { TargetTypes.TargetDestTargetAny, new StaticTargetData(TargetObjects.Dest, TargetReferences.Target, TargetSelections.Default, TargetChecks.Default, TargetDirections.None) },
        { TargetTypes.TargetDestTargetFront, new StaticTargetData(TargetObjects.Dest, TargetReferences.Target, TargetSelections.Default, TargetChecks.Default, TargetDirections.Front) },
        { TargetTypes.TargetDestTargetBack, new StaticTargetData(TargetObjects.Dest, TargetReferences.Target, TargetSelections.Default, TargetChecks.Default, TargetDirections.Back) },
        { TargetTypes.TargetDestTargetRight, new StaticTargetData(TargetObjects.Dest, TargetReferences.Target, TargetSelections.Default, TargetChecks.Default, TargetDirections.Right) },
        { TargetTypes.TargetDestTargetLeft, new StaticTargetData(TargetObjects.Dest, TargetReferences.Target, TargetSelections.Default, TargetChecks.Default, TargetDirections.Left) },
        { TargetTypes.TargetDestTargetFrontRight, new StaticTargetData(TargetObjects.Dest, TargetReferences.Target, TargetSelections.Default, TargetChecks.Default, TargetDirections.FrontRight) }, 
        { TargetTypes.TargetDestTargetBackRight, new StaticTargetData(TargetObjects.Dest, TargetReferences.Target, TargetSelections.Default, TargetChecks.Default, TargetDirections.BackRight) },
        { TargetTypes.TargetDestTargetBackLeft, new StaticTargetData(TargetObjects.Dest, TargetReferences.Target, TargetSelections.Default, TargetChecks.Default, TargetDirections.BackLeft) },
        { TargetTypes.TargetDestTargetFrontLeft, new StaticTargetData(TargetObjects.Dest, TargetReferences.Target, TargetSelections.Default, TargetChecks.Default, TargetDirections.FrontLeft) },
        { TargetTypes.TargetDestCasterRandom, new StaticTargetData(TargetObjects.Dest, TargetReferences.Caster, TargetSelections.Default, TargetChecks.Default, TargetDirections.Random) },
        { TargetTypes.TargetDestCasterRadius, new StaticTargetData(TargetObjects.Dest, TargetReferences.Caster, TargetSelections.Default, TargetChecks.Default, TargetDirections.Random) }, 
        { TargetTypes.TargetDestTargetRandom, new StaticTargetData(TargetObjects.Dest, TargetReferences.Target, TargetSelections.Default, TargetChecks.Default, TargetDirections.Random) }, 
        { TargetTypes.TargetDestTargetRadius, new StaticTargetData(TargetObjects.Dest, TargetReferences.Target, TargetSelections.Default, TargetChecks.Default, TargetDirections.Random) },
        { TargetTypes.TargetDestChannelTarget, new StaticTargetData(TargetObjects.Dest, TargetReferences.Caster, TargetSelections.Channel, TargetChecks.Default, TargetDirections.None) },
        { TargetTypes.TargetUnitChannelTarget, new StaticTargetData(TargetObjects.Unit, TargetReferences.Caster, TargetSelections.Channel, TargetChecks.Default, TargetDirections.None) },
        { TargetTypes.TargetDestDestFront, new StaticTargetData(TargetObjects.Dest, TargetReferences.Dest, TargetSelections.Default, TargetChecks.Default, TargetDirections.Front) },
        { TargetTypes.TargetDestDestBack, new StaticTargetData(TargetObjects.Dest, TargetReferences.Dest, TargetSelections.Default, TargetChecks.Default, TargetDirections.Back) },
        { TargetTypes.TargetDestDestRight, new StaticTargetData(TargetObjects.Dest, TargetReferences.Dest, TargetSelections.Default, TargetChecks.Default, TargetDirections.Right) },
        { TargetTypes.TargetDestDestLeft, new StaticTargetData(TargetObjects.Dest, TargetReferences.Dest, TargetSelections.Default, TargetChecks.Default, TargetDirections.Left) },
        { TargetTypes.TargetDestDestFrontRight, new StaticTargetData(TargetObjects.Dest, TargetReferences.Dest, TargetSelections.Default, TargetChecks.Default, TargetDirections.FrontRight) },
        { TargetTypes.TargetDestDestBackRight, new StaticTargetData(TargetObjects.Dest, TargetReferences.Dest, TargetSelections.Default, TargetChecks.Default, TargetDirections.BackRight) },
        { TargetTypes.TargetDestDestBackLeft, new StaticTargetData(TargetObjects.Dest, TargetReferences.Dest, TargetSelections.Default, TargetChecks.Default, TargetDirections.BackLeft) },
        { TargetTypes.TargetDestDestFrontLeft, new StaticTargetData(TargetObjects.Dest, TargetReferences.Dest, TargetSelections.Default, TargetChecks.Default, TargetDirections.FrontLeft) },
        { TargetTypes.TargetDestDestRandom, new StaticTargetData(TargetObjects.Dest, TargetReferences.Dest, TargetSelections.Default, TargetChecks.Default, TargetDirections.Random) },
        { TargetTypes.TargetDestDest, new StaticTargetData(TargetObjects.Dest, TargetReferences.Dest, TargetSelections.Default, TargetChecks.Default, TargetDirections.None) },
        { TargetTypes.TargetDestTraj, new StaticTargetData(TargetObjects.Dest, TargetReferences.Dest, TargetSelections.Default, TargetChecks.Default, TargetDirections.None) },
        { TargetTypes.TargetDestDestRadius, new StaticTargetData(TargetObjects.Dest, TargetReferences.Dest, TargetSelections.Default, TargetChecks.Default, TargetDirections.Random) },
        { TargetTypes.TargetDestChannelCaster, new StaticTargetData(TargetObjects.Dest, TargetReferences.Caster, TargetSelections.Channel, TargetChecks.Default, TargetDirections.None) },
    };

    #endregion

    public static TargetObjects ObjectType(this TargetTypes baseFlags)
    {
        return TargetTypeEntries[baseFlags].ObjectType;
    }

    public static TargetReferences ReferenceType(this TargetTypes baseFlags)
    {
        return TargetTypeEntries[baseFlags].ReferenceType;
    }

    public static TargetSelections Category(this TargetTypes baseFlags)
    {
        return TargetTypeEntries[baseFlags].SelectionCategory;
    }

    public static TargetChecks CheckType(this TargetTypes baseFlags)
    {
        return TargetTypeEntries[baseFlags].SelectionCheckType;
    }

    public static TargetDirections DirectionType(this TargetTypes baseFlags)
    {
        return TargetTypeEntries[baseFlags].DirectionType;
    }

    public static bool IsArea(this TargetTypes baseFlags)
    {
        return baseFlags.Category() == TargetSelections.Area || baseFlags.Category() == TargetSelections.Cone;
    }

    #endregion

    public static bool HasFlag(this TriggerCastFlags baseFlags, TriggerCastFlags flag)
    {
        return (baseFlags & flag) == flag;
    }

    public static bool HasFlag(this SpellRangeFlag baseFlags, SpellRangeFlag flag)
    {
        return (baseFlags & flag) == flag;
    }

    public static bool HasFlag(this SpellCastTargetFlags baseFlags, SpellCastTargetFlags flag)
    {
        return (baseFlags & flag) == flag;
    }

    public static bool HasFlag(this TargetTypes baseFlags, TargetTypes flag)
    {
        return (baseFlags & flag) == flag;
    }
}