using System.Collections.Generic;
using System.Linq;
using Common;
using JetBrains.Annotations;
using UdpKit;
using UnityEngine;

using EventHandler = Common.EventHandler;

namespace Core
{
    public abstract partial class Unit : WorldEntity
    {
        public new class CreateToken : WorldEntity.CreateToken
        {
            public DeathState DeathState { private get; set; }
            public bool FreeForAll { private get; set; }
            public int FactionId { private get; set; }

            public override void Read(UdpPacket packet)
            {
                base.Read(packet);

                DeathState = (DeathState)packet.ReadInt();
                FactionId = packet.ReadInt();
                FreeForAll = packet.ReadBool();
            }

            public override void Write(UdpPacket packet)
            {
                base.Write(packet);

                packet.WriteInt((int)DeathState);
                packet.WriteInt(FactionId);
                packet.WriteBool(FreeForAll);
            }

            protected void Attached(Unit unit)
            {
                unit.DeathState = DeathState;
                unit.Faction = unit.Balance.FactionsById[FactionId];
                unit.FreeForAll = FreeForAll;
            }
        }

        [SerializeField, UsedImplicitly, Header(nameof(Unit)), Space(10)]
        private CapsuleCollider unitCollider;
        [SerializeField, UsedImplicitly]
        private WarcraftController controller;
        [SerializeField, UsedImplicitly]
        private UnitAttributeDefinition unitAttributeDefinition;
        [SerializeField, UsedImplicitly]
        private UnitMovementDefinition unitMovementDefinition;

        private FactionDefinition faction;
        private UnitFlags unitFlags;
        private DeathState deathState;
        private AuraInterruptFlags auraInterruptFlags;
        private ulong targetId;
        private bool freeForAll;

        private CreateToken createToken;
        private EntityAttributeInt health;
        private EntityAttributeInt maxHealth;
        private EntityAttributeInt mana;
        private EntityAttributeInt maxMana;
        private EntityAttributeInt level;
        private EntityAttributeInt spellPower;
        private EntityAttributeFloat modHaste;
        private EntityAttributeFloat modRangedHaste;
        private EntityAttributeFloat modSpellHaste;
        private EntityAttributeFloat modRegenHaste;
        private EntityAttributeFloat critPercentage;
        private EntityAttributeFloat rangedCritPercentage;
        private EntityAttributeFloat spellCritPercentage;

        private readonly VisibleAuraController visibleAuraController = new VisibleAuraController();

        private readonly Dictionary<UnitMoveType, float> speedRates = new Dictionary<UnitMoveType, float>();
        private readonly Dictionary<AuraStateType, List<AuraApplication>> auraApplicationsByAuraState = new Dictionary<AuraStateType, List<AuraApplication>>();
        private readonly Dictionary<AuraEffectType, List<AuraEffect>> auraEffectsByAuraType = new Dictionary<AuraEffectType, List<AuraEffect>>();
        private readonly Dictionary<int, List<Aura>> ownedAurasById = new Dictionary<int, List<Aura>>();
        private readonly Dictionary<int, List<AuraApplication>> auraApplicationsByAuraId = new Dictionary<int, List<AuraApplication>>();
        private readonly List<AuraApplication> interruptableAuraApplications = new List<AuraApplication>();
        private readonly List<AuraApplication> auraApplications = new List<AuraApplication>();
        private readonly HashSet<AuraApplication> auraApplicationSet = new HashSet<AuraApplication>();
        private readonly List<Aura> ownedAuras = new List<Aura>();

        private readonly HashSet<AuraEffectHandleGroup> tempAuraHandleGroups = new HashSet<AuraEffectHandleGroup>();
        private readonly List<int> tempAuraEffectsToHandle = new List<int>();

        private ThreatManager ThreatManager { get; set; }
        private UnitState UnitState { get; set; }

        private bool FreeForAll
        {
            get => freeForAll;
            set
            {
                freeForAll = value;

                if (IsOwner)
                {
                    EntityState.Faction.FreeForAll = value;
                    createToken.FreeForAll = value;
                }
            }
        }
        
        private DeathState DeathState
        {
            get => deathState;
            set
            {
                deathState = value;

                if (IsOwner)
                {
                    EntityState.DeathState = (int)value;
                    createToken.DeathState = value;
                }
            }
        }

        private FactionDefinition Faction
        {
            get => faction;
            set
            {
                faction = value;

                if (IsOwner)
                {
                    EntityState.Faction.Id = value.FactionId;
                    createToken.FactionId = value.FactionId;
                }
            }
        }

        internal IReadOnlyList<AuraApplication> AuraApplications => auraApplications;
        internal WarcraftController Controller => controller;
        internal bool NeedUpdateVisibleAuras { set => visibleAuraController.NeedUpdateVisibleAuras = value; }

        public Unit Target { get; private set; }
        public SpellCast SpellCast { get; private set; }
        public IUnitState EntityState { get; private set; }
        public SpellHistory SpellHistory { get; private set; }
        public CapsuleCollider UnitCollider => unitCollider;
        public PlayerControllerDefinition ControllerDefinition => controller.ControllerDefinition;

        public int Health => health.Value;
        public int MaxHealth => maxHealth.Value;
        public int BaseMana => mana.Base;
        public int Mana => mana.Value;
        public int MaxMana => maxMana.Value;
        public int SpellPower => spellPower.Value;
        public float HealthRatio => maxHealth.Value > 0 ? (float)Health / MaxHealth : 0.0f;
        public bool HasFullHealth => health.Value == maxHealth.Value;
        public float HealthPercent => 100.0f * HealthRatio;

        public float ModHaste => modHaste.Value;
        public float ModRangedHaste => modRangedHaste.Value;
        public float ModSpellHaste => modSpellHaste.Value;
        public float ModRegenHaste => modRegenHaste.Value;
        public float CritPercentage => critPercentage.Value;
        public float RangedCritPercentage => rangedCritPercentage.Value;
        public float SpellCritPercentage => spellCritPercentage.Value;

        public bool IsMovementBlocked => HasState(UnitState.Root) || HasState(UnitState.Stunned);
        public bool IsAlive => DeathState == DeathState.Alive;
        public bool IsDead => DeathState == DeathState.Dead;
        public bool IsControlledByPlayer => this is Player;
        public bool IsStopped => !HasState(UnitState.Moving);

        public bool HealthBelowPercent(int percent) => health.Value < CountPercentFromMaxHealth(percent);
        public bool HealthAbovePercent(int percent) => health.Value > CountPercentFromMaxHealth(percent);
        public bool HealthAbovePercentHealed(int percent, int healAmount) => health.Value + healAmount > CountPercentFromMaxHealth(percent);
        public bool HealthBelowPercentDamaged(int percent, int damageAmount) => health.Value - damageAmount < CountPercentFromMaxHealth(percent);
        public long CountPercentFromMaxHealth(int percent) => maxHealth.Value.CalculatePercentage(percent);
        public long CountPercentFromCurrentHealth(int percent) => health.Value.CalculatePercentage(percent);
        public float GetSpeed(UnitMoveType type) => speedRates[type] * unitMovementDefinition.BaseSpeedByType(type);
        public float GetSpeedRate(UnitMoveType type) => speedRates[type];
        public float GetPowerPercent(SpellResourceType type) => GetMaxPower(type) > 0 ? 100.0f * GetPower(type) / GetMaxPower(type) : 0.0f;
        public int GetPower(SpellResourceType type) => mana.Value;
        public int GetMaxPower(SpellResourceType type) => maxMana.Value;

        [UsedImplicitly]
        protected override void Awake()
        {
            base.Awake();

            health = new EntityAttributeInt(this, unitAttributeDefinition.BaseHealth, int.MaxValue, EntityAttributes.Health);
            maxHealth = new EntityAttributeInt(this, unitAttributeDefinition.BaseMaxHealth, int.MaxValue, EntityAttributes.MaxHealth);
            mana = new EntityAttributeInt(this, unitAttributeDefinition.BaseMana, int.MaxValue, EntityAttributes.Power);
            maxMana = new EntityAttributeInt(this, unitAttributeDefinition.BaseMaxMana, int.MaxValue, EntityAttributes.MaxPower);
            level = new EntityAttributeInt(this, 1, int.MaxValue, EntityAttributes.Level);
            spellPower = new EntityAttributeInt(this, unitAttributeDefinition.BaseSpellPower, int.MaxValue, EntityAttributes.SpellPower);
            modHaste = new EntityAttributeFloat(this, 1.0f, float.MaxValue, EntityAttributes.ModHaste);
            modRangedHaste = new EntityAttributeFloat(this, 1.0f, float.MaxValue, EntityAttributes.ModRangedHaste);
            modSpellHaste = new EntityAttributeFloat(this, 1.0f, float.MaxValue, EntityAttributes.ModSpellHaste);
            modRegenHaste = new EntityAttributeFloat(this, 1.0f, float.MaxValue, EntityAttributes.ModRegenHaste);
            critPercentage = new EntityAttributeFloat(this, unitAttributeDefinition.CritPercentage, float.MaxValue, EntityAttributes.CritPercentage);
            rangedCritPercentage = new EntityAttributeFloat(this, 1.0f, unitAttributeDefinition.RangedCritPercentage, EntityAttributes.RangedCritPercentage);
            spellCritPercentage = new EntityAttributeFloat(this, 1.0f, unitAttributeDefinition.SpellCritPercentage, EntityAttributes.SpellCritPercentage);

            faction = Balance.DefaultFaction;
        }

        public sealed override void Attached()
        {
            base.Attached();

            HandleAttach();

            WorldManager.UnitManager.Attach(this);
        }

        public sealed override void Detached()
        {
            // called twice on client (from Detached Photon callback and manual in UnitManager.Dispose)
            // if he needs to instantly destroy current world and avoid any events
            if (!IsValid)
                return;

            HandleDetach();

            base.Detached();
        }

        public bool IsHostileTo(Unit unit)
        {
            if (unit == this)
                return false;

            if (unit.FreeForAll && FreeForAll)
                return true;

            return Faction.HostileFactions.Contains(unit.Faction);
        }

        public bool IsFriendlyTo(Unit unit)
        {
            if (unit == this)
                return true;

            if (unit.FreeForAll && FreeForAll)
                return false;

            return Faction.FriendlyFactions.Contains(unit.Faction);
        }

        protected virtual void HandleAttach()
        {
            createToken = (CreateToken) entity.AttachToken;
            EntityState = entity.GetState<IUnitState>();

            foreach (UnitMoveType moveType in StatUtils.UnitMoveTypes)
                speedRates[moveType] = 1.0f;

            if (!IsOwner)
            {
                EntityState.AddCallback(nameof(EntityState.DeathState), OnDeathStateChanged);
                EntityState.AddCallback(nameof(EntityState.Health), OnHealthStateChanged);
                EntityState.AddCallback(nameof(EntityState.TargetId), OnTargetIdChanged);
                EntityState.AddCallback(nameof(EntityState.Faction), OnFactionChanged);
            }
            else
                visibleAuraController.HandleUnitAttach(this);

            ThreatManager = new ThreatManager(this);
            MovementInfo.Attached(EntityState, this);

            SpellHistory = new SpellHistory(this);
            SpellCast = new SpellCast(this);

            SetMap(WorldManager.FindMap(1));

            WorldManager.UnitManager.EventEntityDetach += OnEntityDetach;
        }

        protected virtual void HandleDetach()
        {
            WorldManager.UnitManager.EventEntityDetach -= OnEntityDetach;

            while (ownedAuras.Count > 0)
                RemoveOwnedAura(ownedAuras[0], AuraRemoveMode.Detach);

            while (auraApplications.Count > 0)
                RemoveAura(auraApplications[0], AuraRemoveMode.Detach);

            Assert.IsTrue(auraApplicationsByAuraState.Count == 0);
            Assert.IsTrue(auraEffectsByAuraType.Count == 0);
            Assert.IsTrue(ownedAurasById.Count == 0);
            Assert.IsTrue(auraApplicationsByAuraId.Count == 0);
            Assert.IsTrue(interruptableAuraApplications.Count == 0);
            Assert.IsTrue(auraApplications.Count == 0);
            Assert.IsTrue(auraApplicationSet.Count == 0);
            Assert.IsTrue(ownedAuras.Count == 0);

            if (IsOwner)
                visibleAuraController.HandleUnitDetach();

            SpellHistory.Detached();
            SpellCast.Detached();

            EntityState.RemoveAllCallbacks();

            ResetMap();

            ThreatManager.Detached();
            WorldManager.UnitManager.Detach(this);
            MovementInfo.Detached();

            createToken = null;
        }

        internal override void DoUpdate(int deltaTime)
        {
            base.DoUpdate(deltaTime);

            SpellHistory.DoUpdate(deltaTime);
            Controller.DoUpdate();

            for (int i = 0; i < ownedAuras.Count; i++)
            {
                Aura auraToUpdate = ownedAuras[i];
                if (auraToUpdate.Updated)
                    continue;

                auraToUpdate.DoUpdate(deltaTime);

                if (auraToUpdate.IsExpired)
                    RemoveOwnedAura(auraToUpdate, AuraRemoveMode.Expired);

                if (i >= ownedAuras.Count || auraToUpdate != ownedAuras[i])
                    i = 0;
            }

            for (int i = 0; i < ownedAuras.Count; i++)
                ownedAuras[i].LateUpdate();

            if (IsOwner)
                visibleAuraController.DoUpdate();
        }

        internal void UpdateTarget(ulong newTargetId = UnitUtils.NoTargetId, Unit newTarget = null, bool updateState = false)
        {
            targetId = newTarget?.Id ?? newTargetId;
            Target = newTarget ?? WorldManager.UnitManager.Find(targetId);

            if (updateState)
                EntityState.TargetId = Target?.BoltEntity.NetworkId ?? default;

            EventHandler.ExecuteEvent(this, GameEvents.UnitTargetChanged);
        }

        internal void AddState(UnitState state) { UnitState |= state; }

        internal bool HasState(UnitState state) { return (UnitState & state) != 0; }

        internal void RemoveState(UnitState state) { UnitState &= ~state; }

        internal void SetFlag(UnitFlags flag) => unitFlags |= flag;

        internal void RemoveFlag(UnitFlags flag) => unitFlags &= ~flag;

        internal bool HasFlag(UnitFlags flag) => (unitFlags & flag) == flag;

        internal int ModifyHealth(int delta)
        {
            return SetHealth(Health + delta);
        }

        internal int SetHealth(int value)
        {
            int delta = health.Set(Mathf.Clamp(value, 0, maxHealth.Value));
            EntityState.Health = health.Value;
            return delta;
        }

        internal void UpdateSpeed(UnitMoveType type)
        {
            float increaseModifier = 0.0f;
            float nonStackModifier = 100f;
            float stackMultiplier = 100f;
            
            // increases only affect running movement
            switch (type)
            {
                case UnitMoveType.RunBack:
                    break;
                case UnitMoveType.Walk:
                    break;
                case UnitMoveType.Run:
                    increaseModifier = MaxPositiveAuraModifier(AuraEffectType.SpeedIncreaseModifier);
                    nonStackModifier += MaxPositiveAuraModifier(AuraEffectType.SpeedNonStackableModifier);
                    stackMultiplier = 100.0f * TotalAuraMultiplier(AuraEffectType.SpeedStackableMultiplier);
                    break;
                default:
                    return;
            }

            // calculate increased speed
            float speedRate = 1.0f.ApplyPercentage(Mathf.Max(nonStackModifier, stackMultiplier));
            if (!Mathf.Approximately(increaseModifier, 0.0f))
                speedRate = speedRate.ApplyPercentage(100.0f + increaseModifier);

            if (!HasAuraType(AuraEffectType.SpeedSupressSlowEffects))
            {
                // apply strongest slow effect
                float slowPercent = Mathf.Clamp(MaxPositiveAuraModifier(AuraEffectType.SpeedDecreaseModifier), 0.0f, 99.9f);
                if (slowPercent > 0.0f)
                    speedRate = speedRate.ApplyPercentage(100.0f - slowPercent);
            }

            // check for minimum speed aura
            float minSpeedPercent = MaxPositiveAuraModifier(AuraEffectType.ModMinimumSpeed);
            if (minSpeedPercent > 0)
                speedRate = Mathf.Max(speedRate, 1.0f.ApplyPercentage(minSpeedPercent));

            SetSpeedRate(type, speedRate);
        }

        internal void SetSpeedRate(UnitMoveType type, float rate)
        {
            if (rate < 0)
                rate = 0.0f;

            if (!Mathf.Approximately(speedRates[type], rate))
            {
                speedRates[type] = rate;

                if (IsOwner && this is Player player)
                    EventHandler.ExecuteEvent(EventHandler.GlobalDispatcher, GameEvents.ServerPlayerSpeedChanged, player, type, rate);
            }
        }

        #region Spell Handling

        internal SpellCastResult CastSpell(SpellInfo spellInfo, SpellCastingOptions castOptions)
        {
            Spell spell = new Spell(this, spellInfo, castOptions);

            SpellCastResult castResult = spell.Prepare();
            if (castResult != SpellCastResult.Success)
            {
                WorldManager.SpellManager.Remove(spell);
                return castResult;
            }

            switch (spell.ExecutionState)
            {
                case SpellExecutionState.Casting:
                    SpellCast.HandleSpellCast(spell, SpellCast.HandleMode.Started);
                    break;
                case SpellExecutionState.Processing:
                    return castResult;
                case SpellExecutionState.Completed:
                    return castResult;
            }
            
            return SpellCastResult.Success;
        }

        internal int DamageBySpell(SpellCastDamageInfo damageInfoInfo)
        {
            Unit victim = damageInfoInfo.Target;
            if (victim == null || !victim.IsAlive)
                return 0;

            EventHandler.ExecuteEvent(EventHandler.GlobalDispatcher, GameEvents.SpellDamageDone, this, victim, damageInfoInfo.Damage, damageInfoInfo.HitInfo == HitType.CriticalHit);

            return DealDamage(victim, damageInfoInfo.Damage);
        }

        internal int HealBySpell(Unit target, SpellInfo spellInfo, int healAmount, bool critical = false)
        {
            return DealHeal(target, healAmount);
        }

        internal int DealDamage(Unit target, int damageAmount)
        {
            if (damageAmount < 1)
                return 0;

            int healthValue = target.Health;
            if (healthValue <= damageAmount)
            {
                Kill(target);
                return healthValue;
            }

            return target.ModifyHealth(-damageAmount);
        }

        internal int DealHeal(Unit target, int healAmount)
        {
            if(healAmount < 1)
                return 0;

            return target.ModifyHealth(healAmount);
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
            DeathState = newState;

            if (IsDead && SpellCast.IsCasting)
                SpellCast.Cancel();
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

        internal float GetSpellMinRangeForTarget(Unit target, SpellInfo spellInfo)
        {
            if (Mathf.Approximately(spellInfo.MinRangeFriend, spellInfo.MinRangeHostile))
                return spellInfo.GetMinRange(false);
            if (target == null)
                return spellInfo.GetMinRange(true);
            return spellInfo.GetMinRange(!IsHostileTo(target));
        }

        internal float GetSpellMaxRangeForTarget(Unit target, SpellInfo spellInfo)
        {
            if (Mathf.Approximately(spellInfo.MaxRangeFriend, spellInfo.MaxRangeHostile))
                return spellInfo.GetMaxRange(false);
            if (target == null)
                return spellInfo.GetMaxRange(true);
            return spellInfo.GetMaxRange(!IsHostileTo(target));
        }

        internal void ModifyAuraState(AuraStateType flag, bool apply) { }

        internal bool HasAuraState(AuraStateType flag, SpellInfo spellProto = null, Unit caster = null) { return false; }

        internal Unit GetMagicHitRedirectTarget(Unit victim, SpellInfo spellProto) { return null; }

        internal Unit GetMeleeHitRedirectTarget(Unit victim, SpellInfo spellProto = null) { return null; }

        internal int SpellBaseDamageBonusDone(SpellSchoolMask schoolMask) { return 0; }

        internal int SpellBaseDamageBonusTaken(SpellSchoolMask schoolMask) { return 0; }

        internal int SpellDamageBonusDone(Unit victim, SpellInfo spellProto, int damage, SpellDamageType damagetype, SpellEffectInfo effect, uint stack = 1) { return 0; }

        internal float SpellDamagePctDone(Unit victim, SpellInfo spellProto, SpellDamageType damagetype) { return 0.0f; }

        internal void ApplySpellMod(SpellInfo spellInfo, SpellModifierType modifierType, ref int value) { }

        internal void ApplySpellMod(SpellInfo spellInfo, SpellModifierType modifierType, ref float value) { }

        internal int SpellDamageBonusTaken(Unit caster, SpellInfo spellProto, int damage, SpellDamageType damagetype, SpellEffectInfo effect, uint stack = 1)
        {
            return damage;
        }

        internal int SpellBaseHealingBonusDone(SpellSchoolMask schoolMask) { return 0; }

        internal int SpellBaseHealingBonusTaken(SpellSchoolMask schoolMask) { return 0; }

        internal uint SpellHealingBonusDone(Unit victim, SpellInfo spellProto, uint healamount, SpellDamageType damagetype, SpellEffectInfo effect, uint stack = 1) { return 0; }

        internal uint SpellHealingBonusTaken(Unit caster, SpellInfo spellProto, uint healamount, SpellDamageType damagetype, SpellEffectInfo effect, uint stack = 1) { return 0; }

        internal float SpellHealingPercentDone(Unit victim, SpellInfo spellProto) { return 0.0f; }

        internal bool IsSpellBlocked(Unit victim, SpellInfo spellProto, WeaponAttackType attackType = WeaponAttackType.BaseAttack) { return false; }

        internal bool IsSpellCrit(Unit victim, SpellInfo spellProto, SpellSchoolMask schoolMask, WeaponAttackType attackType = WeaponAttackType.BaseAttack) { return false; }

        internal float GetUnitSpellCriticalChance(Unit victim, SpellInfo spellProto, SpellSchoolMask schoolMask, WeaponAttackType attackType = WeaponAttackType.BaseAttack) { return 0.0f; }

        internal int SpellCriticalHealingBonus(SpellInfo spellProto, int damage, Unit victim) { return 0; }

        internal void ApplySpellDispelImmunity(SpellInfo spellProto, SpellDispelType spellDispelType, bool apply) { }

        internal bool IsImmunedToDamage(SpellSchoolMask meleeSchoolMask) { return false; }

        internal bool IsImmunedToDamage(SpellInfo spellProto) { return false; }

        internal bool IsImmuneToSpell(SpellInfo spellInfo, Unit caster) { return false; }

        internal bool IsImmuneToAura(AuraInfo auraInfo, Unit caster) { return false; }

        internal bool IsImmuneToAuraEffect(AuraEffectInfo auraEffect, Unit caster) { return false; }

        internal uint CalcSpellResistance(Unit victim, SpellSchoolMask schoolMask, SpellInfo spellProto) { return 0; }

        internal void CalcAbsorbResist(Unit victim, SpellSchoolMask schoolMask, SpellDamageType damagetype, int damage, ref int absorb, ref int resist, SpellInfo spellProto = null) { }

        internal void CalcHealAbsorb(Unit victim, SpellInfo spellProto, ref int healAmount, ref int absorb) { }

        internal float ApplyEffectModifiers(SpellInfo spellProto, int effectIndex, float value)
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

        internal int ModifyAuraDuration(AuraInfo auraInfo, Unit target, int duration) { return duration; }

        internal void ModSpellCastTime(SpellInfo spellProto, ref int castTime, Spell spell = null) { }

        internal void ModSpellDurationTime(SpellInfo spellProto, ref int castTime, Spell spell = null) { }

        #endregion

        internal bool HasAuraType(AuraEffectType auraEffectType)
        {
            return auraEffectsByAuraType.ContainsKey(auraEffectType);
        }

        internal float TotalAuraModifier(AuraEffectType auraType)
        {
            if (!auraEffectsByAuraType.TryGetValue(auraType, out List<AuraEffect> auraEffects))
                return 0.0f;

            float modifier = 0.0f;

            foreach (AuraEffect auraEffect in auraEffects)
                modifier += auraEffect.Value;

            return modifier;
        }

        internal float TotalAuraMultiplier(AuraEffectType auraType)
        {
            if (!auraEffectsByAuraType.TryGetValue(auraType, out List<AuraEffect> auraEffects))
                return 1.0f;

            float multiplier = 1.0f;

            foreach (AuraEffect auraEffect in auraEffects)
                multiplier = multiplier.AddPercentage(auraEffect.Value);

            return multiplier;
        }

        internal float MaxPositiveAuraModifier(AuraEffectType auraType)
        {
            if (!auraEffectsByAuraType.TryGetValue(auraType, out List<AuraEffect> auraEffects))
                return 0.0f;

            float modifier = 0.0f;

            foreach (AuraEffect auraEffect in auraEffects)
                modifier = Mathf.Max(modifier, auraEffect.Value);

            return modifier;
        }

        internal float MaxNegativeAuraModifier(AuraEffectType auraType)
        {
            if (!auraEffectsByAuraType.TryGetValue(auraType, out List<AuraEffect> auraEffects))
                return 0.0f;

            float modifier = 0.0f;

            foreach (AuraEffect auraEffect in auraEffects)
                modifier = Mathf.Min(modifier, auraEffect.Value);

            return modifier;
        }

        internal void RefreshOrCreateAura(AuraInfo auraInfo, Unit originalCaster)
        {
            var ownedAura = FindOwnedAura();

            if (ownedAura != null && ownedAura.Info.HasAttribute(AuraAttributes.StackSameAuraInMultipleSlots))
                ownedAura = null;

            if (ownedAura == null)
            {
                ownedAura = new Aura(auraInfo, this, originalCaster);
                AddOwnedAura(ownedAura);
            }

            if (ownedAura.IsRemoved)
                return;

            int duration = ownedAura.MaxDuration;
            duration = originalCaster.ModifyAuraDuration(ownedAura.Info, this, duration);

            if (duration != ownedAura.Duration)
                ownedAura.UpdateDuration(duration, duration);

            ownedAura.UpdateTargets();

            Aura FindOwnedAura()
            {
                if (ownedAurasById.TryGetValue(auraInfo.Id, out List<Aura> ownedAuraList))
                    foreach (Aura aura in ownedAuraList)
                        if (aura.CasterId == originalCaster.Id)
                            return aura;

                return null;
            }
        }

        internal void ApplyAuraApplication(AuraApplication auraApplication)
        {
            Logging.LogAura($"Applying application for target: {Name} for aura: {auraApplication.Aura.Info.name}");

            RemoveNonStackableAuras(auraApplication.Aura);

            auraApplications.Add(auraApplication);
            auraApplicationSet.Add(auraApplication);
            auraApplicationsByAuraId.Insert(auraApplication.Aura.Info.Id, auraApplication);

            HandleStateContainingAura(auraApplication, true);
            HandleInterruptableAura(auraApplication, true);
            HandleAuraEffects(auraApplication, true);

            auraApplication.Aura.RegisterForTarget(this, auraApplication);
            visibleAuraController.HandleAuraApplication(auraApplication, true);
        }

        internal void UnapplyAuraApplication(AuraApplication auraApplication, AuraRemoveMode removeMode)
        {
            auraApplicationsByAuraId.Delete(auraApplication.Aura.Info.Id, auraApplication);
            auraApplications.Remove(auraApplication);
            auraApplicationSet.Remove(auraApplication);

            HandleInterruptableAura(auraApplication, false);
            HandleStateContainingAura(auraApplication, false);
            HandleAuraEffects(auraApplication, false);

            auraApplication.Aura.UnregisterForTarget(this, auraApplication);
            auraApplication.RemoveMode = removeMode;
            visibleAuraController.HandleAuraApplication(auraApplication, false);

            Logging.LogAura($"Unapplied application for target: {Name} for aura: {auraApplication.Aura.Info.name}");
        }
        
        private void AddOwnedAura(Aura aura)
        {
            ownedAuras.Add(aura);
            ownedAurasById.Insert(aura.Info.Id, aura);

            RemoveNonStackableAuras(aura);

            Logging.LogAura($"Added owned aura {aura.Info.name} for target: {Name}");
        }

        private void RemoveOwnedAura(Aura aura, AuraRemoveMode removeMode)
        {
            ownedAuras.Remove(aura);
            ownedAurasById.Delete(aura.Info.Id, aura);

            aura.Remove(removeMode);

            Logging.LogAura($"Removed owned aura {aura.Info.name} for target: {Name} with mode: {removeMode}");
        }

        private void HandleInterruptableAura(AuraApplication auraApplication, bool added)
        {
            if (!auraApplication.Aura.Info.HasInterruptFlags)
                return;

            if (added)
            {
                interruptableAuraApplications.Add(auraApplication);
                auraInterruptFlags |= auraApplication.Aura.Info.InterruptFlags;
            }
            else
            {
                interruptableAuraApplications.Remove(auraApplication);

                auraInterruptFlags = 0;
                foreach (AuraApplication interruptableAura in interruptableAuraApplications)
                    auraInterruptFlags |= interruptableAura.Aura.Info.InterruptFlags;
            }
        }

        private void HandleStateContainingAura(AuraApplication auraApplication, bool added)
        {
            AuraStateType stateType = auraApplication.Aura.Info.StateType;
            if (stateType == AuraStateType.None)
                return;

            if (added)
            {
                auraApplicationsByAuraState.Insert(stateType, auraApplication);
                ModifyAuraState(stateType, true);
            }
            else
            {
                auraApplicationsByAuraState.Delete(stateType, auraApplication);
                ModifyAuraState(stateType, auraApplicationsByAuraState.ContainsKey(stateType));
            }
        }

        private void HandleAuraEffects(AuraApplication auraApplication, bool added)
        {
            if (added)
            {
                for (int i = 0; i < auraApplication.Aura.Effects.Count; i++)
                {
                    if (auraApplication.EffectsToApply.HasBit(i) && !auraApplication.AppliedEffectMask.HasBit(i))
                    {
                        auraEffectsByAuraType.Insert(auraApplication.Aura.Effects[i].EffectInfo.AuraEffectType, auraApplication.Aura.Effects[i]);
                        tempAuraEffectsToHandle.Add(i);
                    }
                }
            }
            else
            {
                for (int i = 0; i < auraApplication.Aura.EffectsInfos.Count; i++)
                {
                    if (auraApplication.AppliedEffectMask.HasBit(i))
                    {
                        auraEffectsByAuraType.Delete(auraApplication.Aura.Effects[i].EffectInfo.AuraEffectType, auraApplication.Aura.Effects[i]);
                        tempAuraEffectsToHandle.Add(i);
                    }
                }
            }

            for (int i = 0; i < tempAuraEffectsToHandle.Count; i++)
                auraApplication.HandleEffect(tempAuraEffectsToHandle[i], added, tempAuraHandleGroups);

            tempAuraEffectsToHandle.Clear();
            tempAuraHandleGroups.Clear();
        }

        private void RemoveNonStackableAuras(Aura aura)
        {
            for (int i = 0; i < auraApplications.Count; i++)
            {
                if (!auraApplications[i].Aura.CanStackWith(aura))
                {
                    RemoveAura(auraApplications[i], AuraRemoveMode.Default);
                    i = 0;
                }
            }
        }

        private void RemoveAura(AuraApplication application, AuraRemoveMode mode)
        {
            if (auraApplicationSet.Contains(application))
            {
                UnapplyAuraApplication(application, mode);

                if (application.Aura.Owner == this)
                    RemoveOwnedAura(application.Aura, mode);
            }
        }

        private void OnEntityDetach(Unit entity)
        {
            if (targetId == entity.Id || Target == entity)
                UpdateTarget(updateState: true);
        }

        #region State Events

        private void OnDeathStateChanged()
        {
            DeathState = (DeathState)EntityState.DeathState;
        }

        private void OnHealthStateChanged()
        {
            SetHealth(EntityState.Health);
        }

        private void OnTargetIdChanged()
        {
            UpdateTarget(EntityState.TargetId.PackedValue);
        }

        private void OnFactionChanged()
        {
            Faction = Balance.FactionsById[EntityState.Faction.Id];
            FreeForAll = EntityState.Faction.FreeForAll;

            EventHandler.ExecuteEvent(this, GameEvents.UnitFactionChanged);
        }

        #endregion
    }
}
