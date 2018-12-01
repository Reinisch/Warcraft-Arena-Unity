using System;
using System.Collections.Generic;

namespace Core
{
    public class Player : Unit, IGridEntity<Player>
    {
        protected Team team;
        protected PlayerExtraFlags extraFlags;
        protected Dictionary<uint, PlayerSpell> playerSpells;
        protected SpecializationInfo specializationInfo;

        protected Dictionary<SpellModOp, List<SpellModifier>> spellMods;
        protected Dictionary<BaseModGroup, Dictionary<BaseModType, float>> auraBaseMod;
        protected Dictionary<CombatRating, short> baseRatingValue;
        protected Dictionary<byte, ActionButton> actionButtons;

        protected uint baseSpellPower;
        protected uint baseManaRegen;
        protected uint baseHealthRegen;
        protected int spellPenetrationItemMod;
        protected ResurrectionData resurrectionData;

        protected bool canParry;
        protected bool canBlock;
        protected bool canTitanGrip;
        protected Runes runes;

        // Rune type / Rune timer
        private uint[] m_runeGraceCooldown = new uint[PlayerHelper.MaxRunes];
        private uint[] m_lastRuneGraceTimers = new uint[PlayerHelper.MaxRunes];

        public GridReference<Player> GridRef { get; private set; }
  
        public string AutoReplyMsg { get; set; }
        public override DeathState DeathState { get { return base.DeathState; } set { throw new NotImplementedException(); } }
        public override float Scale { get { return base.Scale; } set { throw new NotImplementedException(); } }

        public bool IsInGrid() { throw new NotImplementedException(); }
        public void AddToGrid(GridReferenceManager<Player> refManager) { throw new NotImplementedException(); }
        public void RemoveFromGrid() { throw new NotImplementedException(); }

        public WorldLocation GetStartPosition() { throw new NotImplementedException(); }

        public override void AddToWorld() { throw new NotImplementedException(); }
        public override void RemoveFromWorld() { throw new NotImplementedException(); }
        public override void CleanupsBeforeDelete(bool finalCleanup = true) { throw new NotImplementedException(); }

        public override void DestroyForPlayer(Player target) { throw new NotImplementedException(); }
        public override void DoUpdate(uint time) { throw new NotImplementedException(); }
        public override bool IsImmunedToSpellEffect(SpellInfo spellInfo, int index) { throw new NotImplementedException(); }

        #region Player flags

        public bool IsGameMaster() { return (extraFlags & PlayerExtraFlags.GmOn) != 0; }

        #endregion

        #region Movement

        public void SetHomebind(WorldLocation loc, uint areaId) { throw new NotImplementedException(); }
        public void SendBindPointUpdate() { throw new NotImplementedException(); }

        public bool TeleportTo(uint mapid, float x, float y, float z, float orientation, uint options = 0) { throw new NotImplementedException(); }
        public bool TeleportTo(WorldLocation loc, uint options = 0) { throw new NotImplementedException(); }
        public bool TeleportToBGEntryPoint() { throw new NotImplementedException(); }

        public void SetClientControl(Unit target, bool allowMove) { throw new NotImplementedException(); }
        public void SetMover(Unit target) { throw new NotImplementedException(); }

        public void ValidateMovementInfo(MovementInfo mi) { throw new NotImplementedException(); }
        public void SendMovementSetCollisionHeight(float height) { throw new NotImplementedException(); }

        public override bool CanFly() { return MovementInfo.HasMovementFlag(MovementFlags.CanFly); }
        public float GetCollisionHeight(bool mounted) { throw new NotImplementedException(); }

        #endregion

        #region Resurrect

        public void SetResurrectRequestData(Unit caster, uint health, uint mana, uint appliedAura) { throw new NotImplementedException(); }
        public void ClearResurrectRequestData()
        {
            resurrectionData = null;
        }

        public bool IsResurrectRequestedBy(Guid guid)
        {
            if (!IsResurrectRequested())
                return false;

            return resurrectionData.GUID != Guid.Empty && resurrectionData.GUID == guid;
        }
        public bool IsResurrectRequested() { return resurrectionData != null; }
        public void ResurrectUsingRequestData() { throw new NotImplementedException(); }
        public void ResurrectUsingRequestDataImpl() { throw new NotImplementedException(); }

        #endregion

        #region Gossip system

        public void PrepareGossipMenu(WorldEntity source, uint menuId = 0, bool showQuests = false) { throw new NotImplementedException(); }
        public void SendPreparedGossip(WorldEntity source) { throw new NotImplementedException(); }
        public void OnGossipSelect(WorldEntity source, uint gossipListId, uint menuId) { throw new NotImplementedException(); }

        public uint GetGossipTextId(uint menuId, WorldEntity source) { throw new NotImplementedException(); }
        public uint GetGossipTextId(WorldEntity source) { throw new NotImplementedException(); }
        public static uint GetDefaultGossipMenuForSource(WorldEntity source) { throw new NotImplementedException(); }

        #endregion

        #region Regeneration and combo

        public void RegenerateAll() { throw new NotImplementedException(); }
        public void Regenerate(PowerType power) { throw new NotImplementedException(); }
        public void RegenerateHealth() { throw new NotImplementedException(); }

        public int GetComboPoints() { return GetPower(PowerType.ComboPoints); }
        public void AddComboPoints(byte count, Spell spell = null) { throw new NotImplementedException(); }
        public void GainSpellComboPoints(byte count) { throw new NotImplementedException(); }
        public void ClearComboPoints() { throw new NotImplementedException(); }

        #endregion

        #region Target methods

        public Unit GetSelectedUnit() { throw new NotImplementedException(); }
        public Player GetSelectedPlayer() { throw new NotImplementedException(); }

        public override void SetTarget(Guid guid) { } // Used for serverside target changes, does not apply to players
        public void SetSelection(Guid guid) { SetGuidValue(EntityFields.Target, guid); }

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

        #region Action buttons

        ActionButton AddActionButton(byte button, uint action, byte type) { throw new NotImplementedException(); }
        void RemoveActionButton(byte button) { throw new NotImplementedException(); }
        ActionButton  GetActionButton(byte button) { throw new NotImplementedException(); }
        void SendInitialActionButtons() { SendActionButtons(0); }
        void SendActionButtons(uint state) { throw new NotImplementedException(); }
        bool IsActionButtonDataValid(byte button, uint action, byte type) { throw new NotImplementedException(); }

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
        public uint GetRuneTimer(byte index) { return m_runeGraceCooldown[index]; }
        public void SetRuneTimer(byte index, uint timer) { m_runeGraceCooldown[index] = timer; }
        public uint GetLastRuneGraceTimer(byte index) { return m_lastRuneGraceTimers[index]; }
        public void SetLastRuneGraceTimer(byte index, uint timer) { m_lastRuneGraceTimers[index] = timer; }
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