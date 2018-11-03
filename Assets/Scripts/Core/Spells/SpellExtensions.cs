using System;
using System.Collections.Generic;

namespace Core
{
    public static class SpellExtensions
    {
        private struct StaticEffectTargetData
        {
            // defines what target can be added to effect target list if there's no valid target type provided for effect
            public ExplicitTargetTypes ImplicitTargetType { get; private set; }
            // defines valid target object type for spell effect
            public TargetEntities UsedTargetEntityType { get; private set; }


            public StaticEffectTargetData(ExplicitTargetTypes implicitTargetType, TargetEntities usedTargetEntityType) : this()
            {
                ImplicitTargetType = implicitTargetType;
                UsedTargetEntityType = usedTargetEntityType;
            }
        }

        public static SpellCastTargetFlags TargetFlags(this TargetEntities targetEntities)
        {
            switch (targetEntities)
            {
                case TargetEntities.Dest:
                    return SpellCastTargetFlags.DestLocation;
                case TargetEntities.UnitAndDest:
                    return SpellCastTargetFlags.DestLocation | SpellCastTargetFlags.Unit;
                case TargetEntities.Unit:
                    return SpellCastTargetFlags.Unit;
                case TargetEntities.GameEntity:
                    return SpellCastTargetFlags.GameEntity;
                case TargetEntities.Source:
                    return SpellCastTargetFlags.SourceLocation;
                default:
                    return 0;
            }
        }

        public static bool HasFlag(this SpellInterruptFlags baseFlags, SpellInterruptFlags flag)
        {
            return (baseFlags & flag) == flag;
        }

        public static bool HasFlag(this SpellAuraInterruptFlags baseFlags, SpellAuraInterruptFlags flag)
        {
            return (baseFlags & flag) == flag;
        }

        public static bool HasFlag(this SpellChannelInterruptFlags baseFlags, SpellChannelInterruptFlags flag)
        {
            return (baseFlags & flag) == flag;
        }

        public static bool HasFlag(this SpellSchoolMask baseFlags, SpellSchoolMask flag)
        {
            return (baseFlags & flag) == flag;
        }

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

        public static bool HasAnyFlag(this SpellCastTargetFlags baseFlags, SpellCastTargetFlags flag)
        {
            return (baseFlags & flag) != 0;
        }

        #region All effect target types

        private static readonly Dictionary<SpellEffectType, StaticEffectTargetData> EffectTargetTypeEntries = new Dictionary<SpellEffectType, StaticEffectTargetData>
        {
            { SpellEffectType.None, new StaticEffectTargetData(ExplicitTargetTypes.None, TargetEntities.None)},
            { SpellEffectType.Instakill, new StaticEffectTargetData(ExplicitTargetTypes.Explicit, TargetEntities.Unit)},
            { SpellEffectType.SchoolDamage, new StaticEffectTargetData(ExplicitTargetTypes.Explicit, TargetEntities.Unit)},
            { SpellEffectType.Dummy, new StaticEffectTargetData(ExplicitTargetTypes.None, TargetEntities.None)},
            { SpellEffectType.PortalTeleport, new StaticEffectTargetData(ExplicitTargetTypes.None, TargetEntities.None)},
            { SpellEffectType.TeleportDirect, new StaticEffectTargetData(ExplicitTargetTypes.Explicit, TargetEntities.Unit)},
            { SpellEffectType.ApplyAura, new StaticEffectTargetData(ExplicitTargetTypes.Explicit, TargetEntities.Unit)},
            { SpellEffectType.EnvironmentalDamage, new StaticEffectTargetData(ExplicitTargetTypes.Explicit, TargetEntities.Unit)},
            { SpellEffectType.PowerDrain, new StaticEffectTargetData(ExplicitTargetTypes.Explicit, TargetEntities.Unit)},
            { SpellEffectType.HealthLeech, new StaticEffectTargetData(ExplicitTargetTypes.Explicit, TargetEntities.Unit)},
            { SpellEffectType.Heal, new StaticEffectTargetData(ExplicitTargetTypes.Explicit, TargetEntities.Unit)},
            { SpellEffectType.Bind, new StaticEffectTargetData(ExplicitTargetTypes.Explicit, TargetEntities.Unit)},
            { SpellEffectType.Portal, new StaticEffectTargetData(ExplicitTargetTypes.None, TargetEntities.None)},
            { SpellEffectType.RitualBase, new StaticEffectTargetData(ExplicitTargetTypes.None, TargetEntities.None)},
            { SpellEffectType.IncreaseCurrencyCap, new StaticEffectTargetData(ExplicitTargetTypes.None, TargetEntities.None)},
            { SpellEffectType.RitualActivatePortal, new StaticEffectTargetData(ExplicitTargetTypes.None, TargetEntities.None)},
            { SpellEffectType.QuestComplete, new StaticEffectTargetData(ExplicitTargetTypes.Explicit, TargetEntities.Unit)},
            { SpellEffectType.WeaponDamageNoschool, new StaticEffectTargetData(ExplicitTargetTypes.Explicit, TargetEntities.Unit)},
            { SpellEffectType.AddExtraAttacks, new StaticEffectTargetData(ExplicitTargetTypes.Explicit, TargetEntities.Unit)},
            { SpellEffectType.Dodge, new StaticEffectTargetData(ExplicitTargetTypes.None, TargetEntities.Unit)},
            { SpellEffectType.Evade, new StaticEffectTargetData(ExplicitTargetTypes.None, TargetEntities.Unit)},
            { SpellEffectType.Parry, new StaticEffectTargetData(ExplicitTargetTypes.None, TargetEntities.Unit)},
            { SpellEffectType.Block, new StaticEffectTargetData(ExplicitTargetTypes.None, TargetEntities.Unit)},
            { SpellEffectType.CreateItem, new StaticEffectTargetData(ExplicitTargetTypes.Explicit, TargetEntities.Unit)},
            { SpellEffectType.Weapon, new StaticEffectTargetData(ExplicitTargetTypes.None, TargetEntities.Unit)},
            { SpellEffectType.Defense, new StaticEffectTargetData(ExplicitTargetTypes.None, TargetEntities.Unit)},
            { SpellEffectType.PersistentAreaAura, new StaticEffectTargetData(ExplicitTargetTypes.Explicit, TargetEntities.Dest)},
            { SpellEffectType.Summon, new StaticEffectTargetData(ExplicitTargetTypes.Explicit, TargetEntities.Dest)},
            { SpellEffectType.Leap, new StaticEffectTargetData(ExplicitTargetTypes.Explicit, TargetEntities.UnitAndDest)},
            { SpellEffectType.Energize, new StaticEffectTargetData(ExplicitTargetTypes.None, TargetEntities.Unit)},
            { SpellEffectType.WeaponPercentDamage, new StaticEffectTargetData(ExplicitTargetTypes.Explicit, TargetEntities.Unit)},
            { SpellEffectType.TriggerMissile, new StaticEffectTargetData(ExplicitTargetTypes.None, TargetEntities.None)},
            { SpellEffectType.SummonChangeItem, new StaticEffectTargetData(ExplicitTargetTypes.None, TargetEntities.Unit)},
            { SpellEffectType.ApplyAreaAuraParty, new StaticEffectTargetData(ExplicitTargetTypes.Explicit, TargetEntities.Unit)},
            { SpellEffectType.LearnSpell, new StaticEffectTargetData(ExplicitTargetTypes.Explicit, TargetEntities.Unit)},
            { SpellEffectType.SpellDefense, new StaticEffectTargetData(ExplicitTargetTypes.None, TargetEntities.Unit)},
            { SpellEffectType.Dispel, new StaticEffectTargetData(ExplicitTargetTypes.Explicit, TargetEntities.Unit)},
            { SpellEffectType.Language, new StaticEffectTargetData(ExplicitTargetTypes.None, TargetEntities.Unit)},
            { SpellEffectType.DualWield, new StaticEffectTargetData(ExplicitTargetTypes.Explicit, TargetEntities.Unit)},
            { SpellEffectType.Jump, new StaticEffectTargetData(ExplicitTargetTypes.Explicit, TargetEntities.Unit)},
            { SpellEffectType.JumpDest, new StaticEffectTargetData(ExplicitTargetTypes.None, TargetEntities.Dest)},
            { SpellEffectType.TeleportUnitsFaceCaster, new StaticEffectTargetData(ExplicitTargetTypes.Explicit, TargetEntities.UnitAndDest)},
            { SpellEffectType.SkillStep, new StaticEffectTargetData(ExplicitTargetTypes.Explicit, TargetEntities.Unit)},
            { SpellEffectType.PlayMovie, new StaticEffectTargetData(ExplicitTargetTypes.Explicit, TargetEntities.Unit)},
            { SpellEffectType.Spawn, new StaticEffectTargetData(ExplicitTargetTypes.None, TargetEntities.Unit)},
            { SpellEffectType.TradeSkill, new StaticEffectTargetData(ExplicitTargetTypes.None, TargetEntities.Unit)},
            { SpellEffectType.Stealth, new StaticEffectTargetData(ExplicitTargetTypes.None, TargetEntities.Unit)},
            { SpellEffectType.Detect, new StaticEffectTargetData(ExplicitTargetTypes.None, TargetEntities.Unit)},
            { SpellEffectType.TransDoor, new StaticEffectTargetData(ExplicitTargetTypes.Explicit, TargetEntities.Dest)},
            { SpellEffectType.ForceCriticalHit, new StaticEffectTargetData(ExplicitTargetTypes.None, TargetEntities.Unit)},
            { SpellEffectType.SetMaxBattlePetCount, new StaticEffectTargetData(ExplicitTargetTypes.None, TargetEntities.Unit)},
            { SpellEffectType.TameCreature, new StaticEffectTargetData(ExplicitTargetTypes.Explicit, TargetEntities.Unit)},
            { SpellEffectType.SummonPet, new StaticEffectTargetData(ExplicitTargetTypes.Explicit, TargetEntities.Dest)},
            { SpellEffectType.LearnPetSpell, new StaticEffectTargetData(ExplicitTargetTypes.Explicit, TargetEntities.Unit)},
            { SpellEffectType.WeaponDamage, new StaticEffectTargetData(ExplicitTargetTypes.Explicit, TargetEntities.Unit)},
            { SpellEffectType.CreateRandomItem, new StaticEffectTargetData(ExplicitTargetTypes.Explicit, TargetEntities.Unit)},
            { SpellEffectType.Proficiency, new StaticEffectTargetData(ExplicitTargetTypes.None, TargetEntities.Unit)},
            { SpellEffectType.SendEvent, new StaticEffectTargetData(ExplicitTargetTypes.None, TargetEntities.None)},
            { SpellEffectType.PowerBurn, new StaticEffectTargetData(ExplicitTargetTypes.Explicit, TargetEntities.Unit)},
            { SpellEffectType.Threat, new StaticEffectTargetData(ExplicitTargetTypes.Explicit, TargetEntities.Unit)},
            { SpellEffectType.TriggerSpell, new StaticEffectTargetData(ExplicitTargetTypes.None, TargetEntities.None)},
            { SpellEffectType.ApplyAreaAuraRaid, new StaticEffectTargetData(ExplicitTargetTypes.Explicit, TargetEntities.Unit)},
            { SpellEffectType.CreateManaGem, new StaticEffectTargetData(ExplicitTargetTypes.Explicit, TargetEntities.Unit)},
            { SpellEffectType.HealMaxHealth, new StaticEffectTargetData(ExplicitTargetTypes.Explicit, TargetEntities.Unit)},
            { SpellEffectType.InterruptCast, new StaticEffectTargetData(ExplicitTargetTypes.Explicit, TargetEntities.Unit)},
            { SpellEffectType.Distract, new StaticEffectTargetData(ExplicitTargetTypes.Explicit, TargetEntities.UnitAndDest)},
            { SpellEffectType.Pull, new StaticEffectTargetData(ExplicitTargetTypes.Explicit, TargetEntities.Unit)},
            { SpellEffectType.Pickpocket, new StaticEffectTargetData(ExplicitTargetTypes.Explicit, TargetEntities.Unit)},
            { SpellEffectType.AddFarsight, new StaticEffectTargetData(ExplicitTargetTypes.Explicit, TargetEntities.Dest)},
            { SpellEffectType.UntrainTalents, new StaticEffectTargetData(ExplicitTargetTypes.Explicit, TargetEntities.Unit)},
            { SpellEffectType.ApplyGlyph, new StaticEffectTargetData(ExplicitTargetTypes.None, TargetEntities.Unit)},
            { SpellEffectType.HealMechanical, new StaticEffectTargetData(ExplicitTargetTypes.Explicit, TargetEntities.Unit)},
            { SpellEffectType.SummonObjectWild, new StaticEffectTargetData(ExplicitTargetTypes.Explicit, TargetEntities.Dest)},
            { SpellEffectType.ScriptEffect, new StaticEffectTargetData(ExplicitTargetTypes.None, TargetEntities.None)},
            { SpellEffectType.Attack, new StaticEffectTargetData(ExplicitTargetTypes.None, TargetEntities.Unit)},
            { SpellEffectType.Sanctuary, new StaticEffectTargetData(ExplicitTargetTypes.None, TargetEntities.Unit)},
            { SpellEffectType.AddComboPoints, new StaticEffectTargetData(ExplicitTargetTypes.Explicit, TargetEntities.Unit)},
            { SpellEffectType.PushAbilityToActionBar, new StaticEffectTargetData(ExplicitTargetTypes.Explicit, TargetEntities.Dest)},
            { SpellEffectType.BindSight, new StaticEffectTargetData(ExplicitTargetTypes.Explicit, TargetEntities.Unit)},
            { SpellEffectType.Duel, new StaticEffectTargetData(ExplicitTargetTypes.Explicit, TargetEntities.UnitAndDest)},
            { SpellEffectType.Stuck, new StaticEffectTargetData(ExplicitTargetTypes.None, TargetEntities.Unit)},
            { SpellEffectType.SummonPlayer, new StaticEffectTargetData(ExplicitTargetTypes.None, TargetEntities.None)},
            { SpellEffectType.ActivateObject, new StaticEffectTargetData(ExplicitTargetTypes.Explicit, TargetEntities.GameEntity)},
            { SpellEffectType.GameobjectDamage, new StaticEffectTargetData(ExplicitTargetTypes.Explicit, TargetEntities.GameEntity)},
            { SpellEffectType.GameobjectRepair, new StaticEffectTargetData(ExplicitTargetTypes.Explicit, TargetEntities.GameEntity)},
            { SpellEffectType.GameobjectSetDestructionState, new StaticEffectTargetData(ExplicitTargetTypes.Explicit, TargetEntities.GameEntity)},
            { SpellEffectType.KillCredit, new StaticEffectTargetData(ExplicitTargetTypes.Explicit, TargetEntities.Unit)},
            { SpellEffectType.ThreatAll, new StaticEffectTargetData(ExplicitTargetTypes.Explicit, TargetEntities.Unit)},
            { SpellEffectType.EnchantHeldItem, new StaticEffectTargetData(ExplicitTargetTypes.Explicit, TargetEntities.Unit)},
            { SpellEffectType.ForceDeselect, new StaticEffectTargetData(ExplicitTargetTypes.None, TargetEntities.Unit)},
            { SpellEffectType.SelfResurrect, new StaticEffectTargetData(ExplicitTargetTypes.None, TargetEntities.Unit)},
            { SpellEffectType.Skinning, new StaticEffectTargetData(ExplicitTargetTypes.Explicit, TargetEntities.Unit)},
            { SpellEffectType.Charge, new StaticEffectTargetData(ExplicitTargetTypes.Explicit, TargetEntities.Unit)},
            { SpellEffectType.CastButton, new StaticEffectTargetData(ExplicitTargetTypes.None, TargetEntities.Unit)},
            { SpellEffectType.KnockBack, new StaticEffectTargetData(ExplicitTargetTypes.Explicit, TargetEntities.Unit)},
            { SpellEffectType.Inebriate, new StaticEffectTargetData(ExplicitTargetTypes.Explicit, TargetEntities.Unit)},
            { SpellEffectType.DismissPet, new StaticEffectTargetData(ExplicitTargetTypes.Explicit, TargetEntities.Unit)},
            { SpellEffectType.Reputation, new StaticEffectTargetData(ExplicitTargetTypes.Explicit, TargetEntities.Unit)},
            { SpellEffectType.SummonObjectSlot1, new StaticEffectTargetData(ExplicitTargetTypes.Explicit, TargetEntities.Dest)},
            { SpellEffectType.Survey, new StaticEffectTargetData(ExplicitTargetTypes.Explicit, TargetEntities.Dest)},
            { SpellEffectType.ChangeRaidMarker, new StaticEffectTargetData(ExplicitTargetTypes.Explicit, TargetEntities.Dest)},
            { SpellEffectType.ShowCorpseLoot, new StaticEffectTargetData(ExplicitTargetTypes.Explicit, TargetEntities.Dest)},
            { SpellEffectType.DispelMechanic, new StaticEffectTargetData(ExplicitTargetTypes.Explicit, TargetEntities.Unit)},
            { SpellEffectType.ResurrectPet, new StaticEffectTargetData(ExplicitTargetTypes.Explicit, TargetEntities.Dest)},
            { SpellEffectType.DestroyAllTotems, new StaticEffectTargetData(ExplicitTargetTypes.None, TargetEntities.Unit)},
            { SpellEffectType.DurabilityDamage, new StaticEffectTargetData(ExplicitTargetTypes.Explicit, TargetEntities.Unit)},
            { SpellEffectType.AttackMe, new StaticEffectTargetData(ExplicitTargetTypes.Explicit, TargetEntities.Unit)},
            { SpellEffectType.DurabilityDamagePct, new StaticEffectTargetData(ExplicitTargetTypes.Explicit, TargetEntities.Unit)},
            { SpellEffectType.SpiritHeal, new StaticEffectTargetData(ExplicitTargetTypes.Explicit, TargetEntities.Unit)},
            { SpellEffectType.Skill, new StaticEffectTargetData(ExplicitTargetTypes.None, TargetEntities.Unit)},
            { SpellEffectType.ApplyAreaAuraPet, new StaticEffectTargetData(ExplicitTargetTypes.Explicit, TargetEntities.Unit)},
            { SpellEffectType.TeleportGraveyard, new StaticEffectTargetData(ExplicitTargetTypes.Explicit, TargetEntities.Unit)},
            { SpellEffectType.NormalizedWeaponDmg, new StaticEffectTargetData(ExplicitTargetTypes.Explicit, TargetEntities.Unit)},
            { SpellEffectType.SendTaxi, new StaticEffectTargetData(ExplicitTargetTypes.Explicit, TargetEntities.Unit)},
            { SpellEffectType.PullTowards, new StaticEffectTargetData(ExplicitTargetTypes.Explicit, TargetEntities.Unit)},
            { SpellEffectType.ModifyThreatPercent, new StaticEffectTargetData(ExplicitTargetTypes.Explicit, TargetEntities.Unit)},
            { SpellEffectType.StealBeneficialBuff, new StaticEffectTargetData(ExplicitTargetTypes.Explicit, TargetEntities.Unit)},
            { SpellEffectType.ApplyAreaAuraFriend, new StaticEffectTargetData(ExplicitTargetTypes.Explicit, TargetEntities.Unit)},
            { SpellEffectType.ApplyAreaAuraEnemy, new StaticEffectTargetData(ExplicitTargetTypes.Explicit, TargetEntities.Unit)},
            { SpellEffectType.RedirectThreat, new StaticEffectTargetData(ExplicitTargetTypes.Explicit, TargetEntities.Unit)},
            { SpellEffectType.PlaySound, new StaticEffectTargetData(ExplicitTargetTypes.None, TargetEntities.Unit)},
            { SpellEffectType.PlayMusic, new StaticEffectTargetData(ExplicitTargetTypes.Explicit, TargetEntities.Unit)},
            { SpellEffectType.UnlearnSpecialization, new StaticEffectTargetData(ExplicitTargetTypes.Explicit, TargetEntities.Unit)},
            { SpellEffectType.KillCredit2, new StaticEffectTargetData(ExplicitTargetTypes.None, TargetEntities.Unit)},
            { SpellEffectType.CallPet, new StaticEffectTargetData(ExplicitTargetTypes.Explicit, TargetEntities.Dest)},
            { SpellEffectType.HealPct, new StaticEffectTargetData(ExplicitTargetTypes.Explicit, TargetEntities.Unit)},
            { SpellEffectType.EnergizePct, new StaticEffectTargetData(ExplicitTargetTypes.Explicit, TargetEntities.Unit)},
            { SpellEffectType.LeapBack, new StaticEffectTargetData(ExplicitTargetTypes.Explicit, TargetEntities.Unit)},
            { SpellEffectType.ClearQuest, new StaticEffectTargetData(ExplicitTargetTypes.Explicit, TargetEntities.Unit)},
            { SpellEffectType.ForceCast, new StaticEffectTargetData(ExplicitTargetTypes.Explicit, TargetEntities.Unit)},
            { SpellEffectType.ForceCastWithValue, new StaticEffectTargetData(ExplicitTargetTypes.Explicit, TargetEntities.Unit)},
            { SpellEffectType.TriggerSpellWithValue, new StaticEffectTargetData(ExplicitTargetTypes.Explicit, TargetEntities.Unit)},
            { SpellEffectType.ApplyAreaAuraOwner, new StaticEffectTargetData(ExplicitTargetTypes.Explicit, TargetEntities.Unit)},
            { SpellEffectType.KnockBackDest, new StaticEffectTargetData(ExplicitTargetTypes.Explicit, TargetEntities.UnitAndDest)},
            { SpellEffectType.PullTowardsDest, new StaticEffectTargetData(ExplicitTargetTypes.Explicit, TargetEntities.UnitAndDest)},
            { SpellEffectType.ActivateRune, new StaticEffectTargetData(ExplicitTargetTypes.Explicit, TargetEntities.Unit)},
            { SpellEffectType.QuestFail, new StaticEffectTargetData(ExplicitTargetTypes.Explicit, TargetEntities.Unit)},
            { SpellEffectType.TriggerMissileSpellWithValue, new StaticEffectTargetData(ExplicitTargetTypes.None, TargetEntities.None)},
            { SpellEffectType.ChargeDest, new StaticEffectTargetData(ExplicitTargetTypes.Explicit, TargetEntities.Dest)},
            { SpellEffectType.QuestStart, new StaticEffectTargetData(ExplicitTargetTypes.Explicit, TargetEntities.Unit)},
            { SpellEffectType.TriggerSpell2, new StaticEffectTargetData(ExplicitTargetTypes.None, TargetEntities.None)},
            { SpellEffectType.SummonRafFriend, new StaticEffectTargetData(ExplicitTargetTypes.None, TargetEntities.None)},
            { SpellEffectType.CreateTamedPet, new StaticEffectTargetData(ExplicitTargetTypes.Explicit, TargetEntities.Unit)},
            { SpellEffectType.DiscoverTaxi, new StaticEffectTargetData(ExplicitTargetTypes.Explicit, TargetEntities.Unit)},
            { SpellEffectType.TitanGrip, new StaticEffectTargetData(ExplicitTargetTypes.None, TargetEntities.Unit)},
            { SpellEffectType.CreateItem2, new StaticEffectTargetData(ExplicitTargetTypes.Explicit, TargetEntities.Unit)},
            { SpellEffectType.AllowRenamePet, new StaticEffectTargetData(ExplicitTargetTypes.Explicit, TargetEntities.Unit)},
            { SpellEffectType.ForceCast2, new StaticEffectTargetData(ExplicitTargetTypes.Explicit, TargetEntities.Unit)},
            { SpellEffectType.TalentSpecCount, new StaticEffectTargetData(ExplicitTargetTypes.Explicit, TargetEntities.Unit)},
            { SpellEffectType.TalentSpecSelect, new StaticEffectTargetData(ExplicitTargetTypes.Explicit, TargetEntities.Unit)},
            { SpellEffectType.RemoveAura, new StaticEffectTargetData(ExplicitTargetTypes.Explicit, TargetEntities.Unit)},
            { SpellEffectType.GiveCurrency, new StaticEffectTargetData(ExplicitTargetTypes.Caster, TargetEntities.Unit)},
            { SpellEffectType.DestroyItem, new StaticEffectTargetData(ExplicitTargetTypes.Caster, TargetEntities.Unit)},
            { SpellEffectType.RemoveTalent, new StaticEffectTargetData(ExplicitTargetTypes.Caster, TargetEntities.Unit)},
            { SpellEffectType.TeleportToDigsite, new StaticEffectTargetData(ExplicitTargetTypes.None, TargetEntities.None)},
            { SpellEffectType.PlayScene, new StaticEffectTargetData(ExplicitTargetTypes.None, TargetEntities.None)},
            { SpellEffectType.HealBattlepetPct, new StaticEffectTargetData(ExplicitTargetTypes.None, TargetEntities.None)},
            { SpellEffectType.EnableBattlePets, new StaticEffectTargetData(ExplicitTargetTypes.None, TargetEntities.None)},
            { SpellEffectType.ChangeBattlepetQuality, new StaticEffectTargetData(ExplicitTargetTypes.None, TargetEntities.None)},
            { SpellEffectType.LaunchQuestChoice, new StaticEffectTargetData(ExplicitTargetTypes.None, TargetEntities.None)},
        };

        #endregion
    }
}