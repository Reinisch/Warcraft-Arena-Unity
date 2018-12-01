using System;
using System.Collections.Generic;

namespace Core
{
    public class AuraEffect
    {
        private int baseAmount;

        private int amount;
        private int damage;
        private float critChance;
        private float donePct;

        private SpellModifier spellmod;

        private int periodicTimer;
        private uint tickNumber;

        private int effIndex;
        private bool canBeRecalculated;
        private bool isPeriodic;


        public SpellInfo SpellInfo { get; private set; }
        public Aura BaseAura { get; private set; }
        public int Id => SpellInfo.Id;
        public Unit Caster => BaseAura.Caster;
        public Guid CasterGuid => BaseAura.CasterGuid;

        private SpellEffectInfo EffectInfo { get; set; }


        public AuraEffect(Aura baseAura, int effIndex, int baseAmount, Unit caster)
        {
            BaseAura = baseAura;
            SpellInfo = baseAura.SpellInfo;

            this.effIndex = effIndex;
            this.baseAmount = baseAmount > 0 ? baseAmount : BaseAura.GetSpellEffectInfo(effIndex).BasePoints;
            damage = 0;
            critChance = 0;
            donePct = 1.0f;
            spellmod = null;
            periodicTimer = 0;
            tickNumber = 0;
            this.effIndex = effIndex;
            canBeRecalculated = true;
            isPeriodic = false;

            CalculatePeriodic(caster);

            amount = CalculateAmount(caster);

            CalculateSpellMod();
        }

        public void GetTargetList(List<Unit> targets)
        {
        }
        public void GetApplicationList(List<AuraApplication> applications)
        {
        }
        public SpellModifier GetSpellModifier() { return spellmod; }
        public SpellEffectInfo GetSpellEffectInfo() { return EffectInfo; }

        public int GetEffIndex() { return effIndex; }
        public int GetBaseAmount() { return baseAmount; }

        public AuraType GetAuraType() { return GetSpellEffectInfo().AuraType; }
        public int GetAmount() { return amount; }
        public void SetAmount(int amount) { this.amount = amount; canBeRecalculated = false; }
        public void UpdateBaseAmount(int baseAmount)
        {
            this.baseAmount = baseAmount;
        }

        public int GetPeriodicTimer() { return periodicTimer; }
        public void SetPeriodicTimer(int periodicTimer) { this.periodicTimer = periodicTimer; }

        public int CalculateAmount(Unit caster)
        {
            return 0;
        }
        public void CalculatePeriodic(Unit caster, bool resetPeriodicTimer = true, bool load = false) { }
        public void CalculateSpellMod() { }
        public void ChangeAmount(int newAmount, bool mark = true, bool onStackOrReapply = false) { }

        public void RecalculateAmount()
        {
            if (!CanBeRecalculated())
                return;
            ChangeAmount(CalculateAmount(Caster), false);
        }
        public void RecalculateAmount(Unit caster)
        {
            if (!CanBeRecalculated())
                return;
            ChangeAmount(CalculateAmount(caster), false);
        }

        public bool CanBeRecalculated() { return canBeRecalculated; }
        public void SetCanBeRecalculated(bool val) { canBeRecalculated = val; }
        public void HandleEffect(AuraApplication aurApp, AuraEffectHandleModes mode, bool apply) { }
        public void HandleEffect(Unit target, AuraEffectHandleModes mode, bool apply) { }
        public void ApplySpellMod(Unit target, bool apply) { }

        public void SetDamage(int val) { damage = val; }
        public int GetDamage() { return damage; }
        public void SetCritChance(float val) { critChance = val; }
        public float GetCritChance() { return critChance; }
        public void SetDonePct(float val) { donePct = val; }
        public float GetDonePct() { return donePct; }

        public void Update(uint diff, Unit caster) { }
        public void UpdatePeriodic(Unit caster) { }

        public uint GetTickNumber() { return tickNumber; }
        public bool IsPeriodic() { return isPeriodic; }
        public void SetPeriodic(bool isPeriodic) { this.isPeriodic = isPeriodic; }

        public bool IsAffectingSpell(SpellInfo spellInfo)
        {
            return false;
        }
        public bool HasSpellClassMask()
        {
            return false;
        }

        public void SendTickImmune(Unit target, Unit caster) { }
        public void PeriodicTick(AuraApplication aurApp, Unit caster) { }
        public void HandleProc(AuraApplication aurApp, ProcEventInfo eventInfo) { }

        public bool IsEffect() { return EffectInfo.EffectType != 0; }
        public bool IsEffect(SpellEffectType type) { return EffectInfo.EffectType == type; }
        public bool IsAreaAuraEffect()
        {
            return false;
        }

        private bool CanPeriodicTickCrit(Unit caster)
        {
            return false;
        }


        #region Aura effect Apply/remove handlers

        public void HandleNull(AuraApplication auraApplication, AuraEffectHandleModes mode, bool apply)
        {
            // not implemented
        }
        public void HandleUnused(AuraApplication auraApplication, AuraEffectHandleModes mode, bool apply)
        {
            // useless
        }
        public void HandleNoImmediateEffect(AuraApplication auraApplication, AuraEffectHandleModes mode, bool apply)
        {
            // aura type not have immediate effect at add/remove and handled by ID in other code place
        }
        // add/remove SPELL_AURA_MOD_SHAPESHIFT (36) linked auras
        public void HandleShapeshiftBoosts(Unit target, bool apply) { }
        //  visibility & phases
        public void HandleModInvisibilityDetect(AuraApplication auraApplication, AuraEffectHandleModes mode, bool apply) { }
        public void HandleModInvisibility(AuraApplication auraApplication, AuraEffectHandleModes mode, bool apply) { }
        public void HandleModStealth(AuraApplication auraApplication, AuraEffectHandleModes mode, bool apply) { }
        public void HandleModStealthLevel(AuraApplication auraApplication, AuraEffectHandleModes mode, bool apply) { }
        public void HandleModStealthDetect(AuraApplication auraApplication, AuraEffectHandleModes mode, bool apply) { }
        public void HandleSpiritOfRedemption(AuraApplication auraApplication, AuraEffectHandleModes mode, bool apply) { }
        public void HandleAuraGhost(AuraApplication auraApplication, AuraEffectHandleModes mode, bool apply) { }
        public void HandlePhase(AuraApplication auraApplication, AuraEffectHandleModes mode, bool apply) { }
        public void HandlePhaseGroup(AuraApplication auraApplication, AuraEffectHandleModes mode, bool apply) { }

        //  unit model
        public void HandleAuraModShapeshift(AuraApplication auraApplication, AuraEffectHandleModes mode, bool apply) { }
        public void HandleAuraTransform(AuraApplication auraApplication, AuraEffectHandleModes mode, bool apply) { }
        public void HandleAuraModScale(AuraApplication auraApplication, AuraEffectHandleModes mode, bool apply) { }
        public void HandleAuraCloneCaster(AuraApplication auraApplication, AuraEffectHandleModes mode, bool apply) { }
        //  fight
        public void HandleFeignDeath(AuraApplication auraApplication, AuraEffectHandleModes mode, bool apply) { }
        public void HandleModUnattackable(AuraApplication auraApplication, AuraEffectHandleModes mode, bool apply) { }
        public void HandleAuraModDisarm(AuraApplication auraApplication, AuraEffectHandleModes mode, bool apply) { }
        public void HandleAuraModSilence(AuraApplication auraApplication, AuraEffectHandleModes mode, bool apply) { }
        public void HandleAuraModPacify(AuraApplication auraApplication, AuraEffectHandleModes mode, bool apply) { }
        public void HandleAuraModPacifyAndSilence(AuraApplication auraApplication, AuraEffectHandleModes mode, bool apply) { }
        public void HandleAuraAllowOnlyAbility(AuraApplication auraApplication, AuraEffectHandleModes mode, bool apply) { }
        public void HandleAuraModNoActions(AuraApplication auraApplication, AuraEffectHandleModes mode, bool apply) { }
        //  tracking
        public void HandleAuraTrackResources(AuraApplication auraApplication, AuraEffectHandleModes mode, bool apply) { }
        public void HandleAuraTrackCreatures(AuraApplication auraApplication, AuraEffectHandleModes mode, bool apply) { }
        public void HandleAuraTrackStealthed(AuraApplication auraApplication, AuraEffectHandleModes mode, bool apply) { }
        public void HandleAuraModStalked(AuraApplication auraApplication, AuraEffectHandleModes mode, bool apply) { }
        public void HandleAuraUntrackable(AuraApplication auraApplication, AuraEffectHandleModes mode, bool apply) { }
        //  skills & talents
        public void HandleAuraModSkill(AuraApplication auraApplication, AuraEffectHandleModes mode, bool apply) { }
        //  movement
        public void HandleAuraMounted(AuraApplication auraApplication, AuraEffectHandleModes mode, bool apply) { }
        public void HandleAuraAllowFlight(AuraApplication auraApplication, AuraEffectHandleModes mode, bool apply) { }
        public void HandleAuraWaterWalk(AuraApplication auraApplication, AuraEffectHandleModes mode, bool apply) { }
        public void HandleAuraFeatherFall(AuraApplication auraApplication, AuraEffectHandleModes mode, bool apply) { }
        public void HandleAuraHover(AuraApplication auraApplication, AuraEffectHandleModes mode, bool apply) { }
        public void HandleWaterBreathing(AuraApplication auraApplication, AuraEffectHandleModes mode, bool apply) { }
        public void HandleForceMoveForward(AuraApplication auraApplication, AuraEffectHandleModes mode, bool apply) { }
        public void HandleAuraCanTurnWhileFalling(AuraApplication auraApplication, AuraEffectHandleModes mode, bool apply) { }
        //  threat
        public void HandleModThreat(AuraApplication auraApplication, AuraEffectHandleModes mode, bool apply) { }
        public void HandleAuraModTotalThreat(AuraApplication auraApplication, AuraEffectHandleModes mode, bool apply) { }
        public void HandleModTaunt(AuraApplication auraApplication, AuraEffectHandleModes mode, bool apply) { }
        //  control
        public void HandleModConfuse(AuraApplication auraApplication, AuraEffectHandleModes mode, bool apply) { }
        public void HandleModFear(AuraApplication auraApplication, AuraEffectHandleModes mode, bool apply) { }
        public void HandleAuraModStun(AuraApplication auraApplication, AuraEffectHandleModes mode, bool apply) { }
        public void HandleAuraModRoot(AuraApplication auraApplication, AuraEffectHandleModes mode, bool apply) { }
        public void HandlePreventFleeing(AuraApplication auraApplication, AuraEffectHandleModes mode, bool apply) { }
        //  charm
        public void HandleModPossess(AuraApplication auraApplication, AuraEffectHandleModes mode, bool apply) { }
        public void HandleModPossessPet(AuraApplication auraApplication, AuraEffectHandleModes mode, bool apply) { }
        public void HandleModCharm(AuraApplication auraApplication, AuraEffectHandleModes mode, bool apply) { }
        public void HandleCharmConvert(AuraApplication auraApplication, AuraEffectHandleModes mode, bool apply) { }
        public void HandleAuraControlVehicle(AuraApplication auraApplication, AuraEffectHandleModes mode, bool apply) { }
        //  modify speed
        public void HandleAuraModIncreaseSpeed(AuraApplication auraApplication, AuraEffectHandleModes mode, bool apply) { }
        public void HandleAuraModIncreaseMountedSpeed(AuraApplication auraApplication, AuraEffectHandleModes mode, bool apply) { }
        public void HandleAuraModIncreaseFlightSpeed(AuraApplication auraApplication, AuraEffectHandleModes mode, bool apply) { }
        public void HandleAuraModIncreaseSwimSpeed(AuraApplication auraApplication, AuraEffectHandleModes mode, bool apply) { }
        public void HandleAuraModDecreaseSpeed(AuraApplication auraApplication, AuraEffectHandleModes mode, bool apply) { }
        public void HandleAuraModUseNormalSpeed(AuraApplication auraApplication, AuraEffectHandleModes mode, bool apply) { }
        public void HandleAuraModMinimumSpeedRate(AuraApplication auraApplication, AuraEffectHandleModes mode, bool apply) { }
        //  immunity
        public void HandleModStateImmunityMask(AuraApplication auraApplication, AuraEffectHandleModes mode, bool apply) { }
        public void HandleModMechanicImmunity(AuraApplication auraApplication, AuraEffectHandleModes mode, bool apply) { }
        public void HandleAuraModEffectImmunity(AuraApplication auraApplication, AuraEffectHandleModes mode, bool apply) { }
        public void HandleAuraModStateImmunity(AuraApplication auraApplication, AuraEffectHandleModes mode, bool apply) { }
        public void HandleAuraModSchoolImmunity(AuraApplication auraApplication, AuraEffectHandleModes mode, bool apply) { }
        public void HandleAuraModDmgImmunity(AuraApplication auraApplication, AuraEffectHandleModes mode, bool apply) { }
        public void HandleAuraModDispelImmunity(AuraApplication auraApplication, AuraEffectHandleModes mode, bool apply) { }
        //  modify stats
        //   resistance
        public void HandleAuraModResistanceExclusive(AuraApplication auraApplication, AuraEffectHandleModes mode, bool apply) { }
        public void HandleAuraModResistance(AuraApplication auraApplication, AuraEffectHandleModes mode, bool apply) { }
        public void HandleAuraModBaseResistancePct(AuraApplication auraApplication, AuraEffectHandleModes mode, bool apply) { }
        public void HandleModResistancePercent(AuraApplication auraApplication, AuraEffectHandleModes mode, bool apply) { }
        public void HandleModBaseResistance(AuraApplication auraApplication, AuraEffectHandleModes mode, bool apply) { }
        public void HandleModTargetResistance(AuraApplication auraApplication, AuraEffectHandleModes mode, bool apply) { }
        //    stat
        public void HandleAuraModStat(AuraApplication auraApplication, AuraEffectHandleModes mode, bool apply) { }
        public void HandleModPercentStat(AuraApplication auraApplication, AuraEffectHandleModes mode, bool apply) { }
        public void HandleModSpellDamagePercentFromStat(AuraApplication auraApplication, AuraEffectHandleModes mode, bool apply) { }
        public void HandleModSpellHealingPercentFromStat(AuraApplication auraApplication, AuraEffectHandleModes mode, bool apply) { }
        public void HandleModSpellDamagePercentFromAttackPower(AuraApplication auraApplication, AuraEffectHandleModes mode, bool apply) { }
        public void HandleModSpellHealingPercentFromAttackPower(AuraApplication auraApplication, AuraEffectHandleModes mode, bool apply) { }
        public void HandleModHealingDone(AuraApplication auraApplication, AuraEffectHandleModes mode, bool apply) { }
        public void HandleModTotalPercentStat(AuraApplication auraApplication, AuraEffectHandleModes mode, bool apply) { }
        public void HandleAuraModResistenceOfStatPercent(AuraApplication auraApplication, AuraEffectHandleModes mode, bool apply) { }
        public void HandleAuraModExpertise(AuraApplication auraApplication, AuraEffectHandleModes mode, bool apply) { }
        public void HandleModStatBonusPercent(AuraApplication auraApplication, AuraEffectHandleModes mode, bool apply) { }
        public void HandleOverrideSpellPowerByAttackPower(AuraApplication auraApplication, AuraEffectHandleModes mode, bool apply) { }
        public void HandleOverrideAttackPowerBySpellPower(AuraApplication auraApplication, AuraEffectHandleModes mode, bool apply) { }
        //   heal and energize
        public void HandleModPowerRegen(AuraApplication auraApplication, AuraEffectHandleModes mode, bool apply) { }
        public void HandleModPowerRegenPct(AuraApplication auraApplication, AuraEffectHandleModes mode, bool apply) { }
        public void HandleModManaRegen(AuraApplication auraApplication, AuraEffectHandleModes mode, bool apply) { }
        public void HandleAuraModIncreaseHealth(AuraApplication auraApplication, AuraEffectHandleModes mode, bool apply) { }
        public void HandleAuraModIncreaseMaxHealth(AuraApplication auraApplication, AuraEffectHandleModes mode, bool apply) { }
        public void HandleAuraModIncreaseEnergy(AuraApplication auraApplication, AuraEffectHandleModes mode, bool apply) { }
        public void HandleAuraModIncreaseEnergyPercent(AuraApplication auraApplication, AuraEffectHandleModes mode, bool apply) { }
        public void HandleAuraModIncreaseHealthPercent(AuraApplication auraApplication, AuraEffectHandleModes mode, bool apply) { }
        public void HandleAuraIncreaseBaseHealthPercent(AuraApplication auraApplication, AuraEffectHandleModes mode, bool apply) { }
        public void HandleAuraModIncreaseBaseManaPercent(AuraApplication auraApplication, AuraEffectHandleModes mode, bool apply) { }
        public void HandleModPowerDisplay(AuraApplication auraApplication, AuraEffectHandleModes mode, bool apply) { }
        //   fight
        public void HandleAuraModParryPercent(AuraApplication auraApplication, AuraEffectHandleModes mode, bool apply) { }
        public void HandleAuraModDodgePercent(AuraApplication auraApplication, AuraEffectHandleModes mode, bool apply) { }
        public void HandleAuraModBlockPercent(AuraApplication auraApplication, AuraEffectHandleModes mode, bool apply) { }
        public void HandleAuraModRegenInterrupt(AuraApplication auraApplication, AuraEffectHandleModes mode, bool apply) { }
        public void HandleAuraModWeaponCritPercent(AuraApplication auraApplication, AuraEffectHandleModes mode, bool apply) { }
        public void HandleModHitChance(AuraApplication auraApplication, AuraEffectHandleModes mode, bool apply) { }
        public void HandleModSpellHitChance(AuraApplication auraApplication, AuraEffectHandleModes mode, bool apply) { }
        public void HandleModSpellCritChance(AuraApplication auraApplication, AuraEffectHandleModes mode, bool apply) { }
        public void HandleAuraModCritPct(AuraApplication auraApplication, AuraEffectHandleModes mode, bool apply) { }
        //   attack speed
        public void HandleModCastingSpeed(AuraApplication auraApplication, AuraEffectHandleModes mode, bool apply) { }
        public void HandleModMeleeRangedSpeedPct(AuraApplication auraApplication, AuraEffectHandleModes mode, bool apply) { }
        public void HandleModCombatSpeedPct(AuraApplication auraApplication, AuraEffectHandleModes mode, bool apply) { }
        public void HandleModAttackSpeed(AuraApplication auraApplication, AuraEffectHandleModes mode, bool apply) { }
        public void HandleModMeleeSpeedPct(AuraApplication auraApplication, AuraEffectHandleModes mode, bool apply) { }
        public void HandleAuraModRangedHaste(AuraApplication auraApplication, AuraEffectHandleModes mode, bool apply) { }
        //   combat rating
        public void HandleModRating(AuraApplication auraApplication, AuraEffectHandleModes mode, bool apply) { }
        public void HandleModRatingFromStat(AuraApplication auraApplication, AuraEffectHandleModes mode, bool apply) { }
        public void HandleModRatingPct(AuraApplication auraApplication, AuraEffectHandleModes mode, bool apply) { }
        //   attack power
        public void HandleAuraModAttackPower(AuraApplication auraApplication, AuraEffectHandleModes mode, bool apply) { }
        public void HandleAuraModRangedAttackPower(AuraApplication auraApplication, AuraEffectHandleModes mode, bool apply) { }
        public void HandleAuraModAttackPowerPercent(AuraApplication auraApplication, AuraEffectHandleModes mode, bool apply) { }
        public void HandleAuraModRangedAttackPowerPercent(AuraApplication auraApplication, AuraEffectHandleModes mode, bool apply) { }
        public void HandleAuraModAttackPowerOfArmor(AuraApplication auraApplication, AuraEffectHandleModes mode, bool apply) { }
        //   damage bonus
        public void HandleModDamageDone(AuraApplication auraApplication, AuraEffectHandleModes mode, bool apply) { }
        public void HandleModDamagePercentDone(AuraApplication auraApplication, AuraEffectHandleModes mode, bool apply) { }
        public void HandleModOffhandDamagePercent(AuraApplication auraApplication, AuraEffectHandleModes mode, bool apply) { }
        public void HandleShieldBlockValue(AuraApplication auraApplication, AuraEffectHandleModes mode, bool apply) { }
        //  power cost
        public void HandleModPowerCostPct(AuraApplication auraApplication, AuraEffectHandleModes mode, bool apply) { }
        public void HandleModPowerCost(AuraApplication auraApplication, AuraEffectHandleModes mode, bool apply) { }
        public void HandleArenaPreparation(AuraApplication auraApplication, AuraEffectHandleModes mode, bool apply) { }
        public void HandleNoReagentUseAura(AuraApplication auraApplication, AuraEffectHandleModes mode, bool apply) { }
        public void HandleAuraRetainComboPoints(AuraApplication auraApplication, AuraEffectHandleModes mode, bool apply) { }
        //  others
        public void HandleAuraDummy(AuraApplication auraApplication, AuraEffectHandleModes mode, bool apply) { }
        public void HandleChannelDeathItem(AuraApplication auraApplication, AuraEffectHandleModes mode, bool apply) { }
        public void HandleBindSight(AuraApplication auraApplication, AuraEffectHandleModes mode, bool apply) { }
        public void HandleForceReaction(AuraApplication auraApplication, AuraEffectHandleModes mode, bool apply) { }
        public void HandleAuraEmpathy(AuraApplication auraApplication, AuraEffectHandleModes mode, bool apply) { }
        public void HandleAuraModFaction(AuraApplication auraApplication, AuraEffectHandleModes mode, bool apply) { }
        public void HandleComprehendLanguage(AuraApplication auraApplication, AuraEffectHandleModes mode, bool apply) { }
        public void HandleAuraLinked(AuraApplication auraApplication, AuraEffectHandleModes mode, bool apply) { }
        public void HandleAuraOpenStable(AuraApplication auraApplication, AuraEffectHandleModes mode, bool apply) { }
        public void HandleAuraModFakeInebriation(AuraApplication auraApplication, AuraEffectHandleModes mode, bool apply) { }
        public void HandleAuraOverrideSpells(AuraApplication auraApplication, AuraEffectHandleModes mode, bool apply) { }
        public void HandleAuraSetVehicle(AuraApplication auraApplication, AuraEffectHandleModes mode, bool apply) { }
        public void HandlePreventResurrection(AuraApplication auraApplication, AuraEffectHandleModes mode, bool apply) { }
        public void HandleMastery(AuraApplication auraApplication, AuraEffectHandleModes mode, bool apply) { }
        public void HandleAuraForceWeather(AuraApplication auraApplication, AuraEffectHandleModes mode, bool apply) { }
        public void HandleEnableAltPower(AuraApplication auraApplication, AuraEffectHandleModes mode, bool apply) { }
        public void HandleModSpellCategoryCooldown(AuraApplication auraApplication, AuraEffectHandleModes mode, bool apply) { }
        public void HandleShowConfirmationPrompt(AuraApplication auraApplication, AuraEffectHandleModes mode, bool apply) { }
        public void HandleOverridePetSpecs(AuraApplication auraApplication, AuraEffectHandleModes mode, bool apply) { }
        public void HandleAllowUsingGameobjectsWhileMounted(AuraApplication auraApplication, AuraEffectHandleModes mode, bool apply) { }
        public void HandlePlayScene(AuraApplication auraApplication, AuraEffectHandleModes mode, bool apply) { }

        // aura effect periodic tick handlers
        public void HandlePeriodicDummyAuraTick(Unit target, Unit caster) { }
        public void HandlePeriodicTriggerSpellAuraTick(Unit target, Unit caster) { }
        public void HandlePeriodicTriggerSpellWithValueAuraTick(Unit target, Unit caster) { }
        public void HandlePeriodicDamageAurasTick(Unit target, Unit caster) { }
        public void HandlePeriodicHealthLeechAuraTick(Unit target, Unit caster) { }
        public void HandlePeriodicHealthFunnelAuraTick(Unit target, Unit caster) { }
        public void HandlePeriodicHealAurasTick(Unit target, Unit caster) { }
        public void HandlePeriodicManaLeechAuraTick(Unit target, Unit caster) { }
        public void HandleObsModPowerAuraTick(Unit target, Unit caster) { }
        public void HandlePeriodicEnergizeAuraTick(Unit target, Unit caster) { }
        public void HandlePeriodicPowerBurnAuraTick(Unit target, Unit caster) { }

        // aura effect proc handlers
        public void HandleProcTriggerSpellAuraProc(AuraApplication aurApp, ProcEventInfo eventInfo) { }
        public void HandleProcTriggerSpellWithValueAuraProc(AuraApplication aurApp, ProcEventInfo eventInfo) { }
        public void HandleProcTriggerDamageAuraProc(AuraApplication aurApp, ProcEventInfo eventInfo) { }
        public void HandleRaidProcFromChargeAuraProc(AuraApplication aurApp, ProcEventInfo eventInfo) { }
        public void HandleRaidProcFromChargeWithValueAuraProc(AuraApplication aurApp, ProcEventInfo eventInfo) { }
        public void HandleProcTriggerSpellOnPowerAmountAuraProc(AuraApplication aurApp, ProcEventInfo eventInfo) { }

        #endregion
    }
}