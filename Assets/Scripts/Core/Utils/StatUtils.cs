using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;

namespace Core
{
    public static class StatUtils
    {
        public struct EntityFieldsComparer : IEqualityComparer<EntityFields>
        {
            public bool Equals(EntityFields x, EntityFields y)
            {
                return x == y;
            }

            public int GetHashCode(EntityFields obj)
            {
                return (int)obj;
            }
        }

        #region Entity field flags and types

        private static readonly Dictionary<EntityFields, FieldTypes> EntityFieldTypes = new Dictionary<EntityFields, FieldTypes>(new EntityFieldsComparer())
        {
            {EntityFields.Entry, FieldTypes.Int},
            {EntityFields.Scale, FieldTypes.Float},
            {EntityFields.DynamicFlags, FieldTypes.Long},

            {EntityFields.GameEntityCreatedBy, FieldTypes.Ulong},
            {EntityFields.GameEntityDisplayId, FieldTypes.Uint},
            {EntityFields.GameEntityFlags, FieldTypes.Uint},
            {EntityFields.ParentRotation0, FieldTypes.Float},
            {EntityFields.ParentRotation1, FieldTypes.Float},
            {EntityFields.ParentRotation2, FieldTypes.Float},
            {EntityFields.ParentRotation3, FieldTypes.Float},
            {EntityFields.GameEntityFaction, FieldTypes.Uint},
            {EntityFields.GameEntityLevel, FieldTypes.Uint},
            {EntityFields.GameEntityInfo, FieldTypes.Uint},
            {EntityFields.SpellVisualID, FieldTypes.Uint},
            {EntityFields.StateSpellVisualID, FieldTypes.Uint},
            {EntityFields.StateAnimID, FieldTypes.Uint},
            {EntityFields.StateAnimKitID, FieldTypes.Uint},
            {EntityFields.StateWorldEffectID, FieldTypes.Uint},

            {EntityFields.DynamicCaster, FieldTypes.Ulong},
            {EntityFields.DynamicEntityType, FieldTypes.Uint},
            {EntityFields.DynamicSpellVisualID, FieldTypes.Uint},
            {EntityFields.DynamicSpellid, FieldTypes.Uint},
            {EntityFields.DynamicRadius, FieldTypes.Float},
            {EntityFields.DynamicCasttime, FieldTypes.Uint},

            {EntityFields.UnitCharm, FieldTypes.Ulong},
            {EntityFields.UnitSummon, FieldTypes.Ulong},
            {EntityFields.UnitCritter, FieldTypes.Ulong},
            {EntityFields.UnitCharmedBy, FieldTypes.Ulong},
            {EntityFields.UnitSummonedBy, FieldTypes.Ulong},
            {EntityFields.UnitCreatedBy, FieldTypes.Ulong},
            {EntityFields.DemonCreator, FieldTypes.Ulong},
            {EntityFields.Target, FieldTypes.Ulong},
            {EntityFields.BattlePetCompanionGuid, FieldTypes.Ulong},
            {EntityFields.ChannelObject, FieldTypes.Ulong},
            {EntityFields.ChannelSpell, FieldTypes.Uint},
            {EntityFields.ChannelSpellVisual, FieldTypes.Uint},
            {EntityFields.Info, FieldTypes.Int},
            {EntityFields.DisplayPower, FieldTypes.Int},
            {EntityFields.OverrideDisplayPowerId, FieldTypes.Int},
            {EntityFields.Health, FieldTypes.Long},
            {EntityFields.Power, FieldTypes.Int},
            {EntityFields.Power1, FieldTypes.Int},
            {EntityFields.Power2, FieldTypes.Int},
            {EntityFields.Power3, FieldTypes.Int},
            {EntityFields.Power4, FieldTypes.Int},
            {EntityFields.Power5, FieldTypes.Int},
            {EntityFields.MaxHealth, FieldTypes.Long},
            {EntityFields.MaxPower, FieldTypes.Int},
            {EntityFields.MaxPower1, FieldTypes.Int},
            {EntityFields.MaxPower2, FieldTypes.Int},
            {EntityFields.MaxPower3, FieldTypes.Int},
            {EntityFields.MaxPower4, FieldTypes.Int},
            {EntityFields.MaxPower5, FieldTypes.Int},
            {EntityFields.PowerRegenFlatModifier, FieldTypes.Float},
            {EntityFields.PowerRegenFlatModifier1, FieldTypes.Float},
            {EntityFields.PowerRegenFlatModifier2, FieldTypes.Float},
            {EntityFields.PowerRegenFlatModifier3, FieldTypes.Float},
            {EntityFields.PowerRegenFlatModifier4, FieldTypes.Float},
            {EntityFields.PowerRegenFlatModifier5, FieldTypes.Float},
            {EntityFields.PowerRegenInterruptedFlatModifier, FieldTypes.Float},
            {EntityFields.PowerRegenInterruptedFlatModifier1, FieldTypes.Float},
            {EntityFields.PowerRegenInterruptedFlatModifier2, FieldTypes.Float},
            {EntityFields.PowerRegenInterruptedFlatModifier3, FieldTypes.Float},
            {EntityFields.PowerRegenInterruptedFlatModifier4, FieldTypes.Float},
            {EntityFields.PowerRegenInterruptedFlatModifier5, FieldTypes.Float},
            {EntityFields.Level, FieldTypes.Uint},
            {EntityFields.FactionTemplate, FieldTypes.Uint},
            {EntityFields.UnitVirtualItemSlotId, FieldTypes.Int},
            {EntityFields.UnitVirtualItemSlotId1, FieldTypes.Int},
            {EntityFields.UnitVirtualItemSlotId2, FieldTypes.Int},
            {EntityFields.UnitVirtualItemSlotId3, FieldTypes.Int},
            {EntityFields.UnitVirtualItemSlotId4, FieldTypes.Int},
            {EntityFields.UnitVirtualItemSlotId5, FieldTypes.Int},
            {EntityFields.UnitFlags, FieldTypes.Int},
            {EntityFields.AuraState, FieldTypes.Int},
            {EntityFields.BaseAttackTimeMain, FieldTypes.Uint},
            {EntityFields.BaseAttackTimeOffhand, FieldTypes.Uint},
            {EntityFields.BaseAttackTimeRanged, FieldTypes.Uint},
            {EntityFields.BoundingRadius, FieldTypes.Float},
            {EntityFields.CombatReach, FieldTypes.Float},
            {EntityFields.DisplayId, FieldTypes.Uint},
            {EntityFields.NativeDisplayId, FieldTypes.Uint},
            {EntityFields.MountDisplayId, FieldTypes.Uint},
            {EntityFields.MinDamage, FieldTypes.Float},
            {EntityFields.MaxDamage, FieldTypes.Float},
            {EntityFields.MinOffhandDamage, FieldTypes.Float},
            {EntityFields.MaxOffhandDamage, FieldTypes.Float},
            {EntityFields.PetNumber, FieldTypes.Uint},
            {EntityFields.PetNameTimestamp, FieldTypes.Uint},
            {EntityFields.PetExperience, FieldTypes.Uint},
            {EntityFields.PetNextLevelExp, FieldTypes.Uint},
            {EntityFields.UnitModCastSpeed, FieldTypes.Float},
            {EntityFields.UnitModCastHaste, FieldTypes.Float},
            {EntityFields.ModHaste, FieldTypes.Float},
            {EntityFields.ModRangedHaste, FieldTypes.Float},
            {EntityFields.ModHasteRegen, FieldTypes.Float},
            {EntityFields.ModTimeRate, FieldTypes.Float},
            {EntityFields.UnitCreatedBySpell, FieldTypes.Uint},
            {EntityFields.UnitNpcFlags, FieldTypes.Long},
            {EntityFields.Stat, FieldTypes.Uint},
            {EntityFields.Stat1, FieldTypes.Uint},
            {EntityFields.Stat2, FieldTypes.Uint},
            {EntityFields.Stat3, FieldTypes.Uint},
            {EntityFields.PosStat, FieldTypes.Float},
            {EntityFields.PosStat1, FieldTypes.Float},
            {EntityFields.PosStat2, FieldTypes.Float},
            {EntityFields.PosStat3, FieldTypes.Float},
            {EntityFields.NegStat, FieldTypes.Float},
            {EntityFields.NegStat1, FieldTypes.Float},
            {EntityFields.NegStat2, FieldTypes.Float},
            {EntityFields.NegStat3, FieldTypes.Float},
            {EntityFields.Resistances, FieldTypes.Uint},
            {EntityFields.Resistances1, FieldTypes.Uint},
            {EntityFields.Resistances2, FieldTypes.Uint},
            {EntityFields.Resistances3, FieldTypes.Uint},
            {EntityFields.Resistances4, FieldTypes.Uint},
            {EntityFields.Resistances5, FieldTypes.Uint},
            {EntityFields.Resistances6, FieldTypes.Uint},
            {EntityFields.ResistanceBuffModsPositive, FieldTypes.Uint},
            {EntityFields.ResistanceBuffModsPositive1, FieldTypes.Uint},
            {EntityFields.ResistanceBuffModsPositive2, FieldTypes.Uint},
            {EntityFields.ResistanceBuffModsPositive3, FieldTypes.Uint},
            {EntityFields.ResistanceBuffModsPositive4, FieldTypes.Uint},
            {EntityFields.ResistanceBuffModsPositive5, FieldTypes.Uint},
            {EntityFields.ResistanceBuffModsPositive6, FieldTypes.Uint},
            {EntityFields.ResistanceBuffModsNegative, FieldTypes.Uint},
            {EntityFields.ResistanceBuffModsNegative1, FieldTypes.Uint},
            {EntityFields.ResistanceBuffModsNegative2, FieldTypes.Uint},
            {EntityFields.ResistanceBuffModsNegative3, FieldTypes.Uint},
            {EntityFields.ResistanceBuffModsNegative4, FieldTypes.Uint},
            {EntityFields.ResistanceBuffModsNegative5, FieldTypes.Uint},
            {EntityFields.ResistanceBuffModsNegative6, FieldTypes.Uint},
            {EntityFields.BaseMana, FieldTypes.Uint},
            {EntityFields.BaseHealth, FieldTypes.Uint},
            {EntityFields.BaseFlags, FieldTypes.Uint},
            {EntityFields.AttackPower, FieldTypes.Uint},
            {EntityFields.AttackPowerModPos, FieldTypes.Uint},
            {EntityFields.AttackPowerModNeg, FieldTypes.Uint},
            {EntityFields.AttackPowerMultiplier, FieldTypes.Float},
            {EntityFields.RangedAttackPower, FieldTypes.Uint},
            {EntityFields.RangedAttackPowerModPos, FieldTypes.Uint},
            {EntityFields.RangedAttackPowerModNeg, FieldTypes.Uint},
            {EntityFields.RangedAttackPowerMultiplier, FieldTypes.Float},
            {EntityFields.MinRangedDamage, FieldTypes.Float},
            {EntityFields.MaxRangedDamage, FieldTypes.Float},
            {EntityFields.PowerCostModifier, FieldTypes.Uint},
            {EntityFields.PowerCostModifier1, FieldTypes.Uint},
            {EntityFields.PowerCostModifier2, FieldTypes.Uint},
            {EntityFields.PowerCostModifier3, FieldTypes.Uint},
            {EntityFields.PowerCostModifier4, FieldTypes.Uint},
            {EntityFields.PowerCostModifier5, FieldTypes.Uint},
            {EntityFields.PowerCostModifier6, FieldTypes.Uint},
            {EntityFields.PowerCostMultiplier, FieldTypes.Float},
            {EntityFields.PowerCostMultiplier1, FieldTypes.Float},
            {EntityFields.PowerCostMultiplier2, FieldTypes.Float},
            {EntityFields.PowerCostMultiplier3, FieldTypes.Float},
            {EntityFields.PowerCostMultiplier4, FieldTypes.Float},
            {EntityFields.PowerCostMultiplier5, FieldTypes.Float},
            {EntityFields.PowerCostMultiplier6, FieldTypes.Float},

            {EntityFields.PlayerFlags, FieldTypes.Long},
            {EntityFields.ArenaTeam, FieldTypes.Int},
            {EntityFields.DuelTeam, FieldTypes.Uint},
            {EntityFields.ChosenTitle, FieldTypes.Uint},
            {EntityFields.CurrentSpecId, FieldTypes.Uint},
            {EntityFields.Coinage, FieldTypes.Long},
            {EntityFields.Xp, FieldTypes.Uint},
            {EntityFields.PlayerCharacterPoints, FieldTypes.Uint},
            {EntityFields.NextLevelXp, FieldTypes.Uint},
            {EntityFields.BlockPercentage, FieldTypes.Float},
            {EntityFields.DodgePercentage, FieldTypes.Float},
            {EntityFields.DodgePercentageFromAttribute, FieldTypes.Float},
            {EntityFields.ParryPercentage, FieldTypes.Float},
            {EntityFields.ParryPercentageFromAttribute, FieldTypes.Float},
            {EntityFields.CritPercentage, FieldTypes.Float},
            {EntityFields.RangedCritPercentage, FieldTypes.Float},
            {EntityFields.OffhandCritPercentage, FieldTypes.Float},
            {EntityFields.SpellCritPercentage, FieldTypes.Float},
            {EntityFields.ShieldBlock, FieldTypes.Uint},
            {EntityFields.ShieldBlockCritPercentage, FieldTypes.Float},
            {EntityFields.Mastery, FieldTypes.Float},
            {EntityFields.ModDamageDonePos, FieldTypes.Uint},
            {EntityFields.ModDamageDonePos1, FieldTypes.Uint},
            {EntityFields.ModDamageDonePos2, FieldTypes.Uint},
            {EntityFields.ModDamageDonePos3, FieldTypes.Uint},
            {EntityFields.ModDamageDonePos4, FieldTypes.Uint},
            {EntityFields.ModDamageDonePos5, FieldTypes.Uint},
            {EntityFields.ModDamageDonePos6, FieldTypes.Uint},
            {EntityFields.ModDamageDoneNeg, FieldTypes.Uint},
            {EntityFields.ModDamageDoneNeg1, FieldTypes.Uint},
            {EntityFields.ModDamageDoneNeg2, FieldTypes.Uint},
            {EntityFields.ModDamageDoneNeg3, FieldTypes.Uint},
            {EntityFields.ModDamageDoneNeg4, FieldTypes.Uint},
            {EntityFields.ModDamageDoneNeg5, FieldTypes.Uint},
            {EntityFields.ModDamageDoneNeg6, FieldTypes.Uint},
            {EntityFields.ModDamageDonePct, FieldTypes.Float},
            {EntityFields.ModDamageDonePct1, FieldTypes.Float},
            {EntityFields.ModDamageDonePct2, FieldTypes.Float},
            {EntityFields.ModDamageDonePct3, FieldTypes.Float},
            {EntityFields.ModDamageDonePct4, FieldTypes.Float},
            {EntityFields.ModDamageDonePct5, FieldTypes.Float},
            {EntityFields.ModDamageDonePct6, FieldTypes.Float},
            {EntityFields.OverrideSpellPowerByApPct, FieldTypes.Float},
            {EntityFields.OverrideApBySpellPowerPercent, FieldTypes.Float},
            {EntityFields.ModTargetResistance, FieldTypes.Int},
            {EntityFields.ModTargetPhysicalResistance, FieldTypes.Int},
            {EntityFields.CombatRating1, FieldTypes.Uint},
            {EntityFields.CombatRating2, FieldTypes.Uint},
            {EntityFields.CombatRating3, FieldTypes.Uint},
            {EntityFields.CombatRating4, FieldTypes.Uint},
            {EntityFields.CombatRating5, FieldTypes.Uint},
            {EntityFields.CombatRating6, FieldTypes.Uint},
            {EntityFields.CombatRating7, FieldTypes.Uint},
            {EntityFields.CombatRating8, FieldTypes.Uint},
            {EntityFields.CombatRating9, FieldTypes.Uint},
            {EntityFields.CombatRating10, FieldTypes.Uint},

            {EntityFields.ItemOwner, FieldTypes.Ulong},
            {EntityFields.ItemContained, FieldTypes.Ulong},
            {EntityFields.ItemCreator, FieldTypes.Ulong},
            {EntityFields.ItemGiftcreator, FieldTypes.Ulong},
            {EntityFields.ItemStackCount, FieldTypes.Uint},
            {EntityFields.ItemDuration, FieldTypes.Uint},
            {EntityFields.ItemSpellCharges, FieldTypes.Int},
            {EntityFields.ItemSpellCharges1, FieldTypes.Int},
            {EntityFields.ItemSpellCharges2, FieldTypes.Int},
            {EntityFields.ItemSpellCharges3, FieldTypes.Int},
            {EntityFields.ItemSpellCharges4, FieldTypes.Int},
            {EntityFields.ItemFlags, FieldTypes.Uint},
            {EntityFields.ItemEnchantmentID, FieldTypes.Uint},
            {EntityFields.ItemEnchantmentDuration, FieldTypes.Uint},
            {EntityFields.ItemEnchantmentCharges, FieldTypes.Uint},
            {EntityFields.ItemEnchantmentID1, FieldTypes.Uint},
            {EntityFields.ItemEnchantmentDuration1, FieldTypes.Uint},
            {EntityFields.ItemEnchantmentCharges1, FieldTypes.Uint},
            {EntityFields.ItemEnchantmentID2, FieldTypes.Uint},
            {EntityFields.ItemEnchantmentDuration2, FieldTypes.Uint},
            {EntityFields.ItemEnchantmentCharges2, FieldTypes.Uint},
            {EntityFields.ItemEnchantmentID3, FieldTypes.Uint},
            {EntityFields.ItemEnchantmentDuration3, FieldTypes.Uint},
            {EntityFields.ItemEnchantmentCharges3, FieldTypes.Uint},
            {EntityFields.ItemEnchantmentID4, FieldTypes.Uint},
            {EntityFields.ItemEnchantmentDuration4, FieldTypes.Uint},
            {EntityFields.ItemEnchantmentCharges4, FieldTypes.Uint},
            {EntityFields.ItemEnchantmentID5, FieldTypes.Uint},
            {EntityFields.ItemEnchantmentDuration5, FieldTypes.Uint},
            {EntityFields.ItemEnchantmentCharges5, FieldTypes.Uint},
            {EntityFields.ItemEnchantmentID6, FieldTypes.Uint},
            {EntityFields.ItemEnchantmentDuration6, FieldTypes.Uint},
            {EntityFields.ItemEnchantmentCharges6, FieldTypes.Uint},
            {EntityFields.ItemEnchantmentID7, FieldTypes.Uint},
            {EntityFields.ItemEnchantmentDuration7, FieldTypes.Uint},
            {EntityFields.ItemEnchantmentCharges7, FieldTypes.Uint},
            {EntityFields.ItemEnchantmentID8, FieldTypes.Uint},
            {EntityFields.ItemEnchantmentDuration8, FieldTypes.Uint},
            {EntityFields.ItemEnchantmentCharges8, FieldTypes.Uint},
            {EntityFields.ItemEnchantmentID9, FieldTypes.Uint},
            {EntityFields.ItemEnchantmentDuration9, FieldTypes.Uint},
            {EntityFields.ItemEnchantmentCharges9, FieldTypes.Uint},
            {EntityFields.ItemEnchantmentID10, FieldTypes.Uint},
            {EntityFields.ItemEnchantmentDuration10, FieldTypes.Uint},
            {EntityFields.ItemEnchantmentCharges10, FieldTypes.Uint},
            {EntityFields.ItemEnchantmentID11, FieldTypes.Uint},
            {EntityFields.ItemEnchantmentDuration11, FieldTypes.Uint},
            {EntityFields.ItemEnchantmentCharges11, FieldTypes.Uint},
            {EntityFields.ItemEnchantmentID12, FieldTypes.Uint},
            {EntityFields.ItemEnchantmentDuration12, FieldTypes.Uint},
            {EntityFields.ItemEnchantmentCharges12, FieldTypes.Uint},
            {EntityFields.ItemPropertySeed, FieldTypes.Uint},
            {EntityFields.ItemRandomPropertiesID, FieldTypes.Int},
            {EntityFields.ItemDurability, FieldTypes.Uint},
            {EntityFields.ItemMaxdurability, FieldTypes.Uint},
            {EntityFields.ItemCreatePlayedTime, FieldTypes.Uint},
            {EntityFields.ItemModifiersMask, FieldTypes.Uint},
            {EntityFields.ItemContext, FieldTypes.Uint},
            {EntityFields.ItemArtifactXP, FieldTypes.Uint},
            {EntityFields.ItemAppearanceModID, FieldTypes.Uint},

            {EntityFields.ContainerFieldSlot1, FieldTypes.Uint},
            {EntityFields.ContainerFieldSlot2, FieldTypes.Uint},
            {EntityFields.ContainerFieldSlot3, FieldTypes.Uint},
            {EntityFields.ContainerFieldSlot4, FieldTypes.Uint},
            {EntityFields.ContainerFieldSlot5, FieldTypes.Uint},
            {EntityFields.ContainerFieldSlot6, FieldTypes.Uint},
            {EntityFields.ContainerFieldSlot7, FieldTypes.Uint},
            {EntityFields.ContainerFieldSlot8, FieldTypes.Uint},
            {EntityFields.ContainerFieldSlot9, FieldTypes.Uint},
            {EntityFields.ContainerFieldSlot10, FieldTypes.Uint},
            {EntityFields.ContainerFieldSlot11, FieldTypes.Uint},
            {EntityFields.ContainerFieldSlot12, FieldTypes.Uint},
            {EntityFields.ContainerFieldSlot13, FieldTypes.Uint},
            {EntityFields.ContainerFieldSlot14, FieldTypes.Uint},
            {EntityFields.ContainerFieldSlot15, FieldTypes.Uint},
            {EntityFields.ContainerFieldSlot16, FieldTypes.Uint},
            {EntityFields.ContainerFieldSlot17, FieldTypes.Uint},
            {EntityFields.ContainerFieldSlot18, FieldTypes.Uint},
            {EntityFields.ContainerFieldSlot19, FieldTypes.Uint},
            {EntityFields.ContainerFieldSlot20, FieldTypes.Uint},
            {EntityFields.ContainerFieldSlot21, FieldTypes.Uint},
            {EntityFields.ContainerFieldSlot22, FieldTypes.Uint},
            {EntityFields.ContainerFieldSlot23, FieldTypes.Uint},
            {EntityFields.ContainerFieldSlot24, FieldTypes.Uint},
            {EntityFields.ContainerFieldSlot25, FieldTypes.Uint},
            {EntityFields.ContainerFieldSlot26, FieldTypes.Uint},
            {EntityFields.ContainerFieldSlot27, FieldTypes.Uint},
            {EntityFields.ContainerFieldSlot28, FieldTypes.Uint},
            {EntityFields.ContainerFieldSlot29, FieldTypes.Uint},
            {EntityFields.ContainerFieldSlot30, FieldTypes.Uint},
            {EntityFields.ContainerFieldSlot31, FieldTypes.Uint},
            {EntityFields.ContainerFieldSlot32, FieldTypes.Uint},
            {EntityFields.ContainerFieldSlot33, FieldTypes.Uint},
            {EntityFields.ContainerFieldSlot34, FieldTypes.Uint},
            {EntityFields.ContainerFieldSlot35, FieldTypes.Uint},
            {EntityFields.ContainerFieldSlot36, FieldTypes.Uint},
            {EntityFields.ContainerFieldNumSlots, FieldTypes.Uint},

            {EntityFields.CorpseOwner, FieldTypes.Ulong},
            {EntityFields.CorpseParty, FieldTypes.Ulong},
            {EntityFields.CorpseDisplayID, FieldTypes.Uint},
            {EntityFields.CorpseItem, FieldTypes.Uint},
            {EntityFields.CorpseItem1, FieldTypes.Uint},
            {EntityFields.CorpseItem2, FieldTypes.Uint},
            {EntityFields.CorpseItem3, FieldTypes.Uint},
            {EntityFields.CorpseItem4, FieldTypes.Uint},
            {EntityFields.CorpseItem5, FieldTypes.Uint},
            {EntityFields.CorpseItem6, FieldTypes.Uint},
            {EntityFields.CorpseItem7, FieldTypes.Uint},
            {EntityFields.CorpseItem8, FieldTypes.Uint},
            {EntityFields.CorpseItem9, FieldTypes.Uint},
            {EntityFields.CorpseItem10, FieldTypes.Uint},
            {EntityFields.CorpseItem11, FieldTypes.Uint},
            {EntityFields.CorpseItem12, FieldTypes.Uint},
            {EntityFields.CorpseItem13, FieldTypes.Uint},
            {EntityFields.CorpseItem14, FieldTypes.Uint},
            {EntityFields.CorpseItem15, FieldTypes.Uint},
            {EntityFields.CorpseItem16, FieldTypes.Uint},
            {EntityFields.CorpseItem17, FieldTypes.Uint},
            {EntityFields.CorpseItem18, FieldTypes.Uint},
            {EntityFields.CorpseInfo, FieldTypes.Uint},
            {EntityFields.CorpseExtraInfo, FieldTypes.Uint},
            {EntityFields.CorpseFlags, FieldTypes.Uint},
            {EntityFields.CorpseDynamicFlags, FieldTypes.Uint},
            {EntityFields.CorpseFactiontemplate, FieldTypes.Uint},
            {EntityFields.CorpseCustomDisplayOption, FieldTypes.Uint},
        };

        private static readonly Dictionary<EntityFields, FieldFlags> EntityFieldFlags = new Dictionary<EntityFields, FieldFlags>(new EntityFieldsComparer())
        {
            {EntityFields.Entry, FieldFlags.Dynamic},
            {EntityFields.Scale, FieldFlags.Public},
            {EntityFields.DynamicFlags, FieldFlags.Dynamic | FieldFlags.Urgent},

            {EntityFields.GameEntityCreatedBy, FieldFlags.Public},
            {EntityFields.GameEntityDisplayId, FieldFlags.Dynamic | FieldFlags.Urgent},
            {EntityFields.GameEntityFlags, FieldFlags.Dynamic | FieldFlags.Urgent},
            {EntityFields.ParentRotation0, FieldFlags.Public},
            {EntityFields.ParentRotation1, FieldFlags.Public},
            {EntityFields.ParentRotation2, FieldFlags.Public},
            {EntityFields.ParentRotation3, FieldFlags.Public},
            {EntityFields.GameEntityFaction, FieldFlags.Public},
            {EntityFields.GameEntityLevel, FieldFlags.Public},
            {EntityFields.GameEntityInfo, FieldFlags.Public | FieldFlags.Urgent},
            {EntityFields.SpellVisualID, FieldFlags.Public | FieldFlags.Dynamic | FieldFlags.Urgent},
            {EntityFields.StateSpellVisualID, FieldFlags.Public | FieldFlags.Dynamic | FieldFlags.Urgent},
            {EntityFields.StateAnimID, FieldFlags.Dynamic | FieldFlags.Urgent},
            {EntityFields.StateAnimKitID, FieldFlags.Dynamic | FieldFlags.Urgent},
            {EntityFields.StateWorldEffectID, FieldFlags.Dynamic | FieldFlags.Urgent},

            {EntityFields.DynamicCaster, FieldFlags.Public},
            {EntityFields.DynamicEntityType, FieldFlags.Public},
            {EntityFields.DynamicSpellVisualID, FieldFlags.Public},
            {EntityFields.DynamicSpellid, FieldFlags.Public},
            {EntityFields.DynamicRadius, FieldFlags.Public},
            {EntityFields.DynamicCasttime, FieldFlags.Public},

            {EntityFields.UnitCharm, FieldFlags.Public},
            {EntityFields.UnitSummon, FieldFlags.Public},
            {EntityFields.UnitCritter, FieldFlags.Private},
            {EntityFields.UnitCharmedBy, FieldFlags.Public},
            {EntityFields.UnitSummonedBy, FieldFlags.Public},
            {EntityFields.UnitCreatedBy, FieldFlags.Public},
            {EntityFields.DemonCreator, FieldFlags.Public},
            {EntityFields.Target, FieldFlags.Public},
            {EntityFields.BattlePetCompanionGuid, FieldFlags.Public},
            {EntityFields.ChannelObject, FieldFlags.Public | FieldFlags.Urgent},
            {EntityFields.ChannelSpell, FieldFlags.Public | FieldFlags.Urgent},
            {EntityFields.ChannelSpellVisual, FieldFlags.Public | FieldFlags.Urgent},
            {EntityFields.Info, FieldFlags.Public},
            {EntityFields.DisplayPower, FieldFlags.Public},
            {EntityFields.OverrideDisplayPowerId, FieldFlags.Public},
            {EntityFields.Health, FieldFlags.Public},
            {EntityFields.Power, FieldFlags.Public | FieldFlags.UrgentSelfOnly},
            {EntityFields.Power1, FieldFlags.Public | FieldFlags.UrgentSelfOnly},
            {EntityFields.Power2, FieldFlags.Public | FieldFlags.UrgentSelfOnly},
            {EntityFields.Power3, FieldFlags.Public | FieldFlags.UrgentSelfOnly},
            {EntityFields.Power4, FieldFlags.Public | FieldFlags.UrgentSelfOnly},
            {EntityFields.Power5, FieldFlags.Public | FieldFlags.UrgentSelfOnly},
            {EntityFields.MaxHealth, FieldFlags.Public},
            {EntityFields.MaxPower, FieldFlags.Public},
            {EntityFields.MaxPower1, FieldFlags.Public},
            {EntityFields.MaxPower2, FieldFlags.Public},
            {EntityFields.MaxPower3, FieldFlags.Public},
            {EntityFields.MaxPower4, FieldFlags.Public},
            {EntityFields.MaxPower5, FieldFlags.Public},
            {EntityFields.PowerRegenFlatModifier, FieldFlags.Private | FieldFlags.Owner | FieldFlags.UnitAll},
            {EntityFields.PowerRegenFlatModifier1, FieldFlags.Private | FieldFlags.Owner | FieldFlags.UnitAll},
            {EntityFields.PowerRegenFlatModifier2, FieldFlags.Private | FieldFlags.Owner | FieldFlags.UnitAll},
            {EntityFields.PowerRegenFlatModifier3, FieldFlags.Private | FieldFlags.Owner | FieldFlags.UnitAll},
            {EntityFields.PowerRegenFlatModifier4, FieldFlags.Private | FieldFlags.Owner | FieldFlags.UnitAll},
            {EntityFields.PowerRegenFlatModifier5, FieldFlags.Private | FieldFlags.Owner | FieldFlags.UnitAll},
            {EntityFields.PowerRegenInterruptedFlatModifier, FieldFlags.Private | FieldFlags.Owner | FieldFlags.UnitAll},
            {EntityFields.PowerRegenInterruptedFlatModifier1, FieldFlags.Private | FieldFlags.Owner | FieldFlags.UnitAll},
            {EntityFields.PowerRegenInterruptedFlatModifier2, FieldFlags.Private | FieldFlags.Owner | FieldFlags.UnitAll},
            {EntityFields.PowerRegenInterruptedFlatModifier3, FieldFlags.Private | FieldFlags.Owner | FieldFlags.UnitAll},
            {EntityFields.PowerRegenInterruptedFlatModifier4, FieldFlags.Private | FieldFlags.Owner | FieldFlags.UnitAll},
            {EntityFields.PowerRegenInterruptedFlatModifier5, FieldFlags.Private | FieldFlags.Owner | FieldFlags.UnitAll},
            {EntityFields.Level, FieldFlags.Public},
            {EntityFields.FactionTemplate, FieldFlags.Public},
            {EntityFields.UnitVirtualItemSlotId, FieldFlags.Public},
            {EntityFields.UnitVirtualItemSlotId1, FieldFlags.Public},
            {EntityFields.UnitVirtualItemSlotId2, FieldFlags.Public},
            {EntityFields.UnitVirtualItemSlotId3, FieldFlags.Public},
            {EntityFields.UnitVirtualItemSlotId4, FieldFlags.Public},
            {EntityFields.UnitVirtualItemSlotId5, FieldFlags.Public},
            {EntityFields.UnitFlags, FieldFlags.Public | FieldFlags.Urgent},
            {EntityFields.AuraState, FieldFlags.Public},
            {EntityFields.BaseAttackTimeMain, FieldFlags.Public},
            {EntityFields.BaseAttackTimeOffhand, FieldFlags.Public},
            {EntityFields.BaseAttackTimeRanged, FieldFlags.Public},
            {EntityFields.BoundingRadius, FieldFlags.Public},
            {EntityFields.CombatReach, FieldFlags.Public},
            {EntityFields.DisplayId, FieldFlags.Dynamic | FieldFlags.Urgent},
            {EntityFields.NativeDisplayId, FieldFlags.Dynamic | FieldFlags.Urgent},
            {EntityFields.MountDisplayId, FieldFlags.Dynamic | FieldFlags.Urgent},
            {EntityFields.MinDamage, FieldFlags.Private | FieldFlags.Owner | FieldFlags.SpecialInfo},
            {EntityFields.MaxDamage, FieldFlags.Private | FieldFlags.Owner | FieldFlags.SpecialInfo},
            {EntityFields.MinOffhandDamage, FieldFlags.Private | FieldFlags.Owner | FieldFlags.SpecialInfo},
            {EntityFields.MaxOffhandDamage, FieldFlags.Private | FieldFlags.Owner | FieldFlags.SpecialInfo},
            {EntityFields.PetNumber, FieldFlags.Public},
            {EntityFields.PetNameTimestamp, FieldFlags.Public},
            {EntityFields.PetExperience, FieldFlags.Owner},
            {EntityFields.PetNextLevelExp, FieldFlags.Owner},
            {EntityFields.UnitModCastSpeed, FieldFlags.Public},
            {EntityFields.UnitModCastHaste, FieldFlags.Public},
            {EntityFields.ModHaste, FieldFlags.Public},
            {EntityFields.ModRangedHaste, FieldFlags.Public},
            {EntityFields.ModHasteRegen, FieldFlags.Public},
            {EntityFields.ModTimeRate, FieldFlags.Public},
            {EntityFields.UnitCreatedBySpell, FieldFlags.Public},
            {EntityFields.UnitNpcFlags, FieldFlags.Public | FieldFlags.Dynamic},
            {EntityFields.Stat, FieldFlags.Private | FieldFlags.Owner},
            {EntityFields.Stat1, FieldFlags.Private | FieldFlags.Owner},
            {EntityFields.Stat2, FieldFlags.Private | FieldFlags.Owner},
            {EntityFields.Stat3, FieldFlags.Private | FieldFlags.Owner},
            {EntityFields.PosStat, FieldFlags.Private | FieldFlags.Owner},
            {EntityFields.PosStat1, FieldFlags.Private | FieldFlags.Owner},
            {EntityFields.PosStat2, FieldFlags.Private | FieldFlags.Owner},
            {EntityFields.PosStat3, FieldFlags.Private | FieldFlags.Owner},
            {EntityFields.NegStat, FieldFlags.Private | FieldFlags.Owner},
            {EntityFields.NegStat1, FieldFlags.Private | FieldFlags.Owner},
            {EntityFields.NegStat2, FieldFlags.Private | FieldFlags.Owner},
            {EntityFields.NegStat3, FieldFlags.Private | FieldFlags.Owner},
            {EntityFields.Resistances, FieldFlags.Private | FieldFlags.Owner | FieldFlags.SpecialInfo},
            {EntityFields.Resistances1, FieldFlags.Private | FieldFlags.Owner | FieldFlags.SpecialInfo},
            {EntityFields.Resistances2, FieldFlags.Private | FieldFlags.Owner | FieldFlags.SpecialInfo},
            {EntityFields.Resistances3, FieldFlags.Private | FieldFlags.Owner | FieldFlags.SpecialInfo},
            {EntityFields.Resistances4, FieldFlags.Private | FieldFlags.Owner | FieldFlags.SpecialInfo},
            {EntityFields.Resistances5, FieldFlags.Private | FieldFlags.Owner | FieldFlags.SpecialInfo},
            {EntityFields.Resistances6, FieldFlags.Private | FieldFlags.Owner | FieldFlags.SpecialInfo},
            {EntityFields.ResistanceBuffModsPositive, FieldFlags.Private | FieldFlags.Owner},
            {EntityFields.ResistanceBuffModsPositive1, FieldFlags.Private | FieldFlags.Owner},
            {EntityFields.ResistanceBuffModsPositive2, FieldFlags.Private | FieldFlags.Owner},
            {EntityFields.ResistanceBuffModsPositive3, FieldFlags.Private | FieldFlags.Owner},
            {EntityFields.ResistanceBuffModsPositive4, FieldFlags.Private | FieldFlags.Owner},
            {EntityFields.ResistanceBuffModsPositive5, FieldFlags.Private | FieldFlags.Owner},
            {EntityFields.ResistanceBuffModsPositive6, FieldFlags.Private | FieldFlags.Owner},
            {EntityFields.ResistanceBuffModsNegative, FieldFlags.Private | FieldFlags.Owner},
            {EntityFields.ResistanceBuffModsNegative1, FieldFlags.Private | FieldFlags.Owner},
            {EntityFields.ResistanceBuffModsNegative2, FieldFlags.Private | FieldFlags.Owner},
            {EntityFields.ResistanceBuffModsNegative3, FieldFlags.Private | FieldFlags.Owner},
            {EntityFields.ResistanceBuffModsNegative4, FieldFlags.Private | FieldFlags.Owner},
            {EntityFields.ResistanceBuffModsNegative5, FieldFlags.Private | FieldFlags.Owner},
            {EntityFields.ResistanceBuffModsNegative6, FieldFlags.Private | FieldFlags.Owner},
            {EntityFields.BaseMana, FieldFlags.Public},
            {EntityFields.BaseHealth, FieldFlags.Private | FieldFlags.Owner},
            {EntityFields.BaseFlags, FieldFlags.Public},
            {EntityFields.AttackPower, FieldFlags.Private | FieldFlags.Owner},
            {EntityFields.AttackPowerModPos, FieldFlags.Private | FieldFlags.Owner},
            {EntityFields.AttackPowerModNeg, FieldFlags.Private | FieldFlags.Owner},
            {EntityFields.AttackPowerMultiplier, FieldFlags.Private | FieldFlags.Owner},
            {EntityFields.RangedAttackPower, FieldFlags.Private | FieldFlags.Owner},
            {EntityFields.RangedAttackPowerModPos, FieldFlags.Private | FieldFlags.Owner},
            {EntityFields.RangedAttackPowerModNeg, FieldFlags.Private | FieldFlags.Owner},
            {EntityFields.RangedAttackPowerMultiplier, FieldFlags.Private | FieldFlags.Owner},
            {EntityFields.MinRangedDamage, FieldFlags.Private | FieldFlags.Owner},
            {EntityFields.MaxRangedDamage, FieldFlags.Private | FieldFlags.Owner},
            {EntityFields.PowerCostModifier, FieldFlags.Private | FieldFlags.Owner},
            {EntityFields.PowerCostModifier1, FieldFlags.Private | FieldFlags.Owner},
            {EntityFields.PowerCostModifier2, FieldFlags.Private | FieldFlags.Owner},
            {EntityFields.PowerCostModifier3, FieldFlags.Private | FieldFlags.Owner},
            {EntityFields.PowerCostModifier4, FieldFlags.Private | FieldFlags.Owner},
            {EntityFields.PowerCostModifier5, FieldFlags.Private | FieldFlags.Owner},
            {EntityFields.PowerCostModifier6, FieldFlags.Private | FieldFlags.Owner},
            {EntityFields.PowerCostMultiplier, FieldFlags.Private | FieldFlags.Owner},
            {EntityFields.PowerCostMultiplier1, FieldFlags.Private | FieldFlags.Owner},
            {EntityFields.PowerCostMultiplier2, FieldFlags.Private | FieldFlags.Owner},
            {EntityFields.PowerCostMultiplier3, FieldFlags.Private | FieldFlags.Owner},
            {EntityFields.PowerCostMultiplier4, FieldFlags.Private | FieldFlags.Owner},
            {EntityFields.PowerCostMultiplier5, FieldFlags.Private | FieldFlags.Owner},
            {EntityFields.PowerCostMultiplier6, FieldFlags.Private | FieldFlags.Owner},

            {EntityFields.PlayerFlags, FieldFlags.Public},
            {EntityFields.ArenaTeam, FieldFlags.Public},
            {EntityFields.DuelTeam, FieldFlags.Public},
            {EntityFields.ChosenTitle, FieldFlags.Public},
            {EntityFields.CurrentSpecId, FieldFlags.Public},
            {EntityFields.Coinage, FieldFlags.Private},
            {EntityFields.Xp, FieldFlags.Private},
            {EntityFields.PlayerCharacterPoints, FieldFlags.Private},
            {EntityFields.NextLevelXp, FieldFlags.Private},
            {EntityFields.BlockPercentage, FieldFlags.Private},
            {EntityFields.DodgePercentage, FieldFlags.Private},
            {EntityFields.DodgePercentageFromAttribute, FieldFlags.Private},
            {EntityFields.ParryPercentage, FieldFlags.Private},
            {EntityFields.ParryPercentageFromAttribute, FieldFlags.Private},
            {EntityFields.CritPercentage, FieldFlags.Private},
            {EntityFields.RangedCritPercentage, FieldFlags.Private},
            {EntityFields.OffhandCritPercentage, FieldFlags.Private},
            {EntityFields.SpellCritPercentage, FieldFlags.Private},
            {EntityFields.ShieldBlock, FieldFlags.Private},
            {EntityFields.ShieldBlockCritPercentage, FieldFlags.Private},
            {EntityFields.Mastery, FieldFlags.Private},
            {EntityFields.ModDamageDonePos, FieldFlags.Private},
            {EntityFields.ModDamageDonePos1, FieldFlags.Private},
            {EntityFields.ModDamageDonePos2, FieldFlags.Private},
            {EntityFields.ModDamageDonePos3, FieldFlags.Private},
            {EntityFields.ModDamageDonePos4, FieldFlags.Private},
            {EntityFields.ModDamageDonePos5, FieldFlags.Private},
            {EntityFields.ModDamageDonePos6, FieldFlags.Private},
            {EntityFields.ModDamageDoneNeg, FieldFlags.Private},
            {EntityFields.ModDamageDoneNeg1, FieldFlags.Private},
            {EntityFields.ModDamageDoneNeg2, FieldFlags.Private},
            {EntityFields.ModDamageDoneNeg3, FieldFlags.Private},
            {EntityFields.ModDamageDoneNeg4, FieldFlags.Private},
            {EntityFields.ModDamageDoneNeg5, FieldFlags.Private},
            {EntityFields.ModDamageDoneNeg6, FieldFlags.Private},
            {EntityFields.ModDamageDonePct, FieldFlags.Private},
            {EntityFields.ModDamageDonePct1, FieldFlags.Private},
            {EntityFields.ModDamageDonePct2, FieldFlags.Private},
            {EntityFields.ModDamageDonePct3, FieldFlags.Private},
            {EntityFields.ModDamageDonePct4, FieldFlags.Private},
            {EntityFields.ModDamageDonePct5, FieldFlags.Private},
            {EntityFields.ModDamageDonePct6, FieldFlags.Private},
            {EntityFields.OverrideSpellPowerByApPct, FieldFlags.Private},
            {EntityFields.OverrideApBySpellPowerPercent, FieldFlags.Private},
            {EntityFields.ModTargetResistance, FieldFlags.Private},
            {EntityFields.ModTargetPhysicalResistance, FieldFlags.Private},
            {EntityFields.CombatRating1, FieldFlags.Private},
            {EntityFields.CombatRating2, FieldFlags.Private},
            {EntityFields.CombatRating3, FieldFlags.Private},
            {EntityFields.CombatRating4, FieldFlags.Private},
            {EntityFields.CombatRating5, FieldFlags.Private},
            {EntityFields.CombatRating6, FieldFlags.Private},
            {EntityFields.CombatRating7, FieldFlags.Private},
            {EntityFields.CombatRating8, FieldFlags.Private},
            {EntityFields.CombatRating9, FieldFlags.Private},
            {EntityFields.CombatRating10, FieldFlags.Private},

            {EntityFields.ItemOwner, FieldFlags.Public},
            {EntityFields.ItemContained, FieldFlags.Public},
            {EntityFields.ItemCreator, FieldFlags.Public},
            {EntityFields.ItemGiftcreator, FieldFlags.Public},
            {EntityFields.ItemStackCount, FieldFlags.Owner},
            {EntityFields.ItemDuration, FieldFlags.Owner},
            {EntityFields.ItemSpellCharges, FieldFlags.Public},
            {EntityFields.ItemSpellCharges1, FieldFlags.Public},
            {EntityFields.ItemSpellCharges2, FieldFlags.Public},
            {EntityFields.ItemSpellCharges3, FieldFlags.Public},
            {EntityFields.ItemSpellCharges4, FieldFlags.Public},
            {EntityFields.ItemFlags, FieldFlags.Public},
            {EntityFields.ItemEnchantmentID, FieldFlags.Public},
            {EntityFields.ItemEnchantmentDuration, FieldFlags.Public},
            {EntityFields.ItemEnchantmentCharges, FieldFlags.Public},
            {EntityFields.ItemEnchantmentID1, FieldFlags.Public},
            {EntityFields.ItemEnchantmentDuration1, FieldFlags.Public},
            {EntityFields.ItemEnchantmentCharges1, FieldFlags.Public},
            {EntityFields.ItemEnchantmentID2, FieldFlags.Public},
            {EntityFields.ItemEnchantmentDuration2, FieldFlags.Public},
            {EntityFields.ItemEnchantmentCharges2, FieldFlags.Public},
            {EntityFields.ItemEnchantmentID3, FieldFlags.Public},
            {EntityFields.ItemEnchantmentDuration3, FieldFlags.Public},
            {EntityFields.ItemEnchantmentCharges3, FieldFlags.Public},
            {EntityFields.ItemEnchantmentID4, FieldFlags.Public},
            {EntityFields.ItemEnchantmentDuration4, FieldFlags.Public},
            {EntityFields.ItemEnchantmentCharges4, FieldFlags.Public},
            {EntityFields.ItemEnchantmentID5, FieldFlags.Public},
            {EntityFields.ItemEnchantmentDuration5, FieldFlags.Public},
            {EntityFields.ItemEnchantmentCharges5, FieldFlags.Public},
            {EntityFields.ItemEnchantmentID6, FieldFlags.Public},
            {EntityFields.ItemEnchantmentDuration6, FieldFlags.Public},
            {EntityFields.ItemEnchantmentCharges6, FieldFlags.Public},
            {EntityFields.ItemEnchantmentID7, FieldFlags.Public},
            {EntityFields.ItemEnchantmentDuration7, FieldFlags.Public},
            {EntityFields.ItemEnchantmentCharges7, FieldFlags.Public},
            {EntityFields.ItemEnchantmentID8, FieldFlags.Public},
            {EntityFields.ItemEnchantmentDuration8, FieldFlags.Public},
            {EntityFields.ItemEnchantmentCharges8, FieldFlags.Public},
            {EntityFields.ItemEnchantmentID9, FieldFlags.Public},
            {EntityFields.ItemEnchantmentDuration9, FieldFlags.Public},
            {EntityFields.ItemEnchantmentCharges9, FieldFlags.Public},
            {EntityFields.ItemEnchantmentID10, FieldFlags.Public},
            {EntityFields.ItemEnchantmentDuration10, FieldFlags.Public},
            {EntityFields.ItemEnchantmentCharges10, FieldFlags.Public},
            {EntityFields.ItemEnchantmentID11, FieldFlags.Public},
            {EntityFields.ItemEnchantmentDuration11, FieldFlags.Public},
            {EntityFields.ItemEnchantmentCharges11, FieldFlags.Public},
            {EntityFields.ItemEnchantmentID12, FieldFlags.Public},
            {EntityFields.ItemEnchantmentDuration12, FieldFlags.Public},
            {EntityFields.ItemEnchantmentCharges12, FieldFlags.Public},
            {EntityFields.ItemPropertySeed, FieldFlags.Public},
            {EntityFields.ItemRandomPropertiesID, FieldFlags.Public},
            {EntityFields.ItemDurability, FieldFlags.Owner},
            {EntityFields.ItemMaxdurability, FieldFlags.Owner},
            {EntityFields.ItemCreatePlayedTime, FieldFlags.Owner},
            {EntityFields.ItemModifiersMask, FieldFlags.Owner},
            {EntityFields.ItemContext, FieldFlags.Public},
            {EntityFields.ItemArtifactXP, FieldFlags.Owner},
            {EntityFields.ItemAppearanceModID, FieldFlags.Owner},

            {EntityFields.ContainerFieldSlot1, FieldFlags.Public},
            {EntityFields.ContainerFieldSlot2, FieldFlags.Public},
            {EntityFields.ContainerFieldSlot3, FieldFlags.Public},
            {EntityFields.ContainerFieldSlot4, FieldFlags.Public},
            {EntityFields.ContainerFieldSlot5, FieldFlags.Public},
            {EntityFields.ContainerFieldSlot6, FieldFlags.Public},
            {EntityFields.ContainerFieldSlot7, FieldFlags.Public},
            {EntityFields.ContainerFieldSlot8, FieldFlags.Public},
            {EntityFields.ContainerFieldSlot9, FieldFlags.Public},
            {EntityFields.ContainerFieldSlot10, FieldFlags.Public},
            {EntityFields.ContainerFieldSlot11, FieldFlags.Public},
            {EntityFields.ContainerFieldSlot12, FieldFlags.Public},
            {EntityFields.ContainerFieldSlot13, FieldFlags.Public},
            {EntityFields.ContainerFieldSlot14, FieldFlags.Public},
            {EntityFields.ContainerFieldSlot15, FieldFlags.Public},
            {EntityFields.ContainerFieldSlot16, FieldFlags.Public},
            {EntityFields.ContainerFieldSlot17, FieldFlags.Public},
            {EntityFields.ContainerFieldSlot18, FieldFlags.Public},
            {EntityFields.ContainerFieldSlot19, FieldFlags.Public},
            {EntityFields.ContainerFieldSlot20, FieldFlags.Public},
            {EntityFields.ContainerFieldSlot21, FieldFlags.Public},
            {EntityFields.ContainerFieldSlot22, FieldFlags.Public},
            {EntityFields.ContainerFieldSlot23, FieldFlags.Public},
            {EntityFields.ContainerFieldSlot24, FieldFlags.Public},
            {EntityFields.ContainerFieldSlot25, FieldFlags.Public},
            {EntityFields.ContainerFieldSlot26, FieldFlags.Public},
            {EntityFields.ContainerFieldSlot27, FieldFlags.Public},
            {EntityFields.ContainerFieldSlot28, FieldFlags.Public},
            {EntityFields.ContainerFieldSlot29, FieldFlags.Public},
            {EntityFields.ContainerFieldSlot30, FieldFlags.Public},
            {EntityFields.ContainerFieldSlot31, FieldFlags.Public},
            {EntityFields.ContainerFieldSlot32, FieldFlags.Public},
            {EntityFields.ContainerFieldSlot33, FieldFlags.Public},
            {EntityFields.ContainerFieldSlot34, FieldFlags.Public},
            {EntityFields.ContainerFieldSlot35, FieldFlags.Public},
            {EntityFields.ContainerFieldSlot36, FieldFlags.Public},
            {EntityFields.ContainerFieldNumSlots, FieldFlags.Public},

            {EntityFields.CorpseOwner, FieldFlags.Public},
            {EntityFields.CorpseParty, FieldFlags.Public},
            {EntityFields.CorpseDisplayID, FieldFlags.Public},
            {EntityFields.CorpseItem, FieldFlags.Public},
            {EntityFields.CorpseItem1, FieldFlags.Public},
            {EntityFields.CorpseItem2, FieldFlags.Public},
            {EntityFields.CorpseItem3, FieldFlags.Public},
            {EntityFields.CorpseItem4, FieldFlags.Public},
            {EntityFields.CorpseItem5, FieldFlags.Public},
            {EntityFields.CorpseItem6, FieldFlags.Public},
            {EntityFields.CorpseItem7, FieldFlags.Public},
            {EntityFields.CorpseItem8, FieldFlags.Public},
            {EntityFields.CorpseItem9, FieldFlags.Public},
            {EntityFields.CorpseItem10, FieldFlags.Public},
            {EntityFields.CorpseItem11, FieldFlags.Public},
            {EntityFields.CorpseItem12, FieldFlags.Public},
            {EntityFields.CorpseItem13, FieldFlags.Public},
            {EntityFields.CorpseItem14, FieldFlags.Public},
            {EntityFields.CorpseItem15, FieldFlags.Public},
            {EntityFields.CorpseItem16, FieldFlags.Public},
            {EntityFields.CorpseItem17, FieldFlags.Public},
            {EntityFields.CorpseItem18, FieldFlags.Public},
            {EntityFields.CorpseInfo, FieldFlags.Public},
            {EntityFields.CorpseExtraInfo, FieldFlags.Public},
            {EntityFields.CorpseFlags, FieldFlags.Public},
            {EntityFields.CorpseDynamicFlags, FieldFlags.Dynamic},
            {EntityFields.CorpseFactiontemplate, FieldFlags.Public},
            {EntityFields.CorpseCustomDisplayOption, FieldFlags.Public},
        };

        private static readonly List<EntityFields> BaseEntityFieldsOnly = Enum.GetValues(typeof(BaseEntityFields)).Cast<EntityFields>().ToList();
        private static readonly List<EntityFields> GameEntityFieldsOnly = Enum.GetValues(typeof(GameEntityFields)).Cast<EntityFields>().ToList();
        private static readonly List<EntityFields> DynamicEntityFieldsOnly = Enum.GetValues(typeof(DynamicEntityFields)).Cast<EntityFields>().ToList();
        private static readonly List<EntityFields> UnitFieldsOnly = Enum.GetValues(typeof(UnitFields)).Cast<EntityFields>().ToList();
        private static readonly List<EntityFields> PlayerFieldsOnly = Enum.GetValues(typeof(PlayerFields)).Cast<EntityFields>().ToList();
        private static readonly List<EntityFields> ItemFieldsOnly = Enum.GetValues(typeof(ItemFields)).Cast<EntityFields>().ToList();
        private static readonly List<EntityFields> ContainerFieldsOnly = Enum.GetValues(typeof(ContainerFields)).Cast<EntityFields>().ToList();

        private static readonly ReadOnlyCollection<EntityFields> BaseEntityFieldList = BaseEntityFieldsOnly.AsReadOnly();
        private static readonly ReadOnlyCollection<EntityFields> GameEntityFieldList = BaseEntityFieldList.Concat(GameEntityFieldsOnly).ToList().AsReadOnly();
        private static readonly ReadOnlyCollection<EntityFields> DynamicEntityFieldList = BaseEntityFieldList.Concat(DynamicEntityFieldsOnly).ToList().AsReadOnly();
        private static readonly ReadOnlyCollection<EntityFields> UnitFieldList = BaseEntityFieldList.Concat(UnitFieldsOnly).ToList().AsReadOnly();
        private static readonly ReadOnlyCollection<EntityFields> PlayerFieldList = UnitFieldList.Concat(PlayerFieldsOnly).ToList().AsReadOnly();
        private static readonly ReadOnlyCollection<EntityFields> ItemFieldList = BaseEntityFieldList.Concat(ItemFieldsOnly).ToList().AsReadOnly();
        private static readonly ReadOnlyCollection<EntityFields> ContainerFieldList = ItemFieldList.Concat(ContainerFieldsOnly).ToList().AsReadOnly();

        #endregion

        private static readonly Dictionary<UnitMoveType, float> BaseMoveSpeed = new Dictionary<UnitMoveType, float>()
        {
            { UnitMoveType.Run, 7.0f },
            { UnitMoveType.RunBack, 4.5f },
            { UnitMoveType.TurnRate, 3.14f },
            { UnitMoveType.PitchRate, 3.14f },
        };

        public const float ContactDistance = 0.5f;
        public const float InteractionDistance = 5.0f;
        public const float AttackDistance = 5.0f;
        public const float InspectDistance = 28.0f;
        public const float TradeDistance = 11.11f;
        public const float SightRangeUnit = 50.0f;
        public const float DefaultCombatReach = 1.5f;
        public const float MinMeleeReach = 2.0f;
        public const float NominalMeleeRange = 5.0f;
        public const float MeleeRange = NominalMeleeRange - MinMeleeReach * 2;
        public const float DefaultPhase = 169;
        public const int CorpseReclaimRadius = 50;
        public const int AIDefaultCooldown = 5000;

        public static FieldTypes GetFieldType(this EntityFields entityField)
        {
            return EntityFieldTypes[entityField];
        }

        public static FieldFlags GetFieldFlags(this EntityFields entityField)
        {
            return EntityFieldFlags[entityField];
        }

        public static bool HasFlags(this EntityFields entityField, FieldFlags flags)
        {
            return (GetFieldFlags(entityField) & flags) == flags;
        }

        public static bool HasAnyFlag(this EntityFields entityField, FieldFlags flags)
        {
            return (GetFieldFlags(entityField) & flags) != 0;
        }

        public static EntityFields StatField(this StatType statType)
        {
            switch (statType)
            {
                case StatType.Strength:
                    return EntityFields.Stat;
                case StatType.Agility:
                    return EntityFields.Stat1;
                case StatType.Stamina:
                    return EntityFields.Stat2;
                case StatType.Intellect:
                    return EntityFields.Stat3;
                default:
                    throw new NotImplementedException("EntityField for statType: " + statType + " is not implemented yet!");
            }
        }

        public static EntityFields StatPositiveField(this StatType statType)
        {
            switch (statType)
            {
                case StatType.Strength:
                    return EntityFields.PosStat;
                case StatType.Agility:
                    return EntityFields.PosStat1;
                case StatType.Stamina:
                    return EntityFields.PosStat2;
                case StatType.Intellect:
                    return EntityFields.PosStat3;
                default:
                    throw new NotImplementedException("EntityField for positive statType: " + statType + " is not implemented yet!");
            }
        }

        public static EntityFields StatNegativeField(this StatType statType)
        {
            switch (statType)
            {
                case StatType.Strength:
                    return EntityFields.NegStat;
                case StatType.Agility:
                    return EntityFields.NegStat1;
                case StatType.Stamina:
                    return EntityFields.NegStat2;
                case StatType.Intellect:
                    return EntityFields.NegStat3;
                default:
                    throw new NotImplementedException("EntityField for negative statType: " + statType + " is not implemented yet!");
            }
        }

        public static EntityFields ResistanceField(this SpellSchools school)
        {
            switch (school)
            {
                case SpellSchools.Normal:
                    return EntityFields.Resistances;
                case SpellSchools.Holy:
                    return EntityFields.Resistances1;
                case SpellSchools.Fire:
                    return EntityFields.Resistances2;
                case SpellSchools.Nature:
                    return EntityFields.Resistances3;
                case SpellSchools.Frost:
                    return EntityFields.Resistances4;
                case SpellSchools.Shadow:
                    return EntityFields.Resistances5;
                case SpellSchools.Arcane:
                    return EntityFields.Resistances6;
                default:
                    throw new NotImplementedException("EntityField for resistance school: " + school + " is not implemented yet!");
            }
        }

        public static EntityFields ResistanceBuffModPositiveField(this SpellSchools school)
        {
            switch (school)
            {
                case SpellSchools.Normal:
                    return EntityFields.ResistanceBuffModsPositive;
                case SpellSchools.Holy:
                    return EntityFields.ResistanceBuffModsPositive1;
                case SpellSchools.Fire:
                    return EntityFields.ResistanceBuffModsPositive2;
                case SpellSchools.Nature:
                    return EntityFields.ResistanceBuffModsPositive3;
                case SpellSchools.Frost:
                    return EntityFields.ResistanceBuffModsPositive4;
                case SpellSchools.Shadow:
                    return EntityFields.ResistanceBuffModsPositive5;
                case SpellSchools.Arcane:
                    return EntityFields.ResistanceBuffModsPositive6;
                default:
                    throw new NotImplementedException("EntityField for resistance buff mod positive school: " + school + " is not implemented yet!");
            }
        }

        public static EntityFields ResistanceBuffModNegativeField(this SpellSchools school)
        {
            switch (school)
            {
                case SpellSchools.Normal:
                    return EntityFields.ResistanceBuffModsNegative;
                case SpellSchools.Holy:
                    return EntityFields.ResistanceBuffModsNegative1;
                case SpellSchools.Fire:
                    return EntityFields.ResistanceBuffModsNegative2;
                case SpellSchools.Nature:
                    return EntityFields.ResistanceBuffModsNegative3;
                case SpellSchools.Frost:
                    return EntityFields.ResistanceBuffModsNegative4;
                case SpellSchools.Shadow:
                    return EntityFields.ResistanceBuffModsNegative5;
                case SpellSchools.Arcane:
                    return EntityFields.ResistanceBuffModsNegative6;
                default:
                    throw new NotImplementedException("EntityField for resistance buff mod negative school: " + school + " is not implemented yet!");
            }
        }

        public static EntityFields CombatRatingField(this CombatRating rating)
        {
            switch (rating)
            {
                case CombatRating.Dodge:
                    return EntityFields.CombatRating1;
                case CombatRating.Crit:
                    return EntityFields.CombatRating2;
                case CombatRating.MultiStrike:
                    return EntityFields.CombatRating3;
                case CombatRating.Speed:
                    return EntityFields.CombatRating4;
                case CombatRating.Haste:
                    return EntityFields.CombatRating5;
                case CombatRating.ArmorPenetration:
                    return EntityFields.CombatRating6;
                case CombatRating.Mastery:
                    return EntityFields.CombatRating7;
                case CombatRating.PvpPower:
                    return EntityFields.CombatRating8;
                case CombatRating.ResilenceCritTaken:
                    return EntityFields.CombatRating9;
                case CombatRating.ResilencePlayerDamage:
                    return EntityFields.CombatRating10;
                default:
                    throw new NotImplementedException("EntityField for combat rating: " + rating + " is not implemented yet!");
            }
        }

        public static ReadOnlyCollection<EntityFields> GetEntityFields(EntityType typeId)
        {
            switch (typeId)
            {
                case EntityType.Unit:
                    return UnitFieldList;
                case EntityType.Player:
                    return PlayerFieldList;
                case EntityType.GameEntity:
                    return GameEntityFieldList;
                case EntityType.DynamicEntity:
                    return DynamicEntityFieldList;
                case EntityType.Item:
                    return ItemFieldList;
                case EntityType.Container:
                    return ContainerFieldList;
                case EntityType.Entity:
                    return BaseEntityFieldList;
                default:
                    throw new NotImplementedException("Entity type " + typeId + " is not implemented!");
            }
        }

        public static void InitializeSpeedRates(Dictionary<UnitMoveType, float> speedRates)
        {
            speedRates.Clear();

            foreach (var speedRate in BaseMoveSpeed)
                speedRates.Add(speedRate.Key, 1.0f);
        }

        public static float BaseMovementSpeed(UnitMoveType moveType)
        {
            return BaseMoveSpeed[moveType];
        }

        #region Percentage calculations

        public static int CalculatePercentage(this int baseValue, float pct)
        {
            return (int)(baseValue * pct / 100.0f);
        }

        public static uint CalculatePercentage(this uint baseValue, float pct)
        {
            return (uint)(baseValue * pct / 100.0f);
        }

        public static long CalculatePercentage(this long baseValue, float pct)
        {
            return (long)(baseValue * pct / 100.0f);
        }

        public static float CalculatePercentage(this float baseValue, float pct)
        {
            return baseValue * (pct) / 100.0f;
        }

        public static int AddPercentage(this int baseValue, float pct)
        {
            return baseValue + CalculatePercentage(baseValue, pct);
        }

        public static uint AddPercentage(this uint baseValue, float pct)
        {
            return baseValue + CalculatePercentage(baseValue, pct);
        }

        public static long AddPercentage(this long baseValue, float pct)
        {
            return baseValue + CalculatePercentage(baseValue, pct);
        }

        public static float AddPercentage(this float baseValue, float pct)
        {
            return baseValue + CalculatePercentage(baseValue, pct);
        }

        public static int ApplyPercentage(this int baseValue, float pct)
        {
            return CalculatePercentage(baseValue, pct);
        }

        public static uint ApplyPercentage(this uint baseValue, float pct)
        {
            return CalculatePercentage(baseValue, pct);
        }

        public static long ApplyPercentage(this long baseValue, float pct)
        {
            return CalculatePercentage(baseValue, pct);
        }

        public static float ApplyPercentage(this float baseValue, float pct)
        {
            return CalculatePercentage(baseValue, pct);
        }

        #endregion
    }
}