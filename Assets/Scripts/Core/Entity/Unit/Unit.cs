using System;
using System.Collections.Generic;
using System.Linq;
using Common;
using JetBrains.Annotations;
using UdpKit;
using UnityEngine;

using Assert = Common.Assert;
using EventHandler = Common.EventHandler;
using Debug = UnityEngine.Debug;
using Mathf = UnityEngine.Mathf;

namespace Core
{
    public abstract class Unit : WorldEntity
    {
        public new class CreateToken : WorldEntity.CreateToken
        {
            public DeathState DeathState;
            
            public override void Read(UdpPacket packet)
            {
                base.Read(packet);

                DeathState = (DeathState)packet.ReadInt();
            }

            public override void Write(UdpPacket packet)
            {
                base.Write(packet);

                packet.WriteInt((int)DeathState);
            }

            public void Attached(Unit unit)
            {
                unit.deathState = DeathState;
            }
        }

        [SerializeField, UsedImplicitly, Header("Unit"), Space(10)]
        private CapsuleCollider unitCollider;
        [SerializeField, UsedImplicitly]
        private UnitMovementDefinition unitMovementDefinition;

        private IUnitState unitState;
        private DeathState deathState;
        private CreateToken createToken;

        private readonly Dictionary<int, List<Aura>> ownedAuras = new Dictionary<int, List<Aura>>();
        private readonly Dictionary<StatType, float> createStats = new Dictionary<StatType, float>();
        private readonly Dictionary<UnitMoveType, float> speedRates = new Dictionary<UnitMoveType, float>();
        private readonly List<AuraApplication> visibleAuras = new List<AuraApplication>();

        private ThreatManager ThreatManager { get; set; }
        private UnitState UnitState { get; set; }
        protected UnitAI AI { get; private set; }

        public SpellHistory SpellHistory { get; } = new SpellHistory();
        public CapsuleCollider UnitCollider => unitCollider;

        public override EntityType EntityType => EntityType.Unit;
        public int Health => GetIntValue(EntityFields.Health);
        public int MaxHealth => GetIntValue(EntityFields.MaxHealth);
        public uint BaseMana => GetUintValue(EntityFields.BaseMana);
        public ulong Target => GetULongValue(EntityFields.Target);
        public float HealthRatio => (float) GetLongValue(EntityFields.Health) / GetLongValue(EntityFields.MaxHealth);

        public bool IsMovementBlocked => HasUnitState(UnitState.Root) || HasUnitState(UnitState.Stunned);
        public bool IsAlive => deathState == DeathState.Alive;
        public bool IsDead => deathState == DeathState.Dead || deathState == DeathState.Corpse;
        public bool IsInCombat => HasFlag(EntityFields.UnitFlags, (int)UnitFlags.InCombat);
        public bool IsControlledByPlayer => this is Player;

        public bool IsHostileTo(Unit unit) => GetUintValue(EntityFields.FactionTemplate) != unit.GetUintValue(EntityFields.FactionTemplate);
        public bool IsFriendlyTo(Unit unit) => GetUintValue(EntityFields.FactionTemplate) == unit.GetUintValue(EntityFields.FactionTemplate);
        
        public override void Attached()
        {
            unitState = entity.GetState<IUnitState>();

            base.Attached();

            foreach (UnitMoveType moveType in StatUtils.UnitMoveTypes)
                speedRates[moveType] = 1.0f;

            createToken = (CreateToken)entity.attachToken;
            createToken.Attached(this);

            if (!IsOwner)
            {
                unitState.AddCallback(nameof(unitState.DeathState), OnDeathStateChanged);
                unitState.AddCallback(nameof(unitState.Health), OnHealthStateChanged);
            }

            ThreatManager = new ThreatManager(this);
            MovementInfo.Attached(unitState, this);
            WorldManager.UnitManager.Attach(this);

            SetMap(WorldManager.FindMap(1));
        }

        public override void Detached()
        {
            // for instant manual client detach without waiting for Photon
            if (!IsValid)
                return;

            unitState.RemoveAllCallbacks();

            ResetMap();

            ThreatManager.Dispose();
            WorldManager.UnitManager.Detach(this);
            MovementInfo.Detached();

            unitState = null;
            createToken = null;

            base.Detached();
        }

        public float GetSpellMinRangeForTarget(Unit target, SpellInfo spellInfo)
        {
            if (Mathf.Approximately(spellInfo.MinRangeFriend, spellInfo.MinRangeHostile))
                return spellInfo.GetMinRange(false);
            if (target == null)
                return spellInfo.GetMinRange(true);
            return spellInfo.GetMinRange(!IsHostileTo(target));
        }

        public float GetSpellMaxRangeForTarget(Unit target, SpellInfo spellInfo)
        {
            if (Mathf.Approximately(spellInfo.MaxRangeFriend, spellInfo.MaxRangeHostile))
                return spellInfo.GetMaxRange(false);
            if (target == null)
                return spellInfo.GetMaxRange(true);
            return spellInfo.GetMaxRange(!IsHostileTo(target));
        }

        #region Unit info, statType and types

        public void AddUnitState(UnitState f) { UnitState |= f; }
        public bool HasUnitState(UnitState state) { return (UnitState & state) != 0; }
        public void ClearUnitState(UnitState f) { UnitState &= ~f; }
        public uint GetCreatureTypeMask() { return 0; }

        public byte GetLevel() { return (byte)GetUintValue(EntityFields.Level); }
        public byte GetLevelForTarget() { return GetLevel(); }
        public void SetLevel(byte lvl) { }

        public float GetStat(StatType statType) { return GetUintValue(statType.StatField()); }
        public void SetStat(StatType statType, int val) { SetStatIntValue(statType.StatField(), val); }
        public uint GetArmor() { return GetResistance(SpellSchools.Normal); }
        public void SetArmor(int val) { SetResistance(SpellSchools.Normal, val); }

        public uint GetResistance(SpellSchools school) { return GetUintValue(school.ResistanceField()); }
        public uint GetResistance(SpellSchoolMask mask) { return 0; }
        public void SetResistance(SpellSchools school, int val) { SetStatIntValue(school.ResistanceField(), val); }

        #endregion

        #region Health and powers

        public bool IsFullHealth() { return Health== MaxHealth; }
        public bool HealthBelowPct(int pct) { return Health< CountPctFromMaxHealth(pct); }
        public bool HealthBelowPctDamaged(int pct, uint damage) { return Health- damage < CountPctFromMaxHealth(pct); }
        public bool HealthAbovePct(int pct) { return Health> CountPctFromMaxHealth(pct); }
        public bool HealthAbovePctHealed(int pct, uint heal) { return Health+ heal > CountPctFromMaxHealth(pct); }
        public float GetHealthPct() { return MaxHealth> 0 ? 100.0f * Health/ MaxHealth: 0.0f; }
        public long CountPctFromMaxHealth(int pct) { return MaxHealth.CalculatePercentage(pct); }
        public long CountPctFromCurHealth(int pct) { return Health.CalculatePercentage(pct); }

        /// <summary> Modifies health, returns effective delta. </summary>
        internal int ModifyHealth(int delta)
        {
            return SetHealth(Health + delta);
        }

        /// <summary> Sets health, returns effective delta. </summary>
        internal int SetHealth(int value)
        {
            int oldHealth = Health;
            int newHealth = value;
            int maxHealth = MaxHealth;

            if (newHealth < 0)
                newHealth = 0;
            if (newHealth > maxHealth)
                newHealth = maxHealth;

            SetIntValue(EntityFields.Health, newHealth);
            unitState.Health = newHealth;

            return newHealth - oldHealth;
        }

        public SpellResourceType GetPowerType() { return (SpellResourceType)GetIntValue(EntityFields.DisplayPower); }
        public void SetPowerType(SpellResourceType spellResource) { }
        public int GetPower(SpellResourceType spellResource) { return GetIntValue(EntityFields.Power); }
        public int GetMinPower(SpellResourceType spellResource) { return spellResource == SpellResourceType.LunarPower ? -100 : 0; }
        public int GetMaxPower(SpellResourceType spellResource) { return GetIntValue(EntityFields.MaxPower); }
        public float GetPowerPct(SpellResourceType spellResource) { return GetMaxPower(spellResource) > 0 ? 100.0f * GetPower(spellResource) / GetMaxPower(spellResource) : 0.0f; }
        public int CountPctFromMaxPower(SpellResourceType spellResource, int pct) { return GetMaxPower(spellResource).CalculatePercentage(pct); }
        public void SetPower(SpellResourceType spellResource, int val) { }
        public void SetMaxPower(SpellResourceType spellResource, int val) { }
        public int ModifyPower(SpellResourceType spellResource, int val) { return 0; }
        public int ModifyPowerPct(SpellResourceType spellResource, float pct, bool apply = true) { return 0; }

        #endregion

        #region Spell Casting and Hit Calculations

        internal SpellCastResult CastSpell(SpellCastTargets targets, SpellInfo spellInfo, SpellCastFlags spellFlags = 0, AuraEffect triggeredByAura = null, ulong originalCaster = 0)
        {
            return new Spell(this, spellInfo, spellFlags, originalCaster).Prepare(targets, triggeredByAura);
        }

        internal int DealSpellDamage(SpellCastDamageInfo damageInfoInfo)
        {
            if (damageInfoInfo == null)
                return 0;

            Unit victim = damageInfoInfo.Target;

            if (victim == null)
                return 0;

            if (!victim.IsAlive)
                return 0;

            SpellInfo spellProto = Balance.SpellInfosById.ContainsKey(damageInfoInfo.SpellId) ? Balance.SpellInfosById[damageInfoInfo.SpellId] : null;
            if (spellProto == null)
            {
                Debug.LogErrorFormat("Unit.DealSpellDamage has wrong spellDamageInfo->SpellID: {0}", damageInfoInfo.SpellId);
                return 0;
            }

            EventHandler.ExecuteEvent(EventHandler.GlobalDispatcher, GameEvents.SpellDamageDone, this, victim, damageInfoInfo.Damage, damageInfoInfo.HitInfo == HitType.CriticalHit);

            return DealDamage(victim, damageInfoInfo.Damage);
        }

        internal int HealBySpell(Unit victim, SpellInfo spellInfo, int addHealth, bool critical = false) { return 0; }

        internal int DealDamage(Unit victim, int damageAmount)
        {
            if (damageAmount < 1)
                return 0;

            int health = victim.Health;
            if (health <= damageAmount)
            {
                Kill(victim);
                return health;
            }

            return victim.ModifyHealth(-damageAmount);
        }

        internal int DealHeal(Unit victim, int healAmount)
        {
            if(healAmount < 1)
                return 0;

            return victim.ModifyHealth(healAmount);
        }

        internal void Kill(Unit victim)
        {
            if (victim.Health <= 0)
                return;

            victim.SetHealth(0);
            victim.ModifyDeathState(DeathState.Dead);
        }

        internal void ModifyDeathState(DeathState newState)
        {
            if (deathState == newState)
                return;

            unitState.DeathState = (int)(createToken.DeathState = deathState = newState);
        }

        internal int CalculateSpellDamageTaken(SpellCastDamageInfo damageInfoInfo, int damage, SpellInfo spellInfo)
        {
            if (damage < 0)
                return 0;

            Unit victim = damageInfoInfo.Target;
            if (victim == null || !victim.IsAlive)
                return 0;

            SpellSchoolMask damageSchoolMask = damageInfoInfo.SchoolMask;

            if (damage > 0)
            {
                int absorb = damageInfoInfo.Absorb;
                int resist = damageInfoInfo.Resist;
                CalcAbsorbResist(victim, damageSchoolMask, SpellDamageType.Direct, damage, ref absorb, ref resist, spellInfo);
                damageInfoInfo.Absorb = absorb;
                damageInfoInfo.Resist = resist;
                damage -= damageInfoInfo.Absorb + damageInfoInfo.Resist;
            }
            else
                damage = 0;

            return damageInfoInfo.Damage = damage;
        }
        
        internal SpellMissType SpellHitResult(Unit victim, SpellInfo spellInfo, bool canReflect = false)
        {
            // Check for immune
            /*if (victim->IsImmunedToSpell(spellInfo))
                return SPELL_MISS_IMMUNE;*/

            // All positive spells can`t miss
            if (spellInfo.IsPositive() && !IsHostileTo(victim)) // prevent from affecting enemy by "positive" spell
                return SpellMissType.None;

            // Check for immune
            /*if (victim->IsImmunedToDamage(spellInfo))
                return SPELL_MISS_IMMUNE;*/

            if (this == victim)
                return SpellMissType.None;

            // Try victim reflect spell
            /*if (CanReflect)
            {
                int32 reflectchance = victim->GetTotalAuraModifier(SPELL_AURA_REFLECT_SPELLS);
                    Unit::AuraEffectList const& mReflectSpellsSchool = victim->GetAuraEffectsByType(SPELL_AURA_REFLECT_SPELLS_SCHOOL);
                for (Unit::AuraEffectList::const_iterator i = mReflectSpellsSchool.begin(); i != mReflectSpellsSchool.end(); ++i)
                    if ((*i)->GetMiscValue() & spellInfo->GetSchoolMask())
                        reflectchance += (*i)->GetAmount();
                if (reflectchance > 0 && roll_chance_i(reflectchance))
                {
                    // Start triggers for remove charges if need (trigger only for victim, and mark as active spell)
                    ProcDamageAndSpell(victim, PROC_FLAG_NONE, PROC_FLAG_TAKEN_SPELL_MAGIC_DMG_CLASS_NEG, PROC_EX_REFLECT, 1, BASE_ATTACK, spellInfo);
                    return SPELL_MISS_REFLECT;
                }
            }*/

            /*switch (spellInfo->DmgClass)
            {
                case SPELL_DAMAGE_CLASS_RANGED:
                case SPELL_DAMAGE_CLASS_MELEE:
                    return MeleeSpellHitResult(victim, spellInfo);
                case SPELL_DAMAGE_CLASS_NONE:
                    return SPELL_MISS_NONE;
                case SPELL_DAMAGE_CLASS_MAGIC:
                    return MagicSpellHitResult(victim, spellInfo);
            }*/
            return SpellMissType.None;
        }

        #endregion

        #region Aura and target states

        public bool HasAuraTypeWithFamilyFlags(AuraType auraType, uint familyName, uint familyFlags) { return false; }
        public virtual bool HasSpell(uint spellId) { return false; }
        public bool HasBreakableByDamageAuraType(AuraType type, uint excludeAura = 0) { return true; }
        public bool HasBreakableByDamageCrowdControlAura(Unit excludeCasterChannel = null) { return true; }

        public bool HasStealthAura() { return HasAuraType(AuraType.ModStealth); }
        public bool HasInvisibilityAura() { return HasAuraType(AuraType.ModInvisibility); }
        public bool IsFeared() { return HasAuraType(AuraType.ModFear); }
        public bool IsPolymorphed() { return false; }

        public bool IsFrozen() { return false; }

        public bool IsTargetableForAttack(bool checkFakeDeath = true) { return false; }

        public bool IsValidAttackTarget(Unit target) { return false; }
        public bool IsValidAttackTarget(Unit target, SpellInfo bySpell, WorldEntity entity = null) { return false; }

        public bool IsValidAssistTarget(Unit target) { return false; }
        public bool IsValidAssistTarget(Unit target, SpellInfo bySpell) { return false; }

        public bool IsInAccessiblePlaceFor(Creature creature) { return false; }

        #endregion

        #region Aura helpers

        // aura apply/remove helpers - you should better not use these
        public Aura TryStackingOrRefreshingExistingAura(SpellInfo newAuraSpellInfo, ulong originalCasterId, ulong targetCasterId, List<int> baseAmount = null)
        {
            Assert.IsTrue(originalCasterId != 0 || targetCasterId != 0);

            // check if these can stack anyway
            if (originalCasterId == 0 && !newAuraSpellInfo.IsStackableOnOneSlotWithDifferentCasters())
                originalCasterId = targetCasterId;

            // find current aura from spell and change it's stackamount, or refresh it's duration
            var foundAura = GetOwnedAura(newAuraSpellInfo.Id, originalCasterId, 0);
            if (foundAura == null)
                return null;

            // update basepoints with new values - effect amount will be recalculated in ModStackAmount
            foreach (var spellEffectInfo in foundAura.GetSpellEffectInfos())
            {
                if (spellEffectInfo == null)
                    continue;

                AuraEffect auraEffect = foundAura.GetEffect(spellEffectInfo.Index);
                if (auraEffect == null)
                    continue;

                int newBasePoints = baseAmount?[spellEffectInfo.Index] ?? spellEffectInfo.BasePoints;
                foundAura.GetEffect(spellEffectInfo.Index).UpdateBaseAmount(newBasePoints);
            }

            // try to increase stack amount
            foundAura.ModStackAmount(1);
            return foundAura;
        }
        public void AddAura(UnitAura aura, Unit caster)
        {
            if (!ownedAuras.ContainsKey(aura.SpellInfo.Id))
                ownedAuras[aura.SpellInfo.Id] = new List<Aura>();

            ownedAuras[aura.SpellInfo.Id].Add(aura);
        }
        public AuraApplication CreateAuraApplication(Aura aura, uint effMask) { return null; }
        public void ApplyAuraEffect(Aura aura, byte effIndex) { }
        public void ApplyAura(AuraApplication aurApp, uint effMask) { }
        public void UnapplyAura(AuraRemoveMode removeMode) { }
        public void UnapplyAura(AuraApplication aurApp, AuraRemoveMode removeMode) { }
        public void RemoveNoStackAuraApplicationsDueToAura(Aura aura) { }
        public void RemoveNoStackAurasDueToAura(Aura aura) { }
        public bool IsNoStackAuraDueToAura(Aura appliedAura, Aura existingAura) { return false; }
        public void RegisterAuraEffect(AuraEffect aurEff, bool apply) { }

        // OwnedAuras container management
        public void RemoveOwnedAura(AuraRemoveMode removeMode = AuraRemoveMode.Default) { }
        public void RemoveOwnedAura(uint spellId, Guid casterGuid = default, uint reqEffMask = 0, AuraRemoveMode removeMode = AuraRemoveMode.Default) { }
        public void RemoveOwnedAura(Aura aura, AuraRemoveMode removeMode = AuraRemoveMode.Default) { }
        public Aura GetOwnedAura(int spellId, ulong casterId, uint reqEffMask, Aura exceptAura = null)
        {
            if (!ownedAuras.ContainsKey(spellId))
                return null;

            return ownedAuras[spellId].FirstOrDefault(sameSpellAura => sameSpellAura.CasterId == casterId && exceptAura != sameSpellAura);
        }

        // AppliedAuras container management
        public void RemoveAura(AuraRemoveMode mode = AuraRemoveMode.Default) { }
        public void RemoveAura(uint spellId, Guid casterGuid = default, uint reqEffMask = 0, AuraRemoveMode removeMode = AuraRemoveMode.Default) { }
        public void RemoveAura(AuraApplication aurApp, AuraRemoveMode mode = AuraRemoveMode.Default) { }
        public void RemoveAura(Aura aur, AuraRemoveMode mode = AuraRemoveMode.Default) { }

        // Convenience methods removing auras by predicate
        public void RemoveAppliedAuras(Predicate<AuraApplication> check) { }
        public void RemoveOwnedAuras(Predicate<Aura> check) { }

        // Optimized overloads taking advantage of map key
        public void RemoveAppliedAuras(uint spellId, Predicate<AuraApplication> check) { }
        public void RemoveOwnedAuras(uint spellId, Predicate<Aura> check) { }

        public void RemoveAurasByType(AuraType auraType, Predicate<AuraApplication> check) { }
        public void RemoveAurasDueToSpell(uint spellId, Guid casterGuid = default, uint reqEffMask = 0, AuraRemoveMode removeMode = AuraRemoveMode.Default) { }
        public void RemoveAuraFromStack(uint spellId, Guid casterGuid = default, AuraRemoveMode removeMode = AuraRemoveMode.Default) { }
        public void RemoveAurasDueToSpellByDispel(uint spellId, uint dispellerSpellId, Guid casterGuid, Unit dispeller, byte chargesRemoved = 1) { }
        public void RemoveAurasDueToSpellBySteal(uint spellId, Guid casterGuid, Unit stealer) { }
        public void RemoveAurasDueToItemSpell(uint spellId, Guid castItemGuid) { }
        public void RemoveAurasByType(AuraType auraType, Guid casterGuid = default, Aura except = null, bool negative = true, bool positive = true) { }
        public void RemoveNotOwnSingleTargetAuras(uint newPhase = 0x0, bool phaseid = false) { }
        public void RemoveAurasWithInterruptFlags(uint flag, uint except = 0) { }
        public void RemoveAurasWithAttribute(uint flags) { }
        public void RemoveAurasWithMechanic(uint mechanicMask, AuraRemoveMode removemode = AuraRemoveMode.Default, uint except = 0) { }
        public void RemoveMovementImpairingAuras() { }

        public void RemoveAreaAurasDueToLeaveWorld() { }
        public void RemoveAllAuras() { }
        public void RemoveArenaAuras() { }
        public void RemoveAurasOnEvade() { }
        public void RemoveAllAurasOnDeath() { }
        public void RemoveAllAurasRequiringDeadTarget() { }
        public void RemoveAllAurasExceptType(AuraType type) { }
        public void RemoveAllAurasExceptType(AuraType type1, AuraType type2) { }
        public void DelayOwnedAuras(uint spellId, Guid caster, int delaytime) { }

        public bool HasAuraEffect(uint spellId, int effIndex, Guid caster = default) { return false; }
        public uint GetAuraCount(uint spellId) { return 0; }
        public bool HasAura(uint spellId, Guid casterGuid = default, uint reqEffMask = 0) { return false; }
        public bool HasAuraType(AuraType auraType) { return false; }
        public bool HasAuraTypeWithCaster(AuraType auratype, Guid caster) { return false; }
        public bool HasAuraTypeWithMiscvalue(AuraType auratype, int miscvalue) { return false; }
        public bool HasAuraTypeWithAffectMask(AuraType auratype, SpellInfo affectedSpell) { return false; }
        public bool HasAuraTypeWithValue(AuraType auratype, int value) { return false; }
        public bool HasNegativeAuraWithInterruptFlag(uint flag, Guid guid = default) { return false; }
        public bool HasNegativeAuraWithAttribute(uint flag, Guid guid = default) { return false; }
        public bool HasAuraWithMechanic(uint mechanicMask) { return false; }

        #endregion

        #region Spell and aura bonus calculations

        public void ModifyAuraState(AuraStateType flag, bool apply) { }
        public uint BuildAuraStateUpdateForTarget(Unit target) { return 0; }
        public bool HasAuraState(AuraStateType flag, SpellInfo spellProto = null, Unit caster = null) { return false; }
        public void UnsummonAllTotems() { }
        public bool IsMagnet() { return false; }
        public Unit GetMagicHitRedirectTarget(Unit victim, SpellInfo spellProto) { return null; }
        public Unit GetMeleeHitRedirectTarget(Unit victim, SpellInfo spellProto = null) { return null; }

        public int SpellBaseDamageBonusDone(SpellSchoolMask schoolMask) { return 0; }
        public int SpellBaseDamageBonusTaken(SpellSchoolMask schoolMask) { return 0; }
        public int SpellDamageBonusDone(Unit victim, SpellInfo spellProto, int damage, SpellDamageType damagetype, SpellEffectInfo effect, uint stack = 1) { return 0; }
        public float SpellDamagePctDone(Unit victim, SpellInfo spellProto, SpellDamageType damagetype) { return 0.0f; }
        public void ApplySpellMod(SpellInfo spellInfo, SpellModifierType modifierType, ref int value) { }
        public void ApplySpellMod(SpellInfo spellInfo, SpellModifierType modifierType, ref float value) { }

        public int SpellDamageBonusTaken(Unit caster, SpellInfo spellProto, int damage, SpellDamageType damagetype, SpellEffectInfo effect, uint stack = 1)
        {
            return damage;
        }
        public int SpellBaseHealingBonusDone(SpellSchoolMask schoolMask) { return 0; }
        public int SpellBaseHealingBonusTaken(SpellSchoolMask schoolMask) { return 0; }
        public uint SpellHealingBonusDone(Unit victim, SpellInfo spellProto, uint healamount, SpellDamageType damagetype, SpellEffectInfo effect, uint stack = 1) { return 0; }
        public float SpellHealingPctDone(Unit victim, SpellInfo spellProto) { return 0.0f; }
        public uint SpellHealingBonusTaken(Unit caster, SpellInfo spellProto, uint healamount, SpellDamageType damagetype, SpellEffectInfo effect, uint stack = 1) { return 0; }

        public uint MeleeDamageBonusDone(Unit pVictim, uint damage, WeaponAttackType attType, SpellInfo spellProto = null) { return 0; }
        public uint MeleeDamageBonusTaken(Unit attacker, uint pdamage, WeaponAttackType attType, SpellInfo spellProto = null) { return 0; }

        public bool IsSpellBlocked(Unit victim, SpellInfo spellProto, WeaponAttackType attackType = WeaponAttackType.BaseAttack) { return false; }
        public bool IsSpellCrit(Unit victim, SpellInfo spellProto, SpellSchoolMask schoolMask, WeaponAttackType attackType = WeaponAttackType.BaseAttack) { return false; }
        public float GetUnitSpellCriticalChance(Unit victim, SpellInfo spellProto, SpellSchoolMask schoolMask, WeaponAttackType attackType = WeaponAttackType.BaseAttack) { return 0.0f; }
        public int SpellCriticalHealingBonus(SpellInfo spellProto, int damage, Unit victim) { return 0; }

        public uint GetCastingTimeForBonus(SpellInfo spellProto, SpellDamageType damagetype, uint castingTime) { return 0; }
        public float CalculateDefaultCoefficient(SpellInfo spellProto, SpellDamageType damagetype) { return 0.0f; }
        public uint GetRemainingPeriodicAmount(Guid caster, uint spellId, AuraType auraType, int effectIndex = 0) { return 0; }

        public void ApplySpellImmune(uint spellId, uint op, uint type, bool apply) { }
        public void ApplySpellDispelImmunity(SpellInfo spellProto, SpellDispelType spellDispelType, bool apply) { }
        public uint GetSchoolImmunityMask() { return 0; }
        public uint GetMechanicImmunityMask() { return 0; }
        public bool IsImmunedToDamage(SpellSchoolMask meleeSchoolMask) { return false; }
        public bool IsImmunedToDamage(SpellInfo spellProto) { return false; }
        public virtual bool IsImmunedToSpellEffect(SpellInfo spellProto, int index) { return false; }

        public bool IsDamageReducedByArmor(SpellSchoolMask damageSchoolMask, SpellInfo spellProto = null, int effIndex = -1) { return false; }
        public uint CalcArmorReducedDamage(Unit attacker, Unit victim, uint damage, SpellInfo spellProto, WeaponAttackType attackType = WeaponAttackType.BaseAttack) { return 0; }
        public uint CalcSpellResistance(Unit victim, SpellSchoolMask schoolMask, SpellInfo spellProto) { return 0; }
        public void CalcAbsorbResist(Unit victim, SpellSchoolMask schoolMask, SpellDamageType damagetype, int damage, ref int absorb, ref int resist, SpellInfo spellProto = null) { }
        public void CalcHealAbsorb(Unit victim, SpellInfo spellProto, ref uint healAmount, ref uint absorb) { }

        public float ApplyEffectModifiers(SpellInfo spellProto, int effectIndex, float value)
        {
            //var modOwner = this;
            /*modOwner->ApplySpellMod(spellProto->Id, SPELLMOD_ALL_EFFECTS, value);
            switch (effect_index)
            {
                case 0:
                    modOwner->ApplySpellMod(spellProto->Id, SPELLMOD_EFFECT1, value);
                    break;
                case 1:
                    modOwner->ApplySpellMod(spellProto->Id, SPELLMOD_EFFECT2, value);
                    break;
                case 2:
                    modOwner->ApplySpellMod(spellProto->Id, SPELLMOD_EFFECT3, value);
                    break;
                case 3:
                    modOwner->ApplySpellMod(spellProto->Id, SPELLMOD_EFFECT4, value);
                    break;
                case 4:
                    modOwner->ApplySpellMod(spellProto->Id, SPELLMOD_EFFECT5, value);
                    break;
            }*/
            return value;
        }
        public int CalcSpellDuration(SpellInfo spellProto) { return 0; }
        public int ModSpellDuration(SpellInfo spellProto, Unit target, int duration, bool positive, uint effectMask) { return 0; }
        public void ModSpellCastTime(SpellInfo spellProto, ref int castTime, Spell spell = null) { }
        public void ModSpellDurationTime(SpellInfo spellProto, ref int castTime, Spell spell = null) { }
        public float CalculateLevelPenalty(SpellInfo spellProto) { return 0; }

        #endregion

        #region Speed, motion and movement

        public void UpdateSpeed(UnitMoveType type)
        {
            int mainSpeedMod = 0;
            float stackBonus = 1.0f;
            float nonStackBonus = 1.0f;

            switch (type)
            {
                // only apply debuffs
                case UnitMoveType.RunBack:
                    break;
                case UnitMoveType.Run:
                    mainSpeedMod = /*GetMaxPositiveAuraModifier(SPELL_AURA_MOD_INCREASE_SPEED)*/0;
                    stackBonus = /*GetTotalAuraMultiplier(SPELL_AURA_MOD_SPEED_ALWAYS)*/0;
                    nonStackBonus += /*GetMaxPositiveAuraModifier(SPELL_AURA_MOD_SPEED_NOT_STACK) / 100.0f*/0;
                    break;
                default:
                    Debug.LogErrorFormat("Characters::UpdateSpeed: Unsupported move type - {0}", type);
                    return;
            }

            // now we ready for speed calculation
            float speed = Mathf.Max(nonStackBonus, stackBonus);
            if (mainSpeedMod != 0)
                speed *= mainSpeedMod;

            switch (type)
            {
                case UnitMoveType.Run:
                    // Normalize speed by 191 aura SPELL_AURA_USE_NORMAL_MOVEMENT_SPEED if need #TODO
                    int normalization/* = GetMaxPositiveAuraModifier(SPELL_AURA_USE_NORMAL_MOVEMENT_SPEED)*/ = 0;
                    if (normalization > 0)
                    {
                        // Use speed from aura
                        float maxSpeed = normalization / unitMovementDefinition.BaseSpeedByType(type);
                        if (speed > maxSpeed)
                            speed = maxSpeed;
                    }

                    // force minimum speed rate @ aura 437 SPELL_AURA_MOD_MINIMUM_SPEED_RATE
                    int minSpeedModRate = /*GetMaxPositiveAuraModifier(SPELL_AURA_MOD_MINIMUM_SPEED_RATE)*/0;
                    if (minSpeedModRate != 0)
                    {
                        float minSpeed = minSpeedModRate / unitMovementDefinition.BaseSpeedByType(type);
                        if (speed < minSpeed)
                            speed = minSpeed;
                    }
                    break;
            }

            // Apply strongest slow aura mod to speed
            int slow = /*GetMaxNegativeAuraModifier(SPELL_AURA_MOD_DECREASE_SPEED)*/0;
            if (slow != 0)
                speed *= slow;

            float minSpeedMod = /*(float)GetMaxPositiveAuraModifier(SPELL_AURA_MOD_MINIMUM_SPEED)*/0;
            if (minSpeedMod > 0)
            {
                float minSpeed = minSpeedMod / 100.0f;
                if (speed < minSpeed)
                    speed = minSpeed;
            }

            SetSpeedRate(type, speed);
        }

        public float GetSpeed(UnitMoveType type)
        {
            return speedRates[type] * unitMovementDefinition.BaseSpeedByType(type);
        }

        public float GetSpeedRate(UnitMoveType type)
        {
            return speedRates[type];
        }

        public void SetSpeed(UnitMoveType type, float newValue)
        {
            SetSpeedRate(type, newValue / unitMovementDefinition.BaseSpeedByType(type));
        }

        public void SetSpeedRate(UnitMoveType type, float rate)
        {
            if (rate < 0)
                rate = 0.0f;

            speedRates[type] = rate;
        }

        public bool IsStopped() { return !(HasUnitState(UnitState.Moving)); }

        public void StopMoving() { }

        public void AddMovementFlag(MovementFlags f) { MovementInfo.AddMovementFlag(f); }

        public void RemoveMovementFlag(MovementFlags f) { MovementInfo.RemoveMovementFlag(f); }

        public bool HasMovementFlag(MovementFlags f) { return MovementInfo.HasMovementFlag(f); }

        public float GetPositionZMinusOffset() { return 0.0f; }

        public void SetControlled(bool apply, UnitState state) { }

        #endregion

        public abstract void Accept(IUnitVisitor unitVisitor);

        private void OnDeathStateChanged()
        {
            deathState = (DeathState)unitState.DeathState;
        }

        private void OnHealthStateChanged()
        {
            SetIntValue(EntityFields.Health, unitState.Health);
        }
    }
}
