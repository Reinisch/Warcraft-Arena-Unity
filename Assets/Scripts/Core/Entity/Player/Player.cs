using System;
using System.Collections.Generic;
using Client;
using JetBrains.Annotations;
using UnityEngine;

namespace Core
{
    public class Player : Unit, IGridEntity<Player>
    {
        [SerializeField, UsedImplicitly, Header("Player"), Space(10)] private WarcraftController controller;

        protected Team team;
        protected PlayerExtraFlags extraFlags;
        protected SpecializationInfo specializationInfo;

        protected Dictionary<SpellModOp, List<SpellModifier>> spellMods;
        protected Dictionary<BaseModGroup, Dictionary<BaseModType, float>> auraBaseMod;
        protected Dictionary<CombatRating, short> baseRatingValue;

        protected uint baseSpellPower;
        protected uint baseManaRegen;
        protected uint baseHealthRegen;
        protected int spellPenetrationItemMod;
        protected ResurrectionData resurrectionData;

        protected bool canParry;
        protected bool canBlock;
        protected bool canTitanGrip;
        protected Runes runes;

        private uint[] runeGraceCooldown = new uint[PlayerHelper.MaxRunes];
        private uint[] lastRuneGraceTimers = new uint[PlayerHelper.MaxRunes];

        public override EntityType EntityType => EntityType.Player;
        public override bool AutoScoped => true;

        public WarcraftController Controller => controller;
        public override DeathState DeathState { get { return base.DeathState; } set { throw new NotImplementedException(); } }
        public GridReference<Player> GridRef { get; private set; }

        public bool IsInGrid() { throw new NotImplementedException(); }
        public void AddToGrid(GridReferenceManager<Player> refManager) { throw new NotImplementedException(); }
        public void RemoveFromGrid() { throw new NotImplementedException(); }

        public override void DoUpdate(int timeDelta)
        {

        }

        public override bool IsImmunedToSpellEffect(SpellInfo spellInfo, int index)
        {
            return false;
        }

        public override void Accept(IUnitVisitor visitor)
        {
            visitor.Visit(this);
        }

        #region Player flags

        public bool IsGameMaster() { return (extraFlags & PlayerExtraFlags.GmOn) != 0; }

        #endregion

        #region Target methods

        public Unit GetSelectedUnit() { throw new NotImplementedException(); }
        public Player GetSelectedPlayer() { throw new NotImplementedException(); }

        public override void SetTarget(ulong targetId) { } // Used for serverside target changes, does not apply to players
        public void SetSelection(ulong targetId) { SetULongValue(EntityFields.Target, targetId); }

        #endregion

        #region Spell modifiers and potions

        public void AddSpellMod(SpellModifier mod, bool apply) { throw new NotImplementedException(); }
        public bool IsAffectedBySpellmod(SpellInfo  spellInfo, SpellModifier mod, Spell spell = null) { throw new NotImplementedException(); }
        public void ApplySpellMod(int spellId, SpellModOp op, ref float basevalue, Spell spell = null) { throw new NotImplementedException(); }
        public void ApplySpellMod(int spellId, SpellModOp op, ref int basevalue, Spell spell = null) { throw new NotImplementedException(); }
        public void RemoveSpellMods(Spell spell) { throw new NotImplementedException(); }
        public void RestoreSpellMods(Spell spell, uint ownerAuraId = 0, Aura aura = null) { throw new NotImplementedException(); }
        public void RestoreAllSpellMods(uint ownerAuraId = 0, Aura aura = null) { throw new NotImplementedException(); }
        public void DropModCharge(SpellModifier mod, Spell spell) { throw new NotImplementedException(); }
        public void SetSpellModTakingSpell(Spell spell, bool apply) { throw new NotImplementedException(); }

        #endregion

        #region Stat system

        public override bool UpdateStats(Stats stat) { throw new NotImplementedException(); }
        public override bool UpdateAllStats() { throw new NotImplementedException(); }
        public override void UpdateResistances(SpellSchools school) { throw new NotImplementedException(); }
        public override void UpdateArmor() { throw new NotImplementedException(); }
        public override void UpdateMaxHealth() { throw new NotImplementedException(); }
        public override void UpdateMaxPower(PowerType power) { throw new NotImplementedException(); }
        public override void UpdateAttackPowerAndDamage(bool ranged = false) { throw new NotImplementedException(); }

        public void ApplySpellPowerBonus(int amount, bool apply) { throw new NotImplementedException(); }
        public void UpdateSpellDamageAndHealingBonus() { throw new NotImplementedException(); }
        public void ApplyRatingMod(CombatRating cr, int value, bool apply) { throw new NotImplementedException(); }
        public void UpdateRating(CombatRating cr) { throw new NotImplementedException(); }
        public void UpdateAllRatings() { throw new NotImplementedException(); }
        public void UpdateMastery() { throw new NotImplementedException(); }
        public bool CanUseMastery() { throw new NotImplementedException(); }

        public override void CalculateMinMaxDamage(WeaponAttackType attType, bool normalized, bool addTotalPct, ref float minDamage, ref float maxDamage) { throw new NotImplementedException(); }

        public void RecalculateRating(CombatRating cr) { ApplyRatingMod(cr, 0, true); }
        public void GetDodgeFromAgility(ref float diminishing, ref float nondiminishing) { throw new NotImplementedException(); }
        public float GetRatingMultiplier(CombatRating cr) { throw new NotImplementedException(); }
        public float GetRatingBonusValue(CombatRating cr) { throw new NotImplementedException(); }

        /// Returns base spellpower bonus from spellpower stat on items, without spellpower from intellect stat
        public uint GetBaseSpellPowerBonus() { return baseSpellPower; }
        public int GetSpellPenetrationItemMod() { return spellPenetrationItemMod; }

        public float GetExpertiseDodgeOrParryReduction(WeaponAttackType attType) { throw new NotImplementedException(); }
        public void UpdateBlockPercentage() { throw new NotImplementedException(); }
        public void UpdateCritPercentage(WeaponAttackType attType) { throw new NotImplementedException(); }
        public void UpdateAllCritPercentages() { throw new NotImplementedException(); }
        public void UpdateParryPercentage() { throw new NotImplementedException(); }
        public void UpdateDodgePercentage() { throw new NotImplementedException(); }
        public void UpdateMeleeHitChances() { throw new NotImplementedException(); }
        public void UpdateRangedHitChances() { throw new NotImplementedException(); }
        public void UpdateSpellHitChances() { throw new NotImplementedException(); }

        public void UpdateSpellCritChance() { throw new NotImplementedException(); }
        public void UpdateArmorPenetration(int amount) { throw new NotImplementedException(); }
        public void UpdateExpertise(WeaponAttackType attType) { throw new NotImplementedException(); }
        public void ApplyManaRegenBonus(int amount, bool apply) { throw new NotImplementedException(); }
        public void ApplyHealthRegenBonus(int amount, bool apply) { throw new NotImplementedException(); }
        public void UpdateManaRegen() { throw new NotImplementedException(); }
        public uint GetRuneTimer(byte index) { return runeGraceCooldown[index]; }
        public void SetRuneTimer(byte index, uint timer) { runeGraceCooldown[index] = timer; }
        public uint GetLastRuneGraceTimer(byte index) { return lastRuneGraceTimers[index]; }
        public void SetLastRuneGraceTimer(byte index, uint timer) { lastRuneGraceTimers[index] = timer; }
        public void UpdateAllRunesRegen() { throw new NotImplementedException(); }

        public override uint GetBlockPercent() { return GetUintValue(EntityFields.ShieldBlock); }
        public bool CanParry() { return canParry; }
        public void SetCanParry(bool value) { throw new NotImplementedException(); }
        public bool CanBlock() { return canBlock; }
        public void SetCanBlock(bool value) { throw new NotImplementedException(); }
        public bool CanTitanGrip() { return canTitanGrip; }
        public void SetCanTitanGrip(bool value) { canTitanGrip = value; }
        public bool CanTameExoticPets() { return IsGameMaster() || HasAuraType(AuraType.AllowTamePetType); }

        public void SetRegularAttackTime() { throw new NotImplementedException(); }
        public void SetBaseModValue(BaseModGroup modGroup, BaseModType modType, float value) { auraBaseMod[modGroup][modType] = value; }
        public void HandleBaseModValue(BaseModGroup modGroup, BaseModType modType, float amount, bool apply) { throw new NotImplementedException(); }
        public float GetBaseModValue(BaseModGroup modGroup, BaseModType modType) { throw new NotImplementedException(); }
        public float GetTotalBaseModValue(BaseModGroup modGroup) { throw new NotImplementedException(); }
        public float GetTotalPercentageModValue(BaseModGroup modGroup) { return auraBaseMod[modGroup][BaseModType.FlatMod] + auraBaseMod[modGroup][BaseModType.PercentMod]; }
        public void _ApplyAllStatBonuses() { throw new NotImplementedException(); }
        public void _RemoveAllStatBonuses() { throw new NotImplementedException(); }
        public void ResetAllPowers() { throw new NotImplementedException(); }

        public bool IsImmuneToEnvironmentalDamage() { throw new NotImplementedException(); }
        public uint EnvironmentalDamage(EnviromentalDamage type, int damage) { throw new NotImplementedException(); }

        public byte GetRunesState() { return runes.RuneState; }
        public uint GetRuneCooldown(byte index) { return runes.Cooldown[index]; }
        public uint GetRuneBaseCooldown() { throw new NotImplementedException(); }
        public void SetRuneCooldown(byte index, uint cooldown, bool casted = false) { throw new NotImplementedException(); }
        public void ResyncRunes() { throw new NotImplementedException(); }
        public void AddRunePower(byte index) { throw new NotImplementedException(); }
        public void InitRunes() { throw new NotImplementedException(); }

        #endregion

        #region Notifiers and update position

        public override bool UpdatePosition(float x, float y, float z, float orientation, bool teleport = false)
        {
            throw new NotImplementedException();
        }

        public override bool UpdatePosition(Position pos, bool teleport = false)
        {
            return UpdatePosition(pos.X, pos.Y, pos.Z, pos.Orientation, teleport);
        }

        #endregion
    }
}