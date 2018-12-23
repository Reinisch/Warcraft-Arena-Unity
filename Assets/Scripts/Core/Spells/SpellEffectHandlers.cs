using System;
using UnityEngine;
using UnityEngine.Assertions;

namespace Core
{
    public partial class Spell
    {
        public void EffectNone(SpellEffectInfo effect)
        {
            Debug.Log("Spells: Handled EffectNone!");
        }

        public void EffectUnused(SpellEffectInfo effect)
        {
            Debug.Log("Spells: Handled EffectUnused!");
        }

        public void EffectDistract(EffectTeleportDirect effect, Unit target, SpellEffectHandleMode mode)
        {
            if (mode != SpellEffectHandleMode.HitTarget)
                return;

            // Check for possible target
            if (target == null || target.IsInCombat())
                return;

            // target must be OK to do this
            if (target.HasUnitState(UnitState.Confused | UnitState.Stunned | UnitState.Fleeing))
                return;

            target.SetFacingTo(DestTarget);
            target.ClearUnitState(UnitState.Moving);
        }

        public void EffectPull(SpellEffectInfo effect)
        {
            EffectNone(effect);
        }

        public void EffectEnvironmentalDMG(EffectTeleportDirect effect, Unit target, SpellEffectHandleMode mode)
        {
            if (mode != SpellEffectHandleMode.HitTarget)
                return;

            if (target == null || !target.IsAlive)
                return;

            int absorb = 0;
            int resist = 0;
            Caster.CalcAbsorbResist(target, SpellInfo.SchoolMask, DamageEffectType.DirectDamage, SpellDamage, ref absorb, ref resist, SpellInfo);

            if (target.EntityType == EntityType.Player)
                ((Player)target).EnvironmentalDamage(EnviromentalDamage.Fire, SpellDamage);

            var log = new SpellNonMeleeDamage(Caster, target, SpellInfo.Id, SpellInfo.SchoolMask, CastId)
            {
                Damage = SpellDamage - absorb - resist,
                Absorb = absorb,
                Resist = resist
            };
            Caster.SendSpellNonMeleeDamageLog(ref log);
        }

        public void EffectInstaKill(EffectTeleportDirect effect, Unit target, SpellEffectHandleMode mode)
        {
            if (mode != SpellEffectHandleMode.HitTarget)
                return;

            if (target == null || !target.IsAlive)
                return;

            if (Caster == target)
                Finish();

            Caster.DealDamage(target, target.GetHealth(), null, DamageEffectType.NoDamage, SpellSchoolMask.Normal, null, false);
        }

        public void EffectDummy(int effIndex) { throw new NotImplementedException(); }

        public void EffectTeleportUnits(int effIndex) { throw new NotImplementedException(); }

        public void EffectApplyAura(EffectTeleportDirect effect, Unit target, SpellEffectHandleMode mode)
        {
            if (mode != SpellEffectHandleMode.HitTarget)
                return;

            if (SpellAura == null || target == null)
                return;

            Assert.IsTrue(target == SpellAura.Owner);
            SpellAura.ApplyEffectForTargets(effect);
        }

        public void EffectSendEvent(int effIndex) { throw new NotImplementedException(); }

        public void EffectPowerBurn(int effIndex) { throw new NotImplementedException(); }

        public void EffectPowerDrain(int effIndex) { throw new NotImplementedException(); }

        public void EffectHeal(int effIndex) { throw new NotImplementedException(); }

        public void EffectBind(int effIndex) { throw new NotImplementedException(); }

        public void EffectHealthLeech(int effIndex) { throw new NotImplementedException(); }

        public void EffectQuestComplete(int effIndex) { throw new NotImplementedException(); }

        public void EffectCreateItem(int effIndex) { throw new NotImplementedException(); }

        public void EffectCreateItem2(int effIndex) { throw new NotImplementedException(); }

        public void EffectCreateRandomItem(int effIndex) { throw new NotImplementedException(); }

        public void EffectPersistentAA(int effIndex) { throw new NotImplementedException(); }

        public void EffectEnergize(int effIndex) { throw new NotImplementedException(); }

        public void EffectOpenLock(int effIndex) { throw new NotImplementedException(); }

        public void EffectSummonChangeItem(int effIndex) { throw new NotImplementedException(); }

        public void EffectProficiency(int effIndex) { throw new NotImplementedException(); }

        public void EffectApplyAreaAura(int effIndex) { throw new NotImplementedException(); }

        public void EffectSummonType(int effIndex) { throw new NotImplementedException(); }

        public void EffectLearnSpell(int effIndex) { throw new NotImplementedException(); }

        public void EffectDispel(int effIndex) { throw new NotImplementedException(); }

        public void EffectDualWield(int effIndex) { throw new NotImplementedException(); }

        public void EffectPickPocket(int effIndex) { throw new NotImplementedException(); }

        public void EffectAddFarsight(int effIndex) { throw new NotImplementedException(); }

        public void EffectUntrainTalents(int effIndex) { throw new NotImplementedException(); }

        public void EffectHealMechanical(int effIndex) { throw new NotImplementedException(); }

        public void EffectJump(int effIndex) { throw new NotImplementedException(); }

        public void EffectJumpDest(int effIndex) { throw new NotImplementedException(); }

        public void EffectLeapBack(int effIndex) { throw new NotImplementedException(); }

        public void EffectQuestClear(int effIndex) { throw new NotImplementedException(); }

        public void EffectTeleUnitsFaceCaster(int effIndex) { throw new NotImplementedException(); }

        public void EffectLearnSkill(int effIndex) { throw new NotImplementedException(); }

        public void EffectPlayMovie(int effIndex) { throw new NotImplementedException(); }

        public void EffectTradeSkill(int effIndex) { throw new NotImplementedException(); }

        public void EffectEnchantItemPerm(int effIndex) { throw new NotImplementedException(); }

        public void EffectEnchantItemTmp(int effIndex) { throw new NotImplementedException(); }

        public void EffectTameCreature(int effIndex) { throw new NotImplementedException(); }

        public void EffectSummonPet(int effIndex) { throw new NotImplementedException(); }

        public void EffectLearnPetSpell(int effIndex) { throw new NotImplementedException(); }

        public void EffectWeaponDmg(int effIndex) { throw new NotImplementedException(); }

        public void EffectForceCast(int effIndex) { throw new NotImplementedException(); }

        public void EffectTriggerSpell(int effIndex) { throw new NotImplementedException(); }

        public void EffectTriggerMissileSpell(int effIndex) { throw new NotImplementedException(); }

        public void EffectThreat(int effIndex) { throw new NotImplementedException(); }

        public void EffectHealMaxHealth(int effIndex) { throw new NotImplementedException(); }

        public void EffectInterruptCast(int effIndex) { throw new NotImplementedException(); }

        public void EffectSummonObjectWild(int effIndex) { throw new NotImplementedException(); }

        public void EffectScriptEffect(int effIndex) { throw new NotImplementedException(); }

        public void EffectSanctuary(int effIndex) { throw new NotImplementedException(); }

        public void EffectAddComboPoints(int effIndex) { throw new NotImplementedException(); }

        public void EffectDuel(int effIndex) { throw new NotImplementedException(); }

        public void EffectStuck(int effIndex) { throw new NotImplementedException(); }

        public void EffectSummonPlayer(int effIndex) { throw new NotImplementedException(); }

        public void EffectActivateObject(int effIndex) { throw new NotImplementedException(); }

        public void EffectApplyGlyph(int effIndex) { throw new NotImplementedException(); }

        public void EffectEnchantHeldItem(int effIndex) { throw new NotImplementedException(); }

        public void EffectSummonObject(int effIndex) { throw new NotImplementedException(); }

        public void EffectChangeRaidMarker(int effIndex) { throw new NotImplementedException(); }

        public void EffectResurrect(int effIndex) { throw new NotImplementedException(); }

        public void EffectParry(int effIndex) { throw new NotImplementedException(); }

        public void EffectBlock(int effIndex) { throw new NotImplementedException(); }

        public void EffectLeap(int effIndex) { throw new NotImplementedException(); }

        public void EffectTransmitted(int effIndex) { throw new NotImplementedException(); }

        public void EffectDisEnchant(int effIndex) { throw new NotImplementedException(); }

        public void EffectInebriate(int effIndex) { throw new NotImplementedException(); }

        public void EffectFeedPet(int effIndex) { throw new NotImplementedException(); }

        public void EffectDismissPet(int effIndex) { throw new NotImplementedException(); }

        public void EffectReputation(int effIndex) { throw new NotImplementedException(); }

        public void EffectForceDeselect(int effIndex) { throw new NotImplementedException(); }

        public void EffectSelfResurrect(int effIndex) { throw new NotImplementedException(); }

        public void EffectSkinning(int effIndex) { throw new NotImplementedException(); }

        public void EffectCharge(int effIndex) { throw new NotImplementedException(); }

        public void EffectChargeDest(int effIndex) { throw new NotImplementedException(); }

        public void EffectProspecting(int effIndex) { throw new NotImplementedException(); }

        public void EffectMilling(int effIndex) { throw new NotImplementedException(); }

        public void EffectRenamePet(int effIndex) { throw new NotImplementedException(); }

        public void EffectSendTaxi(int effIndex) { throw new NotImplementedException(); }

        public void EffectKnockBack(int effIndex) { throw new NotImplementedException(); }

        public void EffectPullTowards(int effIndex) { throw new NotImplementedException(); }

        public void EffectDispelMechanic(int effIndex) { throw new NotImplementedException(); }

        public void EffectResurrectPet(int effIndex) { throw new NotImplementedException(); }

        public void EffectDestroyAllTotems(int effIndex) { throw new NotImplementedException(); }

        public void EffectDurabilityDamage(int effIndex) { throw new NotImplementedException(); }

        public void EffectSkill(int effIndex) { throw new NotImplementedException(); }

        public void EffectTaunt(int effIndex) { throw new NotImplementedException(); }

        public void EffectDurabilityDamagePercent(int effIndex) { throw new NotImplementedException(); }

        public void EffectModifyThreatPercent(int effIndex) { throw new NotImplementedException(); }

        public void EffectResurrectNew(int effIndex) { throw new NotImplementedException(); }

        public void EffectAddExtraAttacks(int effIndex) { throw new NotImplementedException(); }

        public void EffectSpiritHeal(int effIndex) { throw new NotImplementedException(); }

        public void EffectSkinPlayerCorpse(int effIndex) { throw new NotImplementedException(); }

        public void EffectStealBeneficialBuff(int effIndex) { throw new NotImplementedException(); }

        public void EffectUnlearnSpecialization(int effIndex) { throw new NotImplementedException(); }

        public void EffectHealPercent(int effIndex) { throw new NotImplementedException(); }

        public void EffectEnergizePercent(int effIndex) { throw new NotImplementedException(); }

        public void EffectTriggerRitualOfSummoning(int effIndex) { throw new NotImplementedException(); }

        public void EffectSummonRaFFriend(int effIndex) { throw new NotImplementedException(); }

        public void EffectUnlockGuildVaultTab(int effIndex) { throw new NotImplementedException(); }

        public void EffectKillCreditPersonal(int effIndex) { throw new NotImplementedException(); }

        public void EffectKillCredit(int effIndex) { throw new NotImplementedException(); }

        public void EffectQuestFail(int effIndex) { throw new NotImplementedException(); }

        public void EffectQuestStart(int effIndex) { throw new NotImplementedException(); }

        public void EffectRedirectThreat(int effIndex) { throw new NotImplementedException(); }

        public void EffectGameObjectDamage(int effIndex) { throw new NotImplementedException(); }

        public void EffectGameObjectRepair(int effIndex) { throw new NotImplementedException(); }

        public void EffectGameObjectSetDestructionState(int effIndex) { throw new NotImplementedException(); }

        public void EffectActivateRune(int effIndex) { throw new NotImplementedException(); }

        public void EffectCreateTamedPet(int effIndex) { throw new NotImplementedException(); }

        public void EffectDiscoverTaxi(int effIndex) { throw new NotImplementedException(); }

        public void EffectTitanGrip(int effIndex) { throw new NotImplementedException(); }

        public void EffectEnchantItemPrismatic(int effIndex) { throw new NotImplementedException(); }

        public void EffectPlayMusic(int effIndex) { throw new NotImplementedException(); }

        public void EffectActivateSpec(int effIndex) { throw new NotImplementedException(); }

        public void EffectPlaySound(int effIndex) { throw new NotImplementedException(); }

        public void EffectRemoveAura(int effIndex) { throw new NotImplementedException(); }

        public void EffectDamageFromMaxHealthPercent(int effIndex) { throw new NotImplementedException(); }

        public void EffectCastButtons(int effIndex) { throw new NotImplementedException(); }

        public void EffectRechargeManaGem(int effIndex) { throw new NotImplementedException(); }

        public void EffectGiveCurrency(int effIndex) { throw new NotImplementedException(); }

        public void EffectResurrectWithAura(int effIndex) { throw new NotImplementedException(); }

        public void EffectCreateAreaTrigger(int effIndex) { throw new NotImplementedException(); }

        public void EffectRemoveTalent(int effIndex) { throw new NotImplementedException(); }

        public void EffectDestroyItem(int effIndex) { throw new NotImplementedException(); }

        public void EffectLearnGarrisonBuilding(int effIndex) { throw new NotImplementedException(); }

        public void EffectCreateGarrison(int effIndex) { throw new NotImplementedException(); }

        public void EffectAddGarrisonFollower(int effIndex) { throw new NotImplementedException(); }

        public void EffectActivateGarrisonBuilding(int effIndex) { throw new NotImplementedException(); }

        public void EffectHealBattlePetPct(int effIndex) { throw new NotImplementedException(); }

        public void EffectEnableBattlePets(int effIndex) { throw new NotImplementedException(); }

        public void EffectUncageBattlePet(int effIndex) { throw new NotImplementedException(); }

        public void EffectCreateHeirloomItem(int effIndex) { throw new NotImplementedException(); }

        public void EffectUpgradeHeirloom(int effIndex) { throw new NotImplementedException(); }

        public void EffectApplyEnchantIllusion(int effIndex) { throw new NotImplementedException(); }

        public void EffectUpdatePlayerPhase(int effIndex) { throw new NotImplementedException(); }

        public void EffectUpdateZoneAurasAndPhases(int effIndex) { throw new NotImplementedException(); }

        public void EffectGiveArtifactPower(int effIndex) { throw new NotImplementedException(); }

        public void EffectGiveArtifactPowerNoBonus(int effIndex) { throw new NotImplementedException(); }
    }
}
