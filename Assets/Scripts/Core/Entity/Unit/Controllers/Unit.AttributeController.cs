using System.Collections.Generic;
using Common;
using UnityEngine;

namespace Core
{
    public abstract partial class Unit
    {
        internal class AttributeController : IUnitBehaviour
        {
            private Unit unit;
            private IUnitState unitState;
            private bool initialized;

            private readonly Dictionary<UnitMoveType, float> speedRates = new Dictionary<UnitMoveType, float>();
            private FactionDefinition faction;
            private DeathState deathState;
            private bool freeForAll;
            private ulong targetId;

            internal EntityAttributeInt Health { get; private set; }
            internal EntityAttributeInt MaxHealth { get; private set; }
            internal EntityAttributeInt Mana { get; private set; }
            internal EntityAttributeInt MaxMana { get; private set; }
            internal EntityAttributeInt Level { get; private set; }
            internal EntityAttributeInt SpellPower { get; private set; }
            internal EntityAttributeFloat ModHaste { get; private set; }
            internal EntityAttributeFloat ModRegenHaste { get; private set; }
            internal EntityAttributeFloat CritPercentage { get; private set; }

            internal Unit Target { get; private set; }
            internal IReadOnlyDictionary<UnitMoveType, float> SpeedRates => speedRates;

            public bool HasClientLogic => true;
            public bool HasServerLogic => true;

            internal FactionDefinition Faction
            {
                get => faction;
                set
                {
                    faction = value;

                    if (unit.IsOwner)
                    {
                        unitState.Faction.Id = value.FactionId;
                        unit.createToken.FactionId = value.FactionId;
                    }
                }
            }

            internal DeathState DeathState
            {
                get => deathState;
                set
                {
                    deathState = value;

                    if (unit.IsOwner)
                    {
                        unitState.DeathState = (int)value;
                        unit.createToken.DeathState = value;
                    }
                }
            }

            internal bool FreeForAll
            {
                get => freeForAll;
                set
                {
                    freeForAll = value;

                    if (unit.IsOwner)
                    {
                        unitState.Faction.FreeForAll = value;
                        unit.createToken.FreeForAll = value;
                    }
                }
            }

            void IUnitBehaviour.DoUpdate(int deltaTime)
            {
            }

            void IUnitBehaviour.HandleUnitAttach(Unit unit)
            {
                this.unit = unit;
                unitState = unit.entityState;

                InitializeAttributes();

                foreach (UnitMoveType moveType in StatUtils.UnitMoveTypes)
                    speedRates[moveType] = 1.0f;

                if (!unit.IsOwner)
                {
                    unit.AddCallback(nameof(IUnitState.DeathState), OnDeathStateChanged);
                    unit.AddCallback(nameof(IUnitState.Health), OnHealthStateChanged);
                    unit.AddCallback(nameof(IUnitState.TargetId), OnTargetIdChanged);
                    unit.AddCallback($"{nameof(IUnitState.Faction)}.{nameof(IUnitState.Faction.Id)}", OnFactionIdChanged);
                    unit.AddCallback($"{nameof(IUnitState.Faction)}.{nameof(IUnitState.Faction.FreeForAll)}", OnFactionFreeForAllChanged);
                }

                unit.World.UnitManager.EventEntityDetach += OnEntityDetach;

                void InitializeAttributes()
                {
                    faction = unit.Balance.DefaultFaction;

                    if (initialized)
                    {
                        Health.Reset();
                        MaxHealth.Reset();
                        Mana.Reset();
                        MaxMana.Reset();
                        Level.Reset();
                        SpellPower.Reset();
                        ModHaste.Reset();
                        ModRegenHaste.Reset();
                        CritPercentage.Reset();
                    }
                    else
                    {
                        initialized = true;

                        Health = new EntityAttributeInt(unit, unit.unitAttributeDefinition.BaseHealth, int.MaxValue, EntityAttributes.Health);
                        MaxHealth = new EntityAttributeInt(unit, unit.unitAttributeDefinition.BaseMaxHealth, int.MaxValue, EntityAttributes.MaxHealth);
                        Mana = new EntityAttributeInt(unit, unit.unitAttributeDefinition.BaseMana, int.MaxValue, EntityAttributes.Power);
                        MaxMana = new EntityAttributeInt(unit, unit.unitAttributeDefinition.BaseMaxMana, int.MaxValue, EntityAttributes.MaxPower);
                        Level = new EntityAttributeInt(unit, 1, int.MaxValue, EntityAttributes.Level);
                        SpellPower = new EntityAttributeInt(unit, unit.unitAttributeDefinition.BaseSpellPower, int.MaxValue, EntityAttributes.SpellPower);
                        ModHaste = new EntityAttributeFloat(unit, 1.0f, float.MaxValue, EntityAttributes.ModHaste);
                        ModRegenHaste = new EntityAttributeFloat(unit, 1.0f, float.MaxValue, EntityAttributes.ModRegenHaste);
                        CritPercentage = new EntityAttributeFloat(unit, unit.unitAttributeDefinition.CritPercentage, float.MaxValue, EntityAttributes.CritPercentage);
                    }
                }
            }

            void IUnitBehaviour.HandleUnitDetach()
            {
                unit.World.UnitManager.EventEntityDetach -= OnEntityDetach;

                if (!unit.IsOwner)
                {
                    unit.RemoveCallback(nameof(IUnitState.DeathState), OnDeathStateChanged);
                    unit.RemoveCallback(nameof(IUnitState.Health), OnHealthStateChanged);
                    unit.RemoveCallback(nameof(IUnitState.TargetId), OnTargetIdChanged);
                    unit.RemoveCallback($"{nameof(IUnitState.Faction)}.{nameof(IUnitState.Faction.Id)}", OnFactionIdChanged);
                    unit.RemoveCallback($"{nameof(IUnitState.Faction)}.{nameof(IUnitState.Faction.FreeForAll)}", OnFactionFreeForAllChanged);
                }

                unitState = null;
                unit = null;
            }
            
            internal void UpdateTarget(ulong newTargetId = UnitUtils.NoTargetId, Unit newTarget = null, bool updateState = false)
            {
                targetId = newTarget?.Id ?? newTargetId;
                Target = newTarget ?? unit.World.UnitManager.Find(targetId);

                if (updateState)
                    unitState.TargetId = Target?.BoltEntity.NetworkId ?? default;

                EventHandler.ExecuteEvent(unit, GameEvents.UnitTargetChanged);
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
                        increaseModifier = unit.MaxPositiveAuraModifier(AuraEffectType.SpeedIncreaseModifier);
                        nonStackModifier += unit.MaxPositiveAuraModifier(AuraEffectType.SpeedNonStackableModifier);
                        stackMultiplier = 100.0f * unit.TotalAuraMultiplier(AuraEffectType.SpeedStackableMultiplier);
                        break;
                    default:
                        return;
                }

                // calculate increased speed
                float speedRate = 1.0f.ApplyPercentage(Mathf.Max(nonStackModifier, stackMultiplier));
                if (!Mathf.Approximately(increaseModifier, 0.0f))
                    speedRate = speedRate.ApplyPercentage(100.0f + increaseModifier);

                if (!unit.HasAuraType(AuraEffectType.SpeedSupressSlowEffects))
                {
                    // apply strongest slow effect
                    float slowPercent = Mathf.Clamp(unit.MaxPositiveAuraModifier(AuraEffectType.SpeedDecreaseModifier), 0.0f, 99.9f);
                    if (slowPercent > 0.0f)
                        speedRate = speedRate.ApplyPercentage(100.0f - slowPercent);
                }

                // check for minimum speed aura
                float minSpeedPercent = unit.MaxPositiveAuraModifier(AuraEffectType.ModMinimumSpeed);
                if (minSpeedPercent > 0)
                    speedRate = Mathf.Max(speedRate, 1.0f.ApplyPercentage(minSpeedPercent));

                UpdateSpeedRate(type, speedRate);
            }

            internal void UpdateSpeedRate(UnitMoveType type, float rate)
            {
                if (rate < 0)
                    rate = 0.0f;

                if (!Mathf.Approximately(speedRates[type], rate))
                {
                    speedRates[type] = rate;

                    if (unit.IsOwner && unit is Player player)
                        EventHandler.ExecuteEvent(EventHandler.GlobalDispatcher, GameEvents.ServerPlayerSpeedChanged, player, type, rate);
                }
            }

            internal int SetHealth(int value)
            {
                int delta = Health.Set(Mathf.Clamp(value, 0, MaxHealth.Value));
                unitState.Health = Health.Value;
                return delta;
            }

            private void OnDeathStateChanged()
            {
                DeathState = (DeathState)unitState.DeathState;
            }

            private void OnHealthStateChanged()
            {
                SetHealth(unitState.Health);
            }

            private void OnTargetIdChanged()
            {
                UpdateTarget(unitState.TargetId.PackedValue);
            }

            private void OnFactionIdChanged()
            {
                Faction = unit.Balance.FactionsById[unitState.Faction.Id];

                EventHandler.ExecuteEvent(unit, GameEvents.UnitFactionChanged);
            }

            private void OnFactionFreeForAllChanged()
            {
                FreeForAll = unitState.Faction.FreeForAll;

                EventHandler.ExecuteEvent(unit, GameEvents.UnitFactionChanged);
            }

            private void OnEntityDetach(Unit entity)
            {
                if (targetId == entity.Id || Target == entity)
                    UpdateTarget(updateState: true);
            }
        }
    }
}
