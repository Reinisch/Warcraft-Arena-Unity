using Common;
using JetBrains.Annotations;
using UnityEngine;

namespace Core
{
    public abstract class SpellEffectInfo : ScriptableObject
    {
        [SerializeField, UsedImplicitly, Header("Base Effect")] private AuraType auraType;
        [SerializeField, UsedImplicitly] private SpellMechanics mechanic;
        [SerializeField, UsedImplicitly] private TargetingType mainTargeting;
        [SerializeField, UsedImplicitly] private TargetingType secondaryTargeting;

        [SerializeField, UsedImplicitly] private int basePoints;
        [SerializeField, UsedImplicitly] private int randomPoints;
        [SerializeField, UsedImplicitly] private int applyAuraPeriod;
        [SerializeField, UsedImplicitly] private float minRadius;
        [SerializeField, UsedImplicitly] private float maxRadius;
        [SerializeField, UsedImplicitly] private float amplitude;
        [SerializeField, UsedImplicitly] private float chainAmplitude;

        private float MinRadius => minRadius;
        private float MaxRadius => maxRadius;
        private float Amplitude => amplitude;
        private float ChainAmplitude => chainAmplitude;

        protected SpellInfo SpellInfo { get; private set; }
        protected int RandomPoints => randomPoints;

        public abstract SpellEffectType EffectType { get; }
        public abstract SpellTargetEntities TargetEntityType { get; }
        public abstract SpellExplicitTargetType ExplicitTargetType { get; }

        public TargetingType MainTargeting => mainTargeting;
        public TargetingType SecondaryTargeting => secondaryTargeting;
        public AuraType AuraType => auraType;
        public SpellMechanics Mechanic => mechanic;

        public int BasePoints => basePoints;
        public int ApplyAuraPeriod => applyAuraPeriod;
        public int Index { get; private set; }
        public SpellCastTargetFlags ImplicitTargetFlags { get; private set; }

        internal void Initialize(SpellInfo spellInfo)
        {
            SpellInfo = spellInfo;
            Index = spellInfo.Effects.IndexOf(this);
            ImplicitTargetFlags = MainTargeting.TargetEntities.TargetFlags() | SecondaryTargeting.TargetEntities.TargetFlags();

            Assert.AreNotEqual(Index, -1);
        }

        internal void Deinitialize()
        {
            SpellInfo = null;
            Index = 0;
        }

        internal virtual void Handle(Spell spell, Unit target, SpellEffectHandleMode mode)
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

        public bool IsUnitOwnedAuraEffect()
        {
            return IsAreaAuraEffect() || EffectType == SpellEffectType.ApplyAura;
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
            if (caster != null && spell != null)
            {
                caster.ApplySpellMod(spell.SpellInfo, SpellModifierType.Radius, ref radius);
                radius = Mathf.Clamp(radius, MinRadius, MaxRadius);
            }

            return radius;
        }
    }
}
