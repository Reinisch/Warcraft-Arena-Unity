using JetBrains.Annotations;
using UnityEngine;

namespace Core
{
    public abstract class SpellEffectInfo : ScriptableObject
    {
        [Header("Base Effect")]
        [SerializeField, UsedImplicitly] private SpellTargeting targeting;

        public virtual bool IgnoresSpellImmunity => false;
        public abstract float Value { get; }
        public abstract SpellEffectType EffectType { get; }
        public SpellTargeting Targeting => targeting;

        internal abstract void Handle(Spell spell, int effectIndex, Unit target, SpellEffectHandleMode mode);

        public bool IsEffect(SpellEffectType effectName)
        {
            return EffectType == effectName;
        }

        public bool IsTargetingArea()
        {
            return Targeting is SpellTargetingArea;
        }

        public bool IsAreaAuraEffect()
        {
            return EffectType == SpellEffectType.ApplyAreaAuraParty || EffectType == SpellEffectType.ApplyAreaAuraRaid || EffectType == SpellEffectType.ApplyAreaAuraFriend ||
                   EffectType == SpellEffectType.ApplyAreaAuraEnemy || EffectType == SpellEffectType.ApplyAreaAuraPet || EffectType == SpellEffectType.ApplyAreaAuraOwner;
        }
    }
}
