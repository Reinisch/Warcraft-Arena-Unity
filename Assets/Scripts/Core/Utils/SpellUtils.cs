using System;

namespace Core
{
    public static class SpellUtils
    {
        public static bool HasTargetFlag(this SpellInterruptFlags baseFlags, SpellInterruptFlags flag)
        {
            return (baseFlags & flag) == flag;
        }

        public static bool HasTargetFlag(this AuraInterruptFlags baseFlags, AuraInterruptFlags flag)
        {
            return (baseFlags & flag) == flag;
        }

        public static bool HasAnyFlag(this AuraInterruptFlags baseFlags, AuraInterruptFlags flag)
        {
            return (baseFlags & flag) != 0;
        }

        public static bool HasTargetFlag(this SpellSchoolMask baseFlags, SpellSchoolMask flag)
        {
            return (baseFlags & flag) == flag;
        }

        public static bool HasAnyFlag(this SpellSchoolMask baseFlags, SpellSchoolMask flag)
        {
            return (baseFlags & flag) != 0;
        }

        public static bool HasAnyFlag(this SpellTriggerFlags baseFlags, SpellTriggerFlags flag)
        {
            return (baseFlags & flag) != 0;
        }

        public static bool HasTargetFlag(this HitType baseFlags, HitType flag)
        {
            return (baseFlags & flag) == flag;
        }

        public static bool HasTargetFlag(this SpellMechanicsFlags baseFlags, SpellMechanicsFlags flag)
        {
            return (baseFlags & flag) == flag;
        }

        public static bool HasAnyFlag(this SpellMechanicsFlags baseFlags, SpellMechanicsFlags flag)
        {
            return (baseFlags & flag) != 0;
        }

        public static bool HasTargetFlag(this UnitVisualEffectFlags baseFlags, UnitVisualEffectFlags flag)
        {
            return (baseFlags & flag) == flag;
        }

        public static bool HasAnyFlag(this UnitVisualEffectFlags baseFlags, UnitVisualEffectFlags flag)
        {
            return (baseFlags & flag) != 0;
        }

        public static UnitVisualEffectFlags SetFlag(this UnitVisualEffectFlags baseFlags, UnitVisualEffectFlags flag, bool set)
        {
            return set ? baseFlags | flag : baseFlags & ~flag;
        }

        public static bool HasTargetFlag(this SpellPreventionType baseFlags, SpellPreventionType flag)
        {
            return (baseFlags & flag) == flag;
        }

        public static bool HasAnyFlag(this SpellPreventionType baseFlags, SpellPreventionType flag)
        {
            return (baseFlags & flag) != 0;
        }

        public static bool HasTargetFlag(this SpellCastFlags baseFlags, SpellCastFlags flag)
        {
            return (baseFlags & flag) == flag;
        }

        public static bool HasTargetFlag(this SpellRangeFlags baseFlags, SpellRangeFlags flag)
        {
            return (baseFlags & flag) == flag;
        }

        public static bool HasTargetFlag(this SpellCastTargetFlags baseFlags, SpellCastTargetFlags flag)
        {
            return (baseFlags & flag) == flag;
        }

        public static bool HasAnyFlag(this SpellCastTargetFlags baseFlags, SpellCastTargetFlags flag)
        {
            return (baseFlags & flag) != 0;
        }

        public static AuraStateFlags AsFlag(this AuraStateType auraStateType)
        {
            switch (auraStateType)
            {
                case AuraStateType.None:
                    return 0;
                case AuraStateType.Frozen:
                    return AuraStateFlags.Frozen;
                case AuraStateType.Defense:
                    return AuraStateFlags.Defense;
                case AuraStateType.Berserking:
                    return AuraStateFlags.Berserking;
                case AuraStateType.Judgement:
                    return AuraStateFlags.Judgement;
                case AuraStateType.Conflagrate:
                    return AuraStateFlags.Conflagrate;
                case AuraStateType.Swiftmend:
                    return AuraStateFlags.Swiftmend;
                case AuraStateType.DeadlyPoison:
                    return AuraStateFlags.DeadlyPoison;
                case AuraStateType.Enrage:
                    return AuraStateFlags.Enrage;
                case AuraStateType.Bleeding:
                    return AuraStateFlags.Bleeding;
                default:
                    throw new ArgumentOutOfRangeException(nameof(auraStateType), auraStateType, null);
            }
        }

        public static SpellMechanicsFlags AsFlag(this SpellMechanics mechanics)
        {
            switch (mechanics)
            {
                case SpellMechanics.None:
                    return 0;
                case SpellMechanics.Charm:
                    return SpellMechanicsFlags.Charm;
                case SpellMechanics.Disoriented:
                    return SpellMechanicsFlags.Disoriented;
                case SpellMechanics.Disarm:
                    return SpellMechanicsFlags.Disarm;
                case SpellMechanics.Distract:
                    return SpellMechanicsFlags.Distract;
                case SpellMechanics.Fear:
                    return SpellMechanicsFlags.Fear;
                case SpellMechanics.Grip:
                    return SpellMechanicsFlags.Grip;
                case SpellMechanics.Root:
                    return SpellMechanicsFlags.Root;
                case SpellMechanics.SlowAttack:
                    return SpellMechanicsFlags.SlowAttack;
                case SpellMechanics.Silence:
                    return SpellMechanicsFlags.Silence;
                case SpellMechanics.Sleep:
                    return SpellMechanicsFlags.Sleep;
                case SpellMechanics.Snare:
                    return SpellMechanicsFlags.Snare;
                case SpellMechanics.Stun:
                    return SpellMechanicsFlags.Stun;
                case SpellMechanics.Freeze:
                    return SpellMechanicsFlags.Freeze;
                case SpellMechanics.Knockout:
                    return SpellMechanicsFlags.Knockout;
                case SpellMechanics.Bleed:
                    return SpellMechanicsFlags.Bleed;
                case SpellMechanics.Bandage:
                    return SpellMechanicsFlags.Bandage;
                case SpellMechanics.Polymorph:
                    return SpellMechanicsFlags.Polymorph;
                case SpellMechanics.Banish:
                    return SpellMechanicsFlags.Banish;
                case SpellMechanics.Shield:
                    return SpellMechanicsFlags.Shield;
                case SpellMechanics.Shackle:
                    return SpellMechanicsFlags.Shackle;
                case SpellMechanics.Mount:
                    return SpellMechanicsFlags.Mount;
                case SpellMechanics.Infected:
                    return SpellMechanicsFlags.Infected;
                case SpellMechanics.Horror:
                    return SpellMechanicsFlags.Horror;
                case SpellMechanics.Invulnerability:
                    return SpellMechanicsFlags.Invulnerability;
                case SpellMechanics.Interrupt:
                    return SpellMechanicsFlags.Interrupt;
                case SpellMechanics.Daze:
                    return SpellMechanicsFlags.Daze;
                case SpellMechanics.ImmuneShield:
                    return SpellMechanicsFlags.ImmuneShield;
                case SpellMechanics.Sapped:
                    return SpellMechanicsFlags.Sapped;
                case SpellMechanics.Enraged:
                    return SpellMechanicsFlags.Enraged;
                case SpellMechanics.Wounded:
                    return SpellMechanicsFlags.Wounded;
                default:
                    throw new ArgumentOutOfRangeException(nameof(mechanics), mechanics, null);
            }
        }
    }
}