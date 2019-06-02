using System;
using Client;
using JetBrains.Annotations;
using UnityEngine;

namespace Core
{
    public class Player : Unit
    {
        [SerializeField, UsedImplicitly, Header("Player"), Space(10)] private WarcraftController controller;

        public GridReference<Player> GridReference { get; } = new GridReference<Player>();
        public WarcraftController Controller => controller;
        
        public override bool AutoScoped => true;
        public override EntityType EntityType => EntityType.Player;
        public override DeathState DeathState { get => base.DeathState; set => throw new NotImplementedException(); }

        public override bool IsImmunedToSpellEffect(SpellInfo spellInfo, int index)
        {
            return false;
        }

        public override void Accept(IUnitVisitor visitor)
        {
            visitor.Visit(this);
        }

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

        public void ApplySpellPowerBonus(int amount, bool apply) { throw new NotImplementedException(); }
        public void UpdateSpellDamageAndHealingBonus() { throw new NotImplementedException(); }
        public void ApplyRatingMod(CombatRating cr, int value, bool apply) { throw new NotImplementedException(); }
        public void UpdateRating(CombatRating cr) { throw new NotImplementedException(); }
        public void UpdateAllRatings() { throw new NotImplementedException(); }
        public void UpdateMastery() { throw new NotImplementedException(); }
        public bool CanUseMastery() { throw new NotImplementedException(); }

        public void RecalculateRating(CombatRating cr) { ApplyRatingMod(cr, 0, true); }
        public void GetDodgeFromAgility(ref float diminishing, ref float nondiminishing) { throw new NotImplementedException(); }
        public float GetRatingMultiplier(CombatRating cr) { throw new NotImplementedException(); }
        public float GetRatingBonusValue(CombatRating cr) { throw new NotImplementedException(); }

        public void UpdateSpellCritChance() { throw new NotImplementedException(); }
        public void UpdateArmorPenetration(int amount) { throw new NotImplementedException(); }
        public void UpdateExpertise(WeaponAttackType attType) { throw new NotImplementedException(); }
        public void ApplyManaRegenBonus(int amount, bool apply) { throw new NotImplementedException(); }
        public void ApplyHealthRegenBonus(int amount, bool apply) { throw new NotImplementedException(); }
        public void UpdateManaRegen() { throw new NotImplementedException(); }

        public void SetRegularAttackTime() { throw new NotImplementedException(); }
        public void HandleBaseModValue(BaseModGroup modGroup, BaseModType modType, float amount, bool apply) { throw new NotImplementedException(); }
        public float GetBaseModValue(BaseModGroup modGroup, BaseModType modType) { throw new NotImplementedException(); }
        public float GetTotalBaseModValue(BaseModGroup modGroup) { throw new NotImplementedException(); }
        public void _ApplyAllStatBonuses() { throw new NotImplementedException(); }
        public void _RemoveAllStatBonuses() { throw new NotImplementedException(); }
        public void ResetAllPowers() { throw new NotImplementedException(); }

        public bool IsImmuneToEnvironmentalDamage() { throw new NotImplementedException(); }
        public uint EnvironmentalDamage(EnviromentalDamage type, int damage) { throw new NotImplementedException(); }

        #endregion
    }
}