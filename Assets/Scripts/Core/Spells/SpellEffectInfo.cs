using System;
using JetBrains.Annotations;
using UnityEngine;
using UnityEngine.Assertions;

namespace Core
{
    public abstract class SpellEffectInfo : ScriptableObject
    {
        [SerializeField, UsedImplicitly] private AuraType auraType;
        [SerializeField, UsedImplicitly] private Mechanics mechanic;
        [SerializeField, UsedImplicitly] private TargetingType mainTargeting;
        [SerializeField, UsedImplicitly] private TargetingType secondaryTargeting;

        [SerializeField, UsedImplicitly] private int basePoints;
        [SerializeField, UsedImplicitly] private int randomPoints;
        [SerializeField, UsedImplicitly] private int applyAuraPeriod;
        [SerializeField, UsedImplicitly] private float minRadius;
        [SerializeField, UsedImplicitly] private float maxRadius;
        [SerializeField, UsedImplicitly] private float amplitude;
        [SerializeField, UsedImplicitly] private float chainAmplitude;

        [NonSerialized] private SpellInfo spellInfo;

        public abstract SpellEffectType EffectType { get; }
        public abstract TargetEntities TargetEntityType { get; }
        public abstract ExplicitTargetTypes ExplicitTargetType { get; }

        public TargetingType MainTargeting => mainTargeting;
        public TargetingType SecondaryTargeting => secondaryTargeting;
        public AuraType AuraType => auraType;
        public Mechanics Mechanic => mechanic;

        public int BasePoints => basePoints;
        public int ApplyAuraPeriod => applyAuraPeriod;
        private int RandomPoints => randomPoints;
        private float MinRadius => minRadius;
        private float MaxRadius => maxRadius;
        private float Amplitude => amplitude;
        private float ChainAmplitude => chainAmplitude;

        public int Index { get; private set; }
        public SpellCastTargetFlags ImplicitTargetFlags { get; private set; }

        public void Initialize(SpellInfo spellInfo)
        {
            this.spellInfo = spellInfo;
            Index = spellInfo.Effects.IndexOf(this);
            ImplicitTargetFlags = MainTargeting.TargetEntities.TargetFlags() | SecondaryTargeting.TargetEntities.TargetFlags();

            Assert.AreNotEqual(Index, -1);
        }

        public void Deinitialize()
        {
            spellInfo = null;
            Index = 0;
        }

        public virtual void Handle(Spell spell, Unit target, SpellEffectHandleMode mode)
        {
            spell.EffectNone(this);
        }

        public bool IsEffect(SpellEffectType effectName)
        {
            return EffectType == effectName;
        }

        public bool IsAura()
        {
            return (IsUnitOwnedAuraEffect() || EffectType == SpellEffectType.PersistentAreaAura) && AuraType != AuraType.None;
        }

        public bool IsAura(AuraType aura)
        {
            return IsAura() && AuraType == aura;
        }

        public bool IsTargetingArea()
        {
            return MainTargeting.IsArea || SecondaryTargeting.IsArea;
        }

        public bool IsAreaAuraEffect()
        {
            return EffectType == SpellEffectType.ApplyAreaAuraParty || EffectType == SpellEffectType.ApplyAreaAuraRaid || EffectType == SpellEffectType.ApplyAreaAuraFriend ||
                   EffectType == SpellEffectType.ApplyAreaAuraEnemy || EffectType == SpellEffectType.ApplyAreaAuraPet || EffectType == SpellEffectType.ApplyAreaAuraOwner;
        }

        public bool IsFarUnitTargetEffect()
        {
            return EffectType == SpellEffectType.SummonPlayer || EffectType == SpellEffectType.SummonRafFriend
                   || EffectType == SpellEffectType.Resurrect || EffectType == SpellEffectType.SkinPlayerCorpse;
        }

        public bool IsUnitOwnedAuraEffect()
        {
            return IsAreaAuraEffect() || EffectType == SpellEffectType.ApplyAura;
        }


        public int CalcValue(Unit caster = null, int basePoints = int.MinValue, Unit target = null)
        {
            basePoints = basePoints == int.MinValue ? BasePoints : basePoints;

            if (Mathf.Abs(RandomPoints) <= 1)
                basePoints += RandomPoints;
            else
                basePoints += RandomPoints > 0 ? RandomHelper.Next(1, RandomPoints + 1) : RandomHelper.Next(RandomPoints, 1);

            float value = basePoints;
            if (caster != null)
                value = caster.ApplyEffectModifiers(spellInfo, Index, value);

            return (int)value;
        }

        public float CalcValueMultiplier(Unit caster, Spell spell = null)
        {
            float multiplier = Amplitude;
            caster?.SpellModOwner?.ApplySpellMod(spellInfo.Id, SpellModOp.ValueMultiplier, ref multiplier, spell);

            return multiplier;
        }

        public float CalcDamageMultiplier(Unit caster, Spell spell = null)
        {
            float multiplierPercent = ChainAmplitude * 100.0f;
            caster?.SpellModOwner?.ApplySpellMod(spellInfo.Id, SpellModOp.DamageMultiplier, ref multiplierPercent, spell);
            return multiplierPercent / 100.0f;
        }


        public bool HasRadius()
        {
            return MinRadius > 0 || MaxRadius > 0;
        }

        public bool HasMaxRadius()
        {
            return MaxRadius > 0;
        }

        public float CalcRadius(Unit caster = null, Spell spell = null)
        {
            if (!HasRadius())
                return 0.0f;

            float radius = MinRadius == 0 ? MaxRadius : MinRadius;
            if (caster != null)
            {
                caster.SpellModOwner?.ApplySpellMod(spellInfo.Id, SpellModOp.Radius, ref radius, spell);
                radius = Mathf.Clamp(radius, MinRadius, MaxRadius);
            }

            return radius;
        }


        public SpellCastTargetFlags GetMissingTargetMask(SpellCastTargetFlags mask = 0)
        {
            SpellCastTargetFlags effectImplicitTargetMask = TargetEntityType.TargetFlags();
            SpellCastTargetFlags providedTargetMask = ImplicitTargetFlags | mask;

            // remove all flags covered by effect target mask
            if ((providedTargetMask & SpellCastTargetFlags.UnitMask) > 0)
                effectImplicitTargetMask &= ~SpellCastTargetFlags.UnitMask;
            if ((providedTargetMask & SpellCastTargetFlags.GameEntity) > 0)
                effectImplicitTargetMask &= ~SpellCastTargetFlags.GameEntity;
            if ((providedTargetMask & SpellCastTargetFlags.DestLocation) > 0)
                effectImplicitTargetMask &= ~SpellCastTargetFlags.DestLocation;
            if ((providedTargetMask & SpellCastTargetFlags.SourceLocation) > 0)
                effectImplicitTargetMask &= ~SpellCastTargetFlags.SourceLocation;

            return effectImplicitTargetMask;
        }
    }
}
