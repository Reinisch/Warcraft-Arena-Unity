using JetBrains.Annotations;
using UnityEngine;

namespace Core
{
    public abstract class SpellEffectInfo : ScriptableObject
    {
        [Header("Base Effect")]
        [SerializeField, UsedImplicitly] private TargetingType mainTargeting;
        [SerializeField, UsedImplicitly] private TargetingType secondaryTargeting;
        [SerializeField, UsedImplicitly] private SpellExplicitTargetType explicitTargetType;

        [SerializeField, UsedImplicitly] private float minRadius;
        [SerializeField, UsedImplicitly] private float maxRadius;

        private float MinRadius => minRadius;
        private float MaxRadius => maxRadius;

        public abstract float Value { get; }
        public abstract SpellEffectType EffectType { get; }

        public SpellExplicitTargetType ExplicitTargetType => explicitTargetType;
        public TargetingType MainTargeting => mainTargeting;
        public TargetingType SecondaryTargeting => secondaryTargeting;

        internal abstract void Handle(Spell spell, int effectIndex, Unit target, SpellEffectHandleMode mode);

        public bool IsEffect(SpellEffectType effectName)
        {
            return EffectType == effectName;
        }

        public bool IsAura()
        {
            return IsUnitOwnedAuraEffect() || EffectType == SpellEffectType.PersistentAreaAura;
        }

        public bool IsAura(AuraEffectType auraEffect)
        {
            return IsAura();
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
