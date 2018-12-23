using System;
using System.Collections.Generic;
using System.Linq;
using JetBrains.Annotations;
using UnityEngine;

using Debug = UnityEngine.Debug;
using Assert = UnityEngine.Assertions.Assert;
using Mathf = UnityEngine.Mathf;
using Vector3 = UnityEngine.Vector3;
using AuraApplicationList = System.Collections.Generic.List<Core.AuraApplication>;
using DispelChargesList = System.Collections.Generic.List<System.Collections.Generic.KeyValuePair<Core.Aura, short>>;

namespace Core
{
    public abstract class Unit : WorldEntity
    {
        [SerializeField, UsedImplicitly, Header("Unit"), Space(10)] private CapsuleCollider unitCollider;

        public override EntityType EntityType => EntityType.Unit;
        public CapsuleCollider UnitCollider => unitCollider;

        #region Unit data

        protected Dictionary<SpellSchools, float> threatModifier;
        protected Dictionary<WeaponAttackType, uint> baseAttackSpeed;
        protected Dictionary<WeaponAttackType, float> modAttackSpeedPct;
        protected Dictionary<WeaponAttackType, uint> attackTimer;
        protected Dictionary<int, List<Aura>> ownedAuras;
        protected Dictionary<Guid, AuraApplication> appliedAuras;
        protected Dictionary<Stats, float> createStats = new Dictionary<Stats, float>();
        protected Dictionary<UnitMods, Dictionary<UnitModifierType, float>> auraModifiersGroup;
        protected Dictionary<WeaponAttackType, Dictionary<WeaponDamageRange, float>> weaponDamage;
        protected Dictionary<UnitMoveType, float> speedRates = new Dictionary<UnitMoveType, float>();
        protected Dictionary<CurrentSpellTypes, Spell> currentSpells;
        protected Dictionary<AuraStateType, AuraApplication> auraStateAuras;    // used for improve performance of aura state checks on aura apply/remove
        protected Dictionary<AuraType, List<AuraEffect>> modifierAuras;         // all modifier auras
        protected List<AuraApplication> interruptableAuras;                     // auras which have interrupt mask applied on unit
        protected List<AuraApplication> visibleAuras;
        protected List<AuraApplication> visibleAurasToUpdate;
        protected List<AuraEffect> modAuras;
        protected List<Aura> removedAuras;
        protected List<Aura> singleCastAuras;                                   // cast singlecast auras
        protected DynamicEntity dynEntity;
        protected GameEntity gameEntity;
        protected List<Unit> attackers;

        protected Unit attacking;
        protected UnitAI ai;
        protected UnitAI disabledAI;
        protected UnitTypeMask unitTypeMask;
        protected bool shouldReacquireTarget;
        protected bool autoRepeatFirstCast;
        protected uint regenTimer;
        protected int procDeep;

        public bool IsMovementBlocked => HasUnitState(UnitState.Root) || HasUnitState(UnitState.Stunned);

        public Player MovedPlayer { get; set; }
        public MotionMaster MotionMaster { get; protected set; }
        public ThreatManager ThreatManager { get; protected set; }
        public HostileReferenceManager HostileRefManager { get; private set; }

        public UnitState UnitState { get; set; }
        public UnitMoveType MoveType { get; set; }
        public float CombatReach { get; private set; }

        #endregion

        internal override void Awake()
        {
            base.Awake();

            StatHelper.InitializeSpeedRates(speedRates);
        }

        public override void Attached()
        {
            base.Attached();

            worldEntityState.SetTransforms(worldEntityState.Transform, transform);

            WorldManager.UnitManager.Attach(this);

            SetMap(WorldManager.FindMap(1));
        }

        public override void Detached()
        {
            WorldManager.UnitManager.Detach(this);

            ResetMap();

            base.Detached();
        }

        public DiminishingLevels GetDiminishing(DiminishingGroup group) { return default(DiminishingLevels); }
        public void IncrDiminishing(DiminishingGroup group) { }
        public float ApplyDiminishingToDuration(DiminishingGroup group, ref int duration, Unit caster, DiminishingLevels level, int limitduration) { return 1.0f; }
        public void ApplyDiminishingAura(DiminishingGroup group, bool apply) { }

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

        public override void DoUpdate(int timeDelta)
        {

        }

        #region Unit info, stats and types

        public void AddUnitState(UnitState f) { UnitState |= f; }
        public bool HasUnitState(UnitState state) { return (UnitState & state) != 0; }
        public void ClearUnitState(UnitState f) { UnitState &= ~f; }
        public bool CanFreeMove() { return false; }

        public UnitTypeMask HasUnitTypeMask(UnitTypeMask mask) { return mask & unitTypeMask; }
        public void AddUnitTypeMask(UnitTypeMask mask) { unitTypeMask |= mask; }
        public bool IsSummon() { return (unitTypeMask & UnitTypeMask.Summon) != 0; }
        public bool IsGuardian() { return (unitTypeMask & UnitTypeMask.Guardian) != 0; }
        public bool IsPet() { return (unitTypeMask & UnitTypeMask.Pet) != 0; }
        public bool IsHunterPet() { return (unitTypeMask & UnitTypeMask.HunterPet) != 0; }
        public bool IsTotem() { return (unitTypeMask & UnitTypeMask.Totem) != 0; }
        public bool IsVehicle() { return (unitTypeMask & UnitTypeMask.Vehicle) != 0; }
        public CreatureType GetCreatureType() { return 0; }
        public uint GetCreatureTypeMask() { return 0; }

        public byte GetLevel() { return (byte)GetUintValue(EntityFields.Level); }
        public byte GetLevelForTarget() { return GetLevel(); }
        public void SetLevel(byte lvl) { }
        public byte GetRace() { return GetByteValue(EntityFields.Info, (byte)UnitInfoOffsets.Race); }
        public uint GetRaceMask() { return (uint)1 << (GetRace() - 1); }
        public byte GetClass() { return GetByteValue(EntityFields.Info, (byte)UnitInfoOffsets.Class); }
        public uint GetClassMask() { return (uint)1 << (GetClass() - 1); }
        public byte GetGender() { return GetByteValue(EntityFields.Info, (byte)UnitInfoOffsets.Gender); }

        public float GetStat(Stats stat) { return GetUintValue(stat.StatField()); }
        public void SetStat(Stats stat, int val) { SetStatIntValue(stat.StatField(), val); }
        public uint GetArmor() { return GetResistance(SpellSchools.Normal); }
        public void SetArmor(int val) { SetResistance(SpellSchools.Normal, val); }

        public uint GetResistance(SpellSchools school) { return GetUintValue(school.ResistanceField()); }
        public uint GetResistance(SpellSchoolMask mask) { return 0; }
        public void SetResistance(SpellSchools school, int val) { SetStatIntValue(school.ResistanceField(), val); }

        #endregion


        #region Health and powers

        public long GetHealth() { return GetLongValue(EntityFields.Health); }
        public long GetMaxHealth() { return GetLongValue(EntityFields.MaxHealth); }

        public bool IsFullHealth() { return GetHealth() == GetMaxHealth(); }
        public bool HealthBelowPct(int pct) { return GetHealth() < CountPctFromMaxHealth(pct); }
        public bool HealthBelowPctDamaged(int pct, uint damage) { return GetHealth() - damage < CountPctFromMaxHealth(pct); }
        public bool HealthAbovePct(int pct) { return GetHealth() > CountPctFromMaxHealth(pct); }
        public bool HealthAbovePctHealed(int pct, uint heal) { return GetHealth() + heal > CountPctFromMaxHealth(pct); }
        public float GetHealthPct() { return GetMaxHealth() > 0 ? 100.0f * GetHealth() / GetMaxHealth() : 0.0f; }
        public long CountPctFromMaxHealth(int pct) { return GetMaxHealth().CalculatePercentage(pct); }
        public long CountPctFromCurHealth(int pct) { return GetHealth().CalculatePercentage(pct); }

        public void SetHealth(long val) { }
        public void SetMaxHealth(long val) { }
        public void SetFullHealth() { SetHealth(GetMaxHealth()); }
        public long ModifyHealth(long val) { return 0; }
        public long GetHealthGain(long dVal) { return 0; }

        public PowerType GetPowerType() { return (PowerType)GetIntValue(EntityFields.DisplayPower); }
        public void SetPowerType(PowerType power) { }
        public int GetPower(PowerType power) { return 0; }
        public int GetMinPower(PowerType power) { return power == PowerType.LunarPower ? -100 : 0; }
        public int GetMaxPower(PowerType power) { return 0; }
        public float GetPowerPct(PowerType power) { return GetMaxPower(power) > 0 ? 100.0f * GetPower(power) / GetMaxPower(power) : 0.0f; }
        public int CountPctFromMaxPower(PowerType power, int pct) { return GetMaxPower(power).CalculatePercentage(pct); }
        public void SetPower(PowerType power, int val) { }
        public void SetMaxPower(PowerType power, int val) { }
        public int ModifyPower(PowerType power, int val) { return 0; }
        public int ModifyPowerPct(PowerType power, float pct, bool apply = true) { return 0; }

        #endregion


        #region Attack and cast time

        public int GetBaseAttackTime(WeaponAttackType att) { return 0; }
        public void SetBaseAttackTime(WeaponAttackType att, uint val) { }
        public void UpdateAttackTimeField(WeaponAttackType att) { }
        public void ApplyAttackTimePercentMod(WeaponAttackType att, float val, bool apply) { }
        public void ApplyCastTimePercentMod(float val, bool apply) { }

        #endregion


        #region Factions and relations

        public bool IsHostileTo(Unit unit)
        {
            return GetUintValue(EntityFields.GameEntityFaction) != unit.GetUintValue(EntityFields.GameEntityFaction);
        }
        public bool IsHostileToPlayers() { return false; }
        public bool IsFriendlyTo(Unit unit)
        {
            return GetUintValue(EntityFields.GameEntityFaction) == unit.GetUintValue(EntityFields.GameEntityFaction);
        }
        public bool IsNeutralToAll() { return false; }
        public bool IsInPartyWith(Unit unit) { return false; }
        public bool IsInRaidWith(Unit unit) { return false; }
        public void GetPartyMembers(List<Unit> units) { }

        #endregion


        #region Hit and damage calculation and trigger checks

        public void DealDamageMods(Unit victim, ref uint damage, ref uint absorb) { }
        public long DealDamage(Unit victim, long damage, CleanDamage cleanDamage = null, DamageEffectType damagetype = DamageEffectType.DirectDamage,
            SpellSchoolMask damageSchoolMask = SpellSchoolMask.Normal, SpellInfo spellProto = null, bool durabilityLoss = true)
        {
            // Hook for OnDamage Event
            //sScriptMgr->OnDamage(this, victim, damage);

            if (damage < 1)
                return 0;

            long health = victim.GetHealth();
            if (health <= damage)
            {
                Debug.Log("DealDamage: Victim just died");

                Kill(victim);
            }
            else
            {
                Debug.Log("DealDamage: Alive");

                victim.ModifyHealth(-damage);
            }
            return damage;
        }

        public void Kill(Unit victim, bool durabilityLoss = true)
        {
            // Prevent killing unit twice (and giving reward from kill twice)
            if (victim.GetHealth() <= 0)
                return;

            Debug.Log("Killed unit!");
            victim.DeathState = DeathState.Dead;
        }
        public void KillSelf(bool durabilityLoss = true) { Kill(this, durabilityLoss); }
        public int DealHeal(Unit victim, uint addhealth) { return 0; }

        public void ProcDamageAndSpell(Unit victim, uint procAttacker, uint procVictim, uint procEx, uint amount,
            WeaponAttackType attType = WeaponAttackType.BaseAttack, SpellInfo procSpell = null, SpellInfo procAura = null)
        { }
        public void ProcDamageAndSpellFor(bool isVictim, Unit target, uint procFlag, uint procExtra,
            WeaponAttackType attType, SpellInfo procSpell, uint damage, SpellInfo procAura = null)
        { }

        public void GetProcAurasTriggeredOnEvent(AuraApplicationList aurasTriggeringProc, AuraApplicationList procAuras, ProcEventInfo eventInfo) { }
        public void TriggerAurasProcOnEvent(ProcEventInfo eventInfo, AuraApplicationList procAuras) { }
        public void TriggerAurasProcOnEvent(CalcDamageInfo damageInfo) { }

        public void HandleEmoteCommand(uint animID) { }
        public void AttackerStateUpdate(Unit victim, WeaponAttackType attType = WeaponAttackType.BaseAttack, bool extra = false) { }
        public void FakeAttackerStateUpdate(Unit victim, WeaponAttackType attType = WeaponAttackType.BaseAttack) { }

        public void CalculateMeleeDamage(Unit victim, uint damage, CalcDamageInfo damageInfo, WeaponAttackType attackType = WeaponAttackType.BaseAttack) { }
        public void DealMeleeDamage(CalcDamageInfo damageInfo, bool durabilityLoss) { }
        public void HandleProcExtraAttackFor(Unit victim) { }

        public void CalculateSpellDamageTaken(SpellNonMeleeDamage damageInfo, int damage, SpellInfo spellInfo,
            WeaponAttackType attackType = WeaponAttackType.BaseAttack, bool crit = false)
        {
            if (damage < 0)
                return;

            Unit victim = damageInfo.Target;
            if (victim == null || !victim.IsAlive)
                return;

            SpellSchoolMask damageSchoolMask = damageInfo.SchoolMask;

            // Script Hook For CalculateSpellDamageTaken -- Allow scripts to change the Damage post class mitigation calculations
            //sScriptMgr->ModifySpellDamageTaken(damageInfo->target, damageInfo->attacker, damage);

            // Calculate absorb resist
            if (damage > 0)
            {
                int absorb = damageInfo.Absorb;
                int resist = damageInfo.Resist;
                CalcAbsorbResist(victim, damageSchoolMask, DamageEffectType.DirectDamage, damage, ref absorb, ref resist, spellInfo);
                damageInfo.Absorb = absorb;
                damageInfo.Resist = resist;
                damage -= damageInfo.Absorb + damageInfo.Resist;
            }
            else
                damage = 0;

            damageInfo.Damage = damage;
        }
        public void DealSpellDamage(SpellNonMeleeDamage damageInfo, bool durabilityLoss)
        {
            if (damageInfo == null)
                return;

            Unit victim = damageInfo.Target;

            if (victim == null)
                return;

            if (!victim.IsAlive)
                return;

            SpellInfo spellProto = BalanceManager.SpellInfosById.ContainsKey(damageInfo.SpellId) ? BalanceManager.SpellInfosById[damageInfo.SpellId] : null;
            if (spellProto == null)
            {
                Debug.LogErrorFormat("Unit.DealSpellDamage has wrong damageInfo->SpellID: {0}", damageInfo.SpellId);
                return;
            }

            // Call default DealDamage
            CleanDamage cleanDamage = new CleanDamage(damageInfo.Absorb, damageInfo.Damage);
            DealDamage(victim, damageInfo.Damage, cleanDamage, DamageEffectType.SpellDirectDamage, damageInfo.SchoolMask, spellProto);

            SpellManager.SpellDamageDone(this, victim, damageInfo.Damage, damageInfo.HitInfo == HitInfo.CriticalHit);
        }

        public uint GetDamageReduction(uint damage) { return GetCombatRatingDamageReduction(CombatRating.ResilencePlayerDamage, 1.0f, 100.0f, damage); }
        public void ApplyResilience(Unit victim, ref int damage) { }

        public float MeleeSpellMissChance(Unit victim, WeaponAttackType attType, uint spellId) { return 0.0f; }
        public SpellMissInfo MeleeSpellHitResult(Unit victim, SpellInfo spellInfo) { return SpellMissInfo.None; }
        public SpellMissInfo MagicSpellHitResult(Unit victim, SpellInfo spellInfo) { return SpellMissInfo.None; }
        public SpellMissInfo SpellHitResult(Unit victim, SpellInfo spellInfo, bool canReflect = false)
        {
            // Check for immune
            /*if (victim->IsImmunedToSpell(spellInfo))
                return SPELL_MISS_IMMUNE;*/

            // All positive spells can`t miss
            if (spellInfo.IsPositive() && !IsHostileTo(victim)) // prevent from affecting enemy by "positive" spell
                return SpellMissInfo.None;

            // Check for immune
            /*if (victim->IsImmunedToDamage(spellInfo))
                return SPELL_MISS_IMMUNE;*/

            if (this == victim)
                return SpellMissInfo.None;

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
            return SpellMissInfo.None;
        }

        public float GetUnitDodgeChanceAgainst(Unit attacker) { return 0.0f; }
        public float GetUnitParryChanceAgainst(Unit attacker) { return 0.0f; }
        public float GetUnitBlockChanceAgainst(Unit attacker) { return 0.0f; }
        public float GetUnitMissChance(WeaponAttackType attType) { return 0.0f; }
        public float GetUnitCriticalChance(WeaponAttackType attackType, Unit victim) { return 0.0f; }
        public int GetMechanicResistChance(SpellInfo spellInfo) { return 0; }
        public bool CanUseAttackType(WeaponAttackType attType) { return false; }

        public virtual uint GetBlockPercent() { return 30; }
        public float GetWeaponProcChance() { return 0.0f; }
        public float GetPpmProcChance(uint weaponSpeed, float ppm, SpellInfo spellProto) { return 0.0f; }
        public MeleeHitOutcome RollMeleeOutcomeAgainst(Unit victim, WeaponAttackType attType) { return MeleeHitOutcome.Normal; }

        #endregion


        #region Combat State

        public bool IsInCombat()
        {
            return HasFlag(EntityFields.UnitFlags, (int)UnitFlags.InCombat);
        }

        public bool IsInCombatWith(Unit target)
        {
            return true;
        }

        public void CombatStart(Unit target, bool initialAggro = true)
        {
        }

        public void SetInCombatState(bool isPvP, Unit enemy = null)
        {
        }

        public void SetInCombatWith(Unit enemy)
        {
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


        #region Spell casting

        public void SendHealSpellLog(Unit victim, uint spellId, uint health, uint overHeal, uint absorbed, bool crit = false) { }
        public int HealBySpell(Unit victim, SpellInfo spellInfo, int addHealth, bool critical = false) { return 0; }
        public void SendEnergizeSpellLog(Unit victim, uint spellId, int damage, PowerType powertype) { }
        public void EnergizeBySpell(Unit victim, uint spellId, int damage, PowerType powertype) { }


        public void CastSpell(SpellCastTargets targets, SpellInfo spellInfo, TriggerCastFlags triggerFlags = TriggerCastFlags.None, AuraEffect triggeredByAura = null, ulong originalCaster = 0)
        {
            new Spell(this, spellInfo, triggerFlags, originalCaster).Prepare(targets, triggeredByAura);
        }

        public Aura AddAura(uint spellId, Unit target) { return null; }
        public Aura AddAura(SpellInfo spellInfo, uint effMask, Unit target) { return null; }
        public void SetAuraStack(uint spellId, Unit target, uint stack) { }
        public void SendPlaySpellVisualKit(uint kitId, uint type) { }

        public virtual bool IsInWater() { throw new NotImplementedException(); }
        public virtual bool IsUnderWater() { throw new NotImplementedException(); }
        public virtual void UpdateUnderwaterState(Map m, float x, float y, float z) { throw new NotImplementedException(); }
        public void DeMorph() { throw new NotImplementedException(); }

        public void SendAttackStateUpdate(CalcDamageInfo damageInfo) { }
        public void SendAttackStateUpdate(uint hitInfo, Unit target, byte swingType, SpellSchoolMask damageSchoolMask,
            uint damage, uint absorbDamage, uint resist, VictimState targetState, uint blockedAmount)
        { }
        public void SendSpellNonMeleeDamageLog(ref SpellNonMeleeDamage log) { }
        public void SendSpellMiss(Unit target, uint spellId, SpellMissInfo missInfo) { }
        public void SendSpellDamageResist(Unit target, uint spellId) { }
        public void SendSpellDamageImmune(Unit target, uint spellId, bool isPeriodic) { }

        #endregion


        #region Movement changes and states

        public void NearTeleportTo(float x, float y, float z, float orientation, bool casting = false) { }
        public void SendTeleportPacket(Vector3 pos) { }
        public virtual bool UpdatePosition(float x, float y, float z, float ang, bool teleport = false) { return false; }
        public virtual bool UpdatePosition(Position pos, bool teleport = false) { return false; }
        public void UpdateOrientation(float orientation) { }
        public void UpdateHeight(float newZ) { }

        public void SendMoveKnockBack(Player player, float speedXY, float speedZ, float vcos, float vsin) { }
        public void KnockbackFrom(float x, float y, float speedXY, float speedZ) { }
        public void JumpTo(float speedXY, float speedZ, bool forward = true) { }
        public void JumpTo(WorldEntity obj, float speedZ, bool withOrientation = false) { }
        public void MonsterMoveWithSpeed(float x, float y, float z, float speed, bool generatePath = false, bool forceDestination = false) { }


        public void SendSetPlayHoverAnim(bool enable) { }
        public bool IsLevitating() { return MovementInfo.HasMovementFlag(MovementFlags.DisableGravity); }
        public bool IsWalking() { return MovementInfo.HasMovementFlag(MovementFlags.Walking); }
        public bool IsHovering() { return MovementInfo.HasMovementFlag(MovementFlags.Hover); }
        public bool SetWalk(bool enable) { return false; }
        public bool SetDisableGravity(bool disable) { return false; }
        public bool SetFall(bool enable) { return false; }
        public bool SetSwim(bool enable) { return false; }
        public bool SetCanFly(bool enable) { return false; }
        public bool SetWaterWalking(bool enable) { return false; }
        public bool SetFeatherFall(bool enable) { return false; }
        public bool SetHover(bool enable) { return false; }
        public bool SetCollision(bool disable) { return false; }
        public bool SetCanTransitionBetweenSwimAndFly(bool enable) { return false; }
        public bool SetCanTurnWhileFalling(bool enable) { return false; }
        public bool SetDoubleJump(bool enable) { return false; }
        public void SendSetVehicleRecId(uint vehicleId) { }

        #endregion


        #region Targets, controls, pets

        public virtual DeathState DeathState
        {
            get { return deathState; }
            set { deathState = value; }
        }
        public bool IsAlive => DeathState == DeathState.Alive;
        public bool IsDying => DeathState == DeathState.JustDied;
        public bool IsDead => DeathState == DeathState.Dead || DeathState == DeathState.Corpse;

        public ulong OwnerGuid
        {
            get { return GetULongValue(EntityFields.UnitSummonedBy); }
            set { SetULongValue(EntityFields.UnitSummonedBy, value); }
        }

        public ulong CreatorGuid
        {
            get { return GetULongValue(EntityFields.UnitCreatedBy); }
            set { SetULongValue(EntityFields.UnitCreatedBy, value); }
        }

        public ulong MinionGuid
        {
            get { return GetULongValue(EntityFields.UnitSummon); }
            set { SetULongValue(EntityFields.UnitSummon, value); }
        }

        public ulong CharmerGuid
        {
            get { return GetULongValue(EntityFields.UnitCharmedBy); }
            set { SetULongValue(EntityFields.UnitCharmedBy, value); }
        }

        public ulong PetGuid
        {
            get { return SummonSlots[UnitHelper.SummonSlotPet]; }
            set { SummonSlots[UnitHelper.SummonSlotPet] = value; }
        }

        public ulong CritterGuid
        {
            get { return GetULongValue(EntityFields.UnitCritter); }
            set { SetULongValue(EntityFields.UnitCritter, value); }
        }

        public bool IsControlledByPlayer() { return false; }
        public Guid GetCharmerOrOwnerGuid() { return default(Guid); }
        public Guid GetCharmerOrOwnerOrOwnGuid() { return default(Guid); }
        public bool IsCharmedOwnedByPlayerOrPlayer() { return false; }

        public Player SpellModOwner => null;
        public Unit Owner => null;
        public Guardian GuardianPet => null;
        public Minion FirstMinion => null;
        public Unit Charmer => null;
        public Unit Charm => null;
        public Unit CharmerOrOwner => null;
        public Unit CharmerOrOwnerOrSelf => null;
        public Player CharmerOrOwnerPlayerOrPlayerItself => null;
        public Player AffectingPlayer => null;

        public void SetMinion(Minion minion, bool apply) { }
        public void GetAllMinionsByEntry(List<TempSummon> minions, uint entry) { }
        public void RemoveAllMinionsByEntry(uint entry) { }
        public void SetCharm(Unit target, bool apply) { }
        public Unit GetNextRandomRaidMemberOrPet(float radius) { return null; }
        public bool SetCharmedBy(Unit charmer, CharmType type, AuraApplication aurApp = null) { return false; }
        public void RemoveCharmedBy(Unit charmer) { }
        public void RestoreFaction() { }

        public bool CreateVehicleKit(uint kitId, uint creatureEntry, bool loading = false) { throw new NotImplementedException(); }
        public void RemoveVehicleKit(bool onRemoveFromWorld = false) { throw new NotImplementedException(); }
        public bool IsOnVehicle(Unit vehicle) { throw new NotImplementedException(); }
        public Unit GetVehicleBase() { throw new NotImplementedException(); }
        public Creature GetVehicleCreatureBase() { throw new NotImplementedException(); }

        public List<Unit> Controlled { get; private set; }
        public Unit GetFirstControlled() { throw new NotImplementedException(); }
        public void RemoveAllControlled() { throw new NotImplementedException(); }

        public bool IsCharmed() { return CharmerGuid != 0; }
        public bool IsPossessed() { return HasUnitState(UnitState.Possessed); }
        public bool IsPossessedByPlayer() { throw new NotImplementedException(); }
        public bool IsPossessing() { throw new NotImplementedException(); }
        public bool IsPossessing(Unit u) { throw new NotImplementedException(); }

        public Pet CreateTamedPetFrom(Creature creatureTarget, uint spellID = 0) { throw new NotImplementedException(); }
        public Pet CreateTamedPetFrom(uint creatureEntry, uint spellID = 0) { throw new NotImplementedException(); }
        public bool InitTamedPet(Pet pet, byte level, uint spellID) { throw new NotImplementedException(); }

        #endregion


        #region Aura helpers

        // aura apply/remove helpers - you should better not use these
        public Aura TryStackingOrRefreshingExistingAura(SpellInfo newAuraSpellInfo, Unit caster, List<int> baseAmount = null, ulong casterId = 0)
        {
            Assert.IsTrue(casterId != 0 || caster != null);

            // check if these can stack anyway
            if (casterId == 0 && !newAuraSpellInfo.IsStackableOnOneSlotWithDifferentCasters())
                casterId = caster.NetworkId;

            // find current aura from spell and change it's stackamount, or refresh it's duration
            var foundAura = GetOwnedAura(newAuraSpellInfo.Id, casterId, 0);
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
        public void AddAura(UnitAura aura, Unit caster) { }
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
        public void RemoveOwnedAura(AuraRemoveMode removeMode = AuraRemoveMode.AuraRemoveByDefault) { }
        public void RemoveOwnedAura(uint spellId, Guid casterGuid = default(Guid), uint reqEffMask = 0, AuraRemoveMode removeMode = AuraRemoveMode.AuraRemoveByDefault) { }
        public void RemoveOwnedAura(Aura aura, AuraRemoveMode removeMode = AuraRemoveMode.AuraRemoveByDefault) { }
        public Aura GetOwnedAura(int spellId, ulong casterId, uint reqEffMask, Aura exceptAura = null)
        {
            if (!ownedAuras.ContainsKey(spellId))
                return null;

            return ownedAuras[spellId].FirstOrDefault(sameSpellAura => sameSpellAura.CasterId == casterId && exceptAura != sameSpellAura);
        }

        // AppliedAuras container management
        public void RemoveAura(AuraRemoveMode mode = AuraRemoveMode.AuraRemoveByDefault) { }
        public void RemoveAura(uint spellId, Guid casterGuid = default(Guid), uint reqEffMask = 0, AuraRemoveMode removeMode = AuraRemoveMode.AuraRemoveByDefault) { }
        public void RemoveAura(AuraApplication aurApp, AuraRemoveMode mode = AuraRemoveMode.AuraRemoveByDefault) { }
        public void RemoveAura(Aura aur, AuraRemoveMode mode = AuraRemoveMode.AuraRemoveByDefault) { }

        // Convenience methods removing auras by predicate
        public void RemoveAppliedAuras(Predicate<AuraApplication> check) { }
        public void RemoveOwnedAuras(Predicate<Aura> check) { }

        // Optimized overloads taking advantage of map key
        public void RemoveAppliedAuras(uint spellId, Predicate<AuraApplication> check) { }
        public void RemoveOwnedAuras(uint spellId, Predicate<Aura> check) { }


        public void RemoveAurasByType(AuraType auraType, Predicate<AuraApplication> check) { }
        public void RemoveAurasDueToSpell(uint spellId, Guid casterGuid = default(Guid), uint reqEffMask = 0, AuraRemoveMode removeMode = AuraRemoveMode.AuraRemoveByDefault) { }
        public void RemoveAuraFromStack(uint spellId, Guid casterGuid = default(Guid), AuraRemoveMode removeMode = AuraRemoveMode.AuraRemoveByDefault) { }
        public void RemoveAurasDueToSpellByDispel(uint spellId, uint dispellerSpellId, Guid casterGuid, Unit dispeller, byte chargesRemoved = 1) { }
        public void RemoveAurasDueToSpellBySteal(uint spellId, Guid casterGuid, Unit stealer) { }
        public void RemoveAurasDueToItemSpell(uint spellId, Guid castItemGuid) { }
        public void RemoveAurasByType(AuraType auraType, Guid casterGuid = default(Guid), Aura except = null, bool negative = true, bool positive = true) { }
        public void RemoveNotOwnSingleTargetAuras(uint newPhase = 0x0, bool phaseid = false) { }
        public void RemoveAurasWithInterruptFlags(uint flag, uint except = 0) { }
        public void RemoveAurasWithAttribute(uint flags) { }
        public void RemoveAurasWithFamily(SpellFamilyNames family, long familyFlag, Guid casterGuid) { }
        public void RemoveAurasWithMechanic(uint mechanicMask, AuraRemoveMode removemode = AuraRemoveMode.AuraRemoveByDefault, uint except = 0) { }
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

        public void RemoveAllAuraStatMods() { }
        public void ApplyAllAuraStatMods() { }

        public List<AuraEffect> GetAuraEffectsByType(AuraType type) { return modAuras.FindAll(aura => aura.GetAuraType() == type); }
        public AuraEffect GetAuraEffect(uint spellId, int effIndex, Guid casterGuid = default(Guid)) { return null; }
        public AuraEffect GetAuraEffectOfRankedSpell(uint spellId, int effIndex, Guid casterGuid = default(Guid)) { return null; }
        public AuraEffect GetAuraEffect(AuraType type, SpellFamilyNames family, uint iconId, int effIndex) { return null; }
        public AuraEffect GetAuraEffect(AuraType type, SpellFamilyNames family, Flag128 familyFlag, Guid casterGuid = default(Guid)) { return null; }
        public AuraEffect GetDummyAuraEffect(SpellFamilyNames family, uint iconId, int effIndex) { return null; }

        public AuraApplication GetAuraApplication(uint spellId, Guid casterGuid = default(Guid), uint reqEffMask = 0, AuraApplication except = null) { return null; }
        public Aura GetAura(uint spellId, Guid casterGuid = default(Guid), uint reqEffMask = 0) { return null; }

        public AuraApplication GetAuraApplicationOfRankedSpell(uint spellId, Guid casterGuid = default(Guid), uint reqEffMask = 0, AuraApplication except = null) { return null; }
        public Aura GetAuraOfRankedSpell(uint spellId, Guid casterGuid = default(Guid), uint reqEffMask = 0) { return null; }

        public void GetDispellableAuraList(Unit caster, uint dispelMask, DispelChargesList dispelList) { }

        public bool HasAuraEffect(uint spellId, int effIndex, Guid caster = default(Guid)) { return false; }
        public uint GetAuraCount(uint spellId) { return 0; }
        public bool HasAura(uint spellId, Guid casterGuid = default(Guid), uint reqEffMask = 0) { return false; }
        public bool HasAuraType(AuraType auraType) { return false; }
        public bool HasAuraTypeWithCaster(AuraType auratype, Guid caster) { return false; }
        public bool HasAuraTypeWithMiscvalue(AuraType auratype, int miscvalue) { return false; }
        public bool HasAuraTypeWithAffectMask(AuraType auratype, SpellInfo affectedSpell) { return false; }
        public bool HasAuraTypeWithValue(AuraType auratype, int value) { return false; }
        public bool HasNegativeAuraWithInterruptFlag(uint flag, Guid guid = default(Guid)) { return false; }
        public bool HasNegativeAuraWithAttribute(uint flag, Guid guid = default(Guid)) { return false; }
        public bool HasAuraWithMechanic(uint mechanicMask) { return false; }

        public AuraEffect IsScriptOverriden(SpellInfo spell, int script) { return null; }
        public uint GetDiseasesByCaster(Guid casterGuid, bool remove = false) { return 0; }
        public uint GetDoTsByCaster(Guid casterGuid) { return 0; }

        public int GetTotalAuraModifier(AuraType auratype) { return 0; }
        public float GetTotalAuraMultiplier(AuraType auratype) { return 0.0f; }
        public int GetMaxPositiveAuraModifier(AuraType auratype) { return 0; }
        public int GetMaxNegativeAuraModifier(AuraType auratype) { return 0; }
        public int GetTotalAuraModifierByMiscMask(AuraType auratype, uint miscMask) { return 0; }
        public float GetTotalAuraMultiplierByMiscMask(AuraType auratype, uint miscMask) { return 0; }
        public int GetMaxPositiveAuraModifierByMiscMask(AuraType auratype, uint miscMask, AuraEffect except = null) { return 0; }
        public int GetMaxNegativeAuraModifierByMiscMask(AuraType auratype, uint miscMask) { return 0; }

        public int GetTotalAuraModifierByMiscValue(AuraType auratype, int miscValue) { return 0; }
        public float GetTotalAuraMultiplierByMiscValue(AuraType auratype, int miscValue) { return 0; }
        public int GetMaxPositiveAuraModifierByMiscValue(AuraType auratype, int miscValue) { return 0; }
        public int GetMaxNegativeAuraModifierByMiscValue(AuraType auratype, int miscValue) { return 0; }

        public int GetTotalAuraModifierByAffectMask(AuraType auratype, SpellInfo affectedSpell) { return 0; }
        public float GetTotalAuraMultiplierByAffectMask(AuraType auratype, SpellInfo affectedSpell) { return 0; }
        public int GetMaxPositiveAuraModifierByAffectMask(AuraType auratype, SpellInfo affectedSpell) { return 0; }
        public int GetMaxNegativeAuraModifierByAffectMask(AuraType auratype, SpellInfo affectedSpell) { return 0; }

        #endregion


        #region Stat helpers

        public float GetResistanceBuffMods(SpellSchools school, bool positive) { return 0.0f; }
        public void SetResistanceBuffMods(SpellSchools school, bool positive, float val) { }
        public void ApplyResistanceBuffModsMod(SpellSchools school, bool positive, float val, bool apply) { }
        public void ApplyResistanceBuffModsPercentMod(SpellSchools school, bool positive, float val, bool apply) { }
        public void InitStatBuffMods() { }
        public void ApplyStatBuffMod(Stats stat, float val, bool apply) { }
        public void ApplyStatPercentBuffMod(Stats stat, float val, bool apply) { }
        public void SetCreateStat(Stats stat, float val) { createStats[stat] = val; }
        public void SetCreateHealth(uint val) { SetUintValue(EntityFields.BaseHealth, val); }
        public uint GetCreateHealth() { return GetUintValue(EntityFields.BaseHealth); }
        public void SetCreateMana(uint val) { SetUintValue(EntityFields.BaseMana, val); }
        public uint GetCreateMana() { return GetUintValue(EntityFields.BaseMana); }
        public uint GetPowerIndex(uint powerType) { return 0; }
        public int GetCreatePowers(PowerType power) { return 0; }
        public float GetPosStat(Stats stat) { return GetFloatValue(stat.StatPositiveField()); }
        public float GetNegStat(Stats stat) { return GetFloatValue(stat.StatNegativeField()); }
        public float GetCreateStat(Stats stat) { return createStats[stat]; }

        #endregion


        #region Spell helpers

        public void SetCurrentCastSpell(Spell spell) { }
        public void InterruptSpell(CurrentSpellTypes spellType, bool withDelayed = true, bool withInstant = true) { }
        public void FinishSpell(CurrentSpellTypes spellType, bool ok = true) { }

        public Spell GetCurrentSpell(CurrentSpellTypes spellType) { return currentSpells[spellType]; }
        public Spell FindCurrentSpellBySpellId(uint spellID) { return null; }
        public int GetCurrentSpellCastTime(uint spellID) { return 0; }
        public virtual SpellInfo GetCastSpellInfo(SpellInfo spellInfo) { return null; }

        public SpellHistory SpellHistory { get; } = new SpellHistory();

        private DeathState deathState;
        public ulong[] SummonSlots { get; } = new ulong[UnitHelper.MaxSummonSlot];
        public ulong[] ObjectSlots { get; } = new ulong[UnitHelper.MaxGameEntitySlot];

        public ShapeshiftForm GetShapeshiftForm() { return (ShapeshiftForm)GetByteValue(EntityFields.BaseFlags, 3); }
        public void SetShapeshiftForm(ShapeshiftForm form) { }

        public bool IsInFeralForm() { return false; }
        public bool IsInDisallowedMountForm() { return false; }

        #endregion


        #region Stat system

        public bool HandleStatModifier(UnitMods unitMod, UnitModifierType modifierType, float amount, bool apply) { return false; }
        public void SetModifierValue(UnitMods unitMod, UnitModifierType modifierType, float value) { auraModifiersGroup[unitMod][modifierType] = value; }
        public float GetModifierValue(UnitMods unitMod, UnitModifierType modifierType) { return 0.0f; }
        public float GetTotalStatValue(Stats stat) { return 0.0f; }
        public float GetTotalAuraModValue(UnitMods unitMod) { return 0.0f; }
        public SpellSchools GetSpellSchoolByAuraGroup(UnitMods unitMod) { return SpellSchools.Normal; }
        public Stats GetStatByAuraGroup(UnitMods unitMod) { return Stats.Agility; }
        public PowerType GetPowerTypeByAuraGroup(UnitMods unitMod) { return PowerType.Mana; }

        public virtual bool UpdateStats(Stats stat) { return false; }
        public virtual bool UpdateAllStats() { return false; }
        public virtual void UpdateResistances(SpellSchools school) { }
        public virtual void UpdateAllResistances() { }
        public virtual void UpdateArmor() { }
        public virtual void UpdateMaxHealth() { }
        public virtual void UpdateMaxPower(PowerType power) { }
        public virtual void UpdateAttackPowerAndDamage(bool ranged = false) { }
        public virtual void UpdateDamagePhysical(WeaponAttackType attType) { }
        public float GetTotalAttackPowerValue(WeaponAttackType attType) { return 0.0f; }
        public float GetWeaponDamageRange(WeaponAttackType attType, WeaponDamageRange type) { return 0.0f; }
        public void SetBaseWeaponDamage(WeaponAttackType attType, WeaponDamageRange damageRange, float value) { weaponDamage[attType][damageRange] = value; }
        public virtual void CalculateMinMaxDamage(WeaponAttackType attType, bool normalized, bool addTotalPct, ref float minDamage, ref float maxDamage) { }
        public uint CalculateDamage(WeaponAttackType attType, bool normalized, bool addTotalPct) { return 0; }
        public float GetApMultiplier(WeaponAttackType attType, bool normalized) { return 0.0f; }

        #endregion


        #region Display info

        public List<AuraApplication> GetVisibleAuras() { return visibleAuras; }
        public bool HasVisibleAura(AuraApplication aurApp) { return visibleAuras.Contains(aurApp); }
        public void SetVisibleAura(AuraApplication aurApp) { }
        public void SetVisibleAuraUpdate(AuraApplication aurApp) { visibleAurasToUpdate.Add(aurApp); }
        public void RemoveVisibleAura(AuraApplication aurApp) { }
        public void UpdateInterruptMask() { }

        public uint GetDisplayId() { return GetUintValue(EntityFields.DisplayId); }
        public virtual void SetDisplayId(uint modelId) { }
        public uint GetNativeDisplayId() { return GetUintValue(EntityFields.NativeDisplayId); }
        public void RestoreDisplayId() { }
        public void SetNativeDisplayId(uint modelId) { SetUintValue(EntityFields.NativeDisplayId, modelId); }


        public uint GetModelForForm(ShapeshiftForm form) { return 0; }
        public uint GetModelForTotem(PlayerTotemType totemType) { return 0; }

        #endregion


        #region  Dynamic object management

        public void _RegisterDynObject(WorldEntity dynObj) { }
        public void _UnregisterDynObject(WorldEntity dynObj) { }
        public WorldEntity GetDynObject(uint spellId) { return null; }
        public List<WorldEntity> GetDynObjects(uint spellId) { return null; }
        public void RemoveDynObject(uint spellId) { }
        public void RemoveAllDynObjects() { }

        public GameEntity GetGameEntity(uint spellId) { return null; }
        public List<GameEntity> GetGameEntities(uint spellId) { return null; }
        public void AddGameObject(GameEntity gameObj) { }
        public void RemoveGameObject(GameEntity gameObj, bool del) { }
        public void RemoveGameObject(uint spellid, bool del) { }
        public void RemoveAllGameObjects() { }

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
        public int SpellDamageBonusDone(Unit victim, SpellInfo spellProto, int damage, DamageEffectType damagetype, SpellEffectInfo effect, uint stack = 1) { return 0; }
        public float SpellDamagePctDone(Unit victim, SpellInfo spellProto, DamageEffectType damagetype) { return 0.0f; }
        public int SpellDamageBonusTaken(Unit caster, SpellInfo spellProto, int damage, DamageEffectType damagetype, SpellEffectInfo effect, uint stack = 1) { return 0; }
        public int SpellBaseHealingBonusDone(SpellSchoolMask schoolMask) { return 0; }
        public int SpellBaseHealingBonusTaken(SpellSchoolMask schoolMask) { return 0; }
        public uint SpellHealingBonusDone(Unit victim, SpellInfo spellProto, uint healamount, DamageEffectType damagetype, SpellEffectInfo effect, uint stack = 1) { return 0; }
        public float SpellHealingPctDone(Unit victim, SpellInfo spellProto) { return 0.0f; }
        public uint SpellHealingBonusTaken(Unit caster, SpellInfo spellProto, uint healamount, DamageEffectType damagetype, SpellEffectInfo effect, uint stack = 1) { return 0; }

        public uint MeleeDamageBonusDone(Unit pVictim, uint damage, WeaponAttackType attType, SpellInfo spellProto = null) { return 0; }
        public uint MeleeDamageBonusTaken(Unit attacker, uint pdamage, WeaponAttackType attType, SpellInfo spellProto = null) { return 0; }

        public bool IsSpellBlocked(Unit victim, SpellInfo spellProto, WeaponAttackType attackType = WeaponAttackType.BaseAttack) { return false; }
        public bool IsBlockCritical() { return false; }
        public bool IsSpellCrit(Unit victim, SpellInfo spellProto, SpellSchoolMask schoolMask, WeaponAttackType attackType = WeaponAttackType.BaseAttack) { return false; }
        public float GetUnitSpellCriticalChance(Unit victim, SpellInfo spellProto, SpellSchoolMask schoolMask, WeaponAttackType attackType = WeaponAttackType.BaseAttack) { return 0.0f; }
        public int SpellCriticalHealingBonus(SpellInfo spellProto, int damage, Unit victim) { return 0; }
        public void SetContestedPvP(Player attackedPlayer = null) { }

        public uint GetCastingTimeForBonus(SpellInfo spellProto, DamageEffectType damagetype, uint castingTime) { return 0; }
        public float CalculateDefaultCoefficient(SpellInfo spellProto, DamageEffectType damagetype) { return 0.0f; }

        public uint GetRemainingPeriodicAmount(Guid caster, uint spellId, AuraType auraType, int effectIndex = 0) { return 0; }

        public void ApplySpellImmune(uint spellId, uint op, uint type, bool apply) { }
        public void ApplySpellDispelImmunity(SpellInfo spellProto, DispelType dispelType, bool apply) { }
        public virtual bool IsImmunedToSpell(SpellInfo spellProto) { return false; } // redefined in Creature
        public uint GetSchoolImmunityMask() { return 0; }
        public uint GetMechanicImmunityMask() { return 0; }

        public bool IsImmunedToDamage(SpellSchoolMask meleeSchoolMask) { return false; }
        public bool IsImmunedToDamage(SpellInfo spellProto) { return false; }
        public virtual bool IsImmunedToSpellEffect(SpellInfo spellProto, int index) { return false; } // redefined in Creature

        public bool IsDamageReducedByArmor(SpellSchoolMask damageSchoolMask, SpellInfo spellProto = null, int effIndex = -1) { return false; }
        public uint CalcArmorReducedDamage(Unit attacker, Unit victim, uint damage, SpellInfo spellProto, WeaponAttackType attackType = WeaponAttackType.BaseAttack) { return 0; }
        public uint CalcSpellResistance(Unit victim, SpellSchoolMask schoolMask, SpellInfo spellProto) { return 0; }
        public void CalcAbsorbResist(Unit victim, SpellSchoolMask schoolMask, DamageEffectType damagetype, int damage, ref int absorb, ref int resist, SpellInfo spellProto = null) { }
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
        public int CalculateSpellDamage(Unit target, SpellInfo spellProto, int effectIndex, int basePoints = 0)
        {
            return spellProto.Effects[effectIndex]?.CalcValue(this, basePoints, target) ?? 0;
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
                        float maxSpeed = normalization / StatHelper.BaseMovementSpeed(type);
                        if (speed > maxSpeed)
                            speed = maxSpeed;
                    }

                    // force minimum speed rate @ aura 437 SPELL_AURA_MOD_MINIMUM_SPEED_RATE
                    int minSpeedModRate = /*GetMaxPositiveAuraModifier(SPELL_AURA_MOD_MINIMUM_SPEED_RATE)*/0;
                    if (minSpeedModRate != 0)
                    {
                        float minSpeed = minSpeedModRate / StatHelper.BaseMovementSpeed(type);
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
            return speedRates[type] * StatHelper.BaseMovementSpeed(type);
        }

        public float GetSpeedRate(UnitMoveType type)
        {
            return speedRates[type];
        }

        public void SetSpeed(UnitMoveType type, float newValue)
        {
            SetSpeedRate(type, newValue / StatHelper.BaseMovementSpeed(type));
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


        public bool IsMoving() { return MovementInfo.HasMovementFlag(MovementFlags.MaskMoving); }
        public bool IsTurning() { return MovementInfo.HasMovementFlag(MovementFlags.MaskTurning); }
        public bool IsFlying() { return MovementInfo.HasMovementFlag(MovementFlags.Flying | MovementFlags.DisableGravity); }
        public bool IsFalling() { return false; }

        public ulong GetTarget() { return GetULongValue(EntityFields.Target); }
        public virtual void SetTarget(ulong targetNetworkId) { }

        private float GetCombatRatingReduction(CombatRating cr) { return 0.0f; }
        private uint GetCombatRatingDamageReduction(CombatRating cr, float rate, float cap, uint damage) { return 0; }

        public abstract void Accept(IUnitVisitor unitVisitor);
    }
}
