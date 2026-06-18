using System;
using System.Collections.Generic;
using Common;
using UnityEngine;

namespace Core
{
    public abstract partial class Unit
    {
        public class AttributeController : IUnitBehaviour, ILogicBehaviour
        {
            private const int RegenerationSyncTime = 2000;

            private Unit unit;
            private FactionDefinition faction;
            private UnitVisualEffectFlags visualEffectFlags;
            private SpellPowerType displayPowerType;
            private DeathState deathState;
            private MovementMode movementMode;
            private EmoteType emoteType;
            private ClassType classType;
            private bool initialized;
            private bool freeForAll;
            private ulong targetId;
            private float scale;
            private int modelId;
            private float emoteTime;
            private int accumulatedRegenerationTime;
            private int slowFallSpeed;

            private readonly float[] accumulatedRegeneration = new float[UnitUtils.MaxUnitPowers];
            private readonly EntityAttributeInt[,] powers = new EntityAttributeInt[UnitUtils.MaxUnitPowers, 3];
            private readonly Dictionary<UnitMoveType, float> speedRates = new();
            private readonly Dictionary<SpellPowerType, (int, SpellPowerTypeInfo)> spellPowerIndexes = new();
            private readonly Dictionary<(UnitModifierType, StatModifierType), float> statModifiers = new();

            internal EntityAttributeInt Health { get; private set; }
            internal EntityAttributeInt MaxHealth { get; private set; }
            internal EntityAttributeInt ComboPoints { get; private set; }
            internal EntityAttributeInt Level { get; private set; }
            internal EntityAttributeInt SpellPower { get; private set; }
            internal EntityAttributeInt Intellect { get; private set; }
            internal EntityAttributeFloat ModHaste { get; private set; }
            internal EntityAttributeFloat ModRegenHaste { get; private set; }
            internal EntityAttributeFloat CritPercentage { get; private set; }

            internal int OriginalModelId { get; set; }
            internal Unit Target { get; private set; }
            internal IReadOnlyDictionary<UnitMoveType, float> SpeedRates => speedRates;

            internal FactionDefinition Faction
            {
                get => faction;
                set
                {
                    faction = value;

                    unit.EventBus.ExecuteEvent(unit, GameEvents.UnitFactionChanged);
                }
            }

            internal DeathState DeathState
            {
                get => deathState;
                set
                {
                    if (deathState != value)
                    {
                        deathState = value;
                        EventDeathStateChanged?.Invoke();
                    }
                }
            }

            internal ClassType ClassType
            {
                get => classType;
                set
                {
                    classType = value;

                    unit.EventBus.ExecuteEvent(unit, GameEvents.UnitClassChanged);
                }
            }

            internal EmoteType EmoteType
            {
                get => emoteType;
                set
                {
                    emoteType = value;
                    emoteTime = Time.frameCount;

                    EventEmoteStateChanged?.Invoke();
                }
            }

            internal float EmoteTime => emoteTime;
            
            internal int SlowFallSpeed
            {
                get => slowFallSpeed;
                set => slowFallSpeed = value;
            }

            internal MovementMode MovementMode
            {
                get => movementMode;
                set => movementMode = value;
            }

            internal SpellPowerType DisplayPowerType
            {
                get => displayPowerType;
                set
                {
                    displayPowerType = value;

                    unit.EventBus.ExecuteEvent(unit, GameEvents.UnitDisplayPowerChanged);
                }
            }

            internal UnitVisualEffectFlags VisualEffectFlags
            {
                get => visualEffectFlags;
                set
                {
                    visualEffectFlags = value;

                    unit.EventBus.ExecuteEvent(unit, GameEvents.UnitVisualsChanged);
                }
            }

            internal float Scale
            {
                get => scale;
                set
                {
                    scale = value;

                    unit.EventBus.ExecuteEvent(unit, GameEvents.UnitScaleChanged);
                }
            }

            internal int ModelId
            {
                get => modelId;
                set
                {
                    modelId = value;

                    unit.EventBus.ExecuteEvent(unit, GameEvents.UnitModelChanged);
                }
            }

            internal bool FreeForAll
            {
                get => freeForAll;
                set
                {
                    freeForAll = value;

                    unit.EventBus.ExecuteEvent(unit, GameEvents.UnitFactionChanged);
                }
            }

            public bool HasClientLogic => true;
            public bool HasServerLogic => true;

            public event Action EventDeathStateChanged;
            public event Action EventEmoteStateChanged;

            void IUnitBehaviour.DoUpdate(int deltaTime)
            {
                if (EmoteType != EmoteType.None && EmoteType.IsState() && unit.Motion.IsMoving)
                    if (Time.time - unit.EmoteFrame > UnitUtils.EmoteStateMovementFrameThreshold)
                        unit.ModifyEmoteState(EmoteType.None);

                if (unit.IsAlive)
                {
                    accumulatedRegenerationTime += deltaTime;
                    bool timeToUpdateDisplay = accumulatedRegenerationTime >= RegenerationSyncTime;

                    foreach (var powerEntry in spellPowerIndexes)
                    {
                        int oldValue = powers[powerEntry.Value.Item1, 0].Value;
                        int minValue = powers[powerEntry.Value.Item1, 0].Min;
                        int maxValue = powers[powerEntry.Value.Item1, 1].Value;

                        float regeneratedValue = powerEntry.Value.Item2.Regeneration * deltaTime / 1000;
                        regeneratedValue *= unit.Auras.TotalAuraMultiplier(AuraEffectType.ModifyPowerRegenPercent, (int)powerEntry.Key, ComparisonOperator.Equal);

                        accumulatedRegeneration[powerEntry.Value.Item1] += regeneratedValue;
                        int deltaValue = Mathf.FloorToInt(accumulatedRegeneration[powerEntry.Value.Item1]);
                        accumulatedRegeneration[powerEntry.Value.Item1] -= deltaValue;

                        int newValue = Mathf.Clamp(oldValue + deltaValue, minValue, maxValue);
                        if (newValue != oldValue || timeToUpdateDisplay && DisplayPowerType == powerEntry.Key)
                           SetPower(powerEntry.Key, newValue, timeToUpdateDisplay);
                    }

                    if (timeToUpdateDisplay)
                        accumulatedRegenerationTime -= RegenerationSyncTime;
                }
            }

            void IUnitBehaviour.HandleUnitAttach(Unit unit)
            {
                this.unit = unit;

                foreach (UnitMoveType moveType in StatUtils.UnitMoveTypes)
                    speedRates[moveType] = 1.0f;

                foreach (UnitModifierType modType in StatUtils.UnitModTypes)
                {
                    statModifiers[(modType, StatModifierType.BaseValue)] = 0.0f;
                    statModifiers[(modType, StatModifierType.BasePercent)] = 1.0f;
                    statModifiers[(modType, StatModifierType.TotalValue)] = 0.0f;
                    statModifiers[(modType, StatModifierType.TotalPercent)] = 1.0f;
                }

                InitializeAttributes();

                unit.World.UnitManager.EventEntityDetach += OnEntityDetach;

                void InitializeAttributes()
                {
                    faction = unit.Balance.DefaultFaction;

                    if (initialized)
                    {
                        Health.Reset();
                        MaxHealth.Reset();
                        ComboPoints.Reset();
                        Level.Reset();
                        SpellPower.Reset();
                        Intellect.Reset();
                        ModHaste.Reset();
                        ModRegenHaste.Reset();
                        CritPercentage.Reset();
                    }
                    else
                    {
                        initialized = true;

                        Health = new EntityAttributeInt(unit, unit.EventBus, unit.AttributeDefinition.BaseHealth, int.MaxValue, EntityAttributes.Health);
                        MaxHealth = new EntityAttributeInt(unit, unit.EventBus, unit.AttributeDefinition.BaseMaxHealth, int.MaxValue, EntityAttributes.MaxHealth);
                        
                        ComboPoints = new EntityAttributeInt(unit, unit.EventBus, 0, 5, EntityAttributes.ComboPoints);
                        Level = new EntityAttributeInt(unit, unit.EventBus, 1, int.MaxValue, EntityAttributes.Level);
                        SpellPower = new EntityAttributeInt(unit, unit.EventBus, unit.AttributeDefinition.BaseSpellPower, int.MaxValue, EntityAttributes.SpellPower);
                        Intellect = new EntityAttributeInt(unit, unit.EventBus, unit.AttributeDefinition.BaseIntellect, int.MaxValue, EntityAttributes.Intellect);
                        ModHaste = new EntityAttributeFloat(unit, unit.EventBus, 1.0f, float.MaxValue, EntityAttributes.ModHaste);
                        ModRegenHaste = new EntityAttributeFloat(unit, unit.EventBus, 1.0f, float.MaxValue, EntityAttributes.ModRegenHaste);
                        CritPercentage = new EntityAttributeFloat(unit, unit.EventBus, unit.AttributeDefinition.CritPercentage, float.MaxValue, EntityAttributes.CritPercentage);

                        for (int i = 0; i < powers.GetLength(0); i++)
                        {
                            powers[i, 0] = new EntityAttributeInt(unit, unit.EventBus, 0, int.MaxValue, 0);
                            powers[i, 1] = new EntityAttributeInt(unit, unit.EventBus, 0, int.MaxValue, 0);
                            powers[i, 2] = new EntityAttributeInt(unit, unit.EventBus, 0, int.MaxValue, 0);
                        }
                    }

                    statModifiers[(UnitModifierType.Intellect, StatModifierType.BaseValue)] = Intellect.Base;
                }

                UpdateIntellect();
            }

            void IUnitBehaviour.HandleUnitDetach()
            {
                unit.World.UnitManager.EventEntityDetach -= OnEntityDetach;
                VisualEffectFlags = 0;
                unit = null;
            }

            internal int Power(SpellPowerType powerType) => spellPowerIndexes.TryGetValue(powerType, out var i) ? powers[i.Item1, 0].Value : 0;

            internal int MaxPower(SpellPowerType powerType) => spellPowerIndexes.TryGetValue(powerType, out var i) ? powers[i.Item1, 1].Value : 0;

            internal int MaxPowerWithNoMods(SpellPowerType powerType) => spellPowerIndexes.TryGetValue(powerType, out var i) ? powers[i.Item1, 2].Value : 0;

            internal void UpdateTarget(ulong newTargetId = UnitUtils.NoTargetId, Unit newTarget = null, bool updateState = false)
            {
                targetId = newTarget?.Id ?? newTargetId;
                Target = newTarget ?? unit.World.UnitManager.Find(targetId);

                unit.EventBus.ExecuteEvent(unit, GameEvents.UnitTargetChanged);
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

                    if (unit.World.HasServerLogic)
                        unit.EventBus.ExecuteEvent(Common.GameEvents.ServerPlayerSpeedChanged, unit, type, rate);
                }
            }

            internal void HandleStatPercentModifier(UnitModifierType unitModifierType, StatModifierType modifierType, float value, bool apply)
            {
                switch (modifierType)
                {
                    case StatModifierType.BaseValue:
                    case StatModifierType.TotalValue:
                        statModifiers[(unitModifierType, modifierType)] += apply ? value : -value;
                        break;
                    case StatModifierType.BasePercent:
                    case StatModifierType.TotalPercent:
                        statModifiers[(unitModifierType, modifierType)] = StatUtils.ModifyMultiplierPercent(statModifiers[(unitModifierType, modifierType)], value, apply);
                        break;
                    default:
                        throw new ArgumentOutOfRangeException(nameof(modifierType), modifierType, nameof(StatModifierType));
                }

                switch (unitModifierType)
                {
                    case UnitModifierType.Strength:
                        break;
                    case UnitModifierType.Agility:
                        break;
                    case UnitModifierType.Stamina:
                        break;
                    case UnitModifierType.Intellect:
                        UpdateIntellect();
                        break;
                    case UnitModifierType.Mana:
                    case UnitModifierType.Rage:
                    case UnitModifierType.Focus:
                    case UnitModifierType.Energy:
                    case UnitModifierType.ComboPoints:
                    case UnitModifierType.Runes:
                    case UnitModifierType.RunicPower:
                    case UnitModifierType.SoulShards:
                    case UnitModifierType.LunarPower:
                    case UnitModifierType.HolyPower:
                    case UnitModifierType.AlternatePower:
                    case UnitModifierType.Maelstrom:
                    case UnitModifierType.Chi:
                    case UnitModifierType.Insanity:
                    case UnitModifierType.BurningEmbers:
                    case UnitModifierType.DemonicFury:
                    case UnitModifierType.ArcaneCharges:
                    case UnitModifierType.Fury:
                    case UnitModifierType.Pain:
                        UpdateMaxPower(unitModifierType.AsPower());
                        break;
                }
            }

            internal void UpdateIntellect()
            {
                int totalIntellect = CalculateTotalStatValue(UnitModifierType.Intellect);

                SpellPower.Set(SpellPower.Base + totalIntellect);
            }

            internal void UpdateMaxPower(SpellPowerType powerType)
            {
                if (!spellPowerIndexes.TryGetValue(powerType, out (int, SpellPowerTypeInfo) powerTypeInfo))
                    return;

                UnitModifierType unitModifierType = powerType.AsModifier();
                float value = statModifiers[(unitModifierType, StatModifierType.BaseValue)] + powers[powerTypeInfo.Item1, 1].Base;
                value *= statModifiers[(unitModifierType, StatModifierType.BasePercent)];
                value += statModifiers[(unitModifierType, StatModifierType.TotalValue)];
                value *= statModifiers[(unitModifierType, StatModifierType.TotalPercent)];

                SetMaxPower(powerType, (int)Math.Round(value));
            }

            internal void UpdateDisplayPower()
            {
                SpellPowerType newPowerType = unit.ShapeShiftForm == ShapeShiftForm.CatForm
                    ? SpellPowerType.Energy
                    : unit.Balance.ClassesByType[unit.ClassType].MainPowerType;

                DisplayPowerType = newPowerType;

                if (unit.IsOwner)
                {
                    SetMaxPower(DisplayPowerType, MaxPower(DisplayPowerType));

                    SetPower(DisplayPowerType, Power(DisplayPowerType));
                }
            }

            internal void UpdateAvailablePowers()
            {
                ClassInfo unitClassInfo = unit.Balance.ClassesByType[ClassType];
                int maxPowers = powers.GetLength(0);
                int usedPowers = unitClassInfo.PowerTypes.Count;

                Assert.IsTrue(usedPowers <= maxPowers);

                spellPowerIndexes.Clear();
                for (int i = 0; i < usedPowers ; i++)
                {
                    SpellPowerTypeInfo powerTypeInfo = unitClassInfo.PowerTypes[i];
                    powers[i, 0].ModifyAttribute(powerTypeInfo.MaxBasePower, powerTypeInfo.MaxBasePower, powerTypeInfo.MinBasePower, powerTypeInfo.MaxTotalPower, powerTypeInfo.AttributeTypeCurrent);
                    powers[i, 1].ModifyAttribute(powerTypeInfo.MaxBasePower, powerTypeInfo.MaxBasePower, powerTypeInfo.MinBasePower, powerTypeInfo.MaxTotalPower, powerTypeInfo.AttributeTypeMax);
                    powers[i, 2].ModifyAttribute(powerTypeInfo.MaxBasePower, powerTypeInfo.MaxBasePower, powerTypeInfo.MinBasePower, powerTypeInfo.MaxTotalPower, powerTypeInfo.AttributeTypeMaxNoMods);
                    spellPowerIndexes[powerTypeInfo.PowerType] = (i, powerTypeInfo);
                }

                for (int i = usedPowers; i < maxPowers; i++)
                {
                    powers[i, 0].ModifyAttribute(0, 0, 0, 0, 0);
                    powers[i, 1].ModifyAttribute(0, 0, 0, 0, 0);
                    powers[i, 2].ModifyAttribute(0, 0, 0, 0, 0);
                }

                UpdateDisplayPower();
            }

            internal int CalculateTotalStatValue(UnitModifierType modType)
            {
                float value = statModifiers[(modType, StatModifierType.BaseValue)];
                value *= statModifiers[(modType, StatModifierType.BasePercent)];
                value += statModifiers[(modType, StatModifierType.TotalValue)];
                value *= statModifiers[(modType, StatModifierType.TotalPercent)];
                return Mathf.Max(0, (int)value);
            }

            internal void SetHealth(int value)
            {
                Health.Set(Mathf.Clamp(value, 0, MaxHealth.Value));
            }

            internal void SetMaxHealth(int value)
            {
                MaxHealth.Set(value);
            }

            internal void ApplyAttributeDefinition(UnitAttributeDefinition definition)
            {
                MaxHealth.ModifyAttribute(definition.BaseMaxHealth, definition.BaseMaxHealth, 0, int.MaxValue, EntityAttributes.MaxHealth);
                Health.ModifyAttribute(definition.BaseHealth, Mathf.Min(definition.BaseHealth, definition.BaseMaxHealth), 0, int.MaxValue, EntityAttributes.Health);

                Intellect.ModifyAttribute(definition.BaseIntellect, definition.BaseIntellect, 0, int.MaxValue, EntityAttributes.Intellect);
                SpellPower.ModifyAttribute(definition.BaseSpellPower, definition.BaseSpellPower, 0, int.MaxValue, EntityAttributes.SpellPower);
                statModifiers[(UnitModifierType.Intellect, StatModifierType.BaseValue)] = Intellect.Base;
                UpdateIntellect();

                CritPercentage.ModifyAttribute(definition.CritPercentage, definition.CritPercentage, 0f, float.MaxValue, EntityAttributes.CritPercentage);

                unit.EventBus.ExecuteEvent(unit, GameEvents.UnitAttributeChanged, EntityAttributes.MaxHealth);
                unit.EventBus.ExecuteEvent(unit, GameEvents.UnitAttributeChanged, EntityAttributes.Health);
            }

            internal void SetComboPoints(int points)
            {
                ComboPoints.Set(points);
            }

            internal int ModifyPower(SpellPowerType powerType, int delta)
            {
                return SetPower(powerType, Power(powerType) + delta);
            }

            internal int SetPower(SpellPowerType powerType, int newValue, bool updateDisplayPower = true)
            {
                if (spellPowerIndexes.TryGetValue(powerType, out var i))
                {
                    newValue = Mathf.Clamp(newValue, powers[i.Item1, 0].Min, powers[i.Item1, 1].Value);
                    int delta = powers[i.Item1, 0].Set(newValue);

                    if (powerType == DisplayPowerType && updateDisplayPower)
                    {
                        unit.EventBus.ExecuteEvent(unit, GameEvents.UnitAttributeChanged, EntityAttributes.Power);
                    }

                    return delta;
                }

                return 0;
            }

            internal void SetMaxPower(SpellPowerType powerType, int newValue)
            {
                if (spellPowerIndexes.TryGetValue(powerType, out var i))
                {
                    powers[i.Item1, 1].Set(newValue);

                    if (powerType == DisplayPowerType)
                    {
                        unit.EventBus.ExecuteEvent(unit, GameEvents.UnitAttributeChanged, EntityAttributes.MaxPower);
                    }
                }
            }

            internal float Speed(UnitMoveType type) => SpeedRates[type] * unit.Balance.UnitMovementDefinition.BaseSpeedByType(type);

            private void OnEntityDetach(Unit entity)
            {
                if (targetId == entity.Id || Target == entity)
                    UpdateTarget(updateState: true);
            }
        }
    }
}
