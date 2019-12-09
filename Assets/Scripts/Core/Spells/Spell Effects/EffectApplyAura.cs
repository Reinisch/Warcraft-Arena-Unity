using JetBrains.Annotations;
using UnityEngine;

namespace Core
{
    [UsedImplicitly, CreateAssetMenu(fileName = "Effect Apply Aura", menuName = "Game Data/Spells/Effects/Apply Aura", order = 1)]
    public class EffectApplyAura : SpellEffectInfo
    {
        [SerializeField, UsedImplicitly, Header("Apply Aura")] private AuraInfo auraInfo;

        internal AuraInfo AuraInfo => auraInfo;

        public override float Value => 1.0f;
        public override SpellEffectType EffectType => SpellEffectType.ApplyAura;

        internal override void Handle(Spell spell, int effectIndex, Unit target, SpellEffectHandleMode mode)
        {
            spell.EffectApplyAura(this, target, mode);
        }
    }

    public partial class Spell
    {
        internal void EffectApplyAura(EffectApplyAura effect, Unit target, SpellEffectHandleMode mode)
        {
            if (mode != SpellEffectHandleMode.HitFinal || target == null || OriginalCaster == null)
                return;

            if (target.IsDead && !effect.AuraInfo.HasAttribute(AuraAttributes.DeathPersistent))
                return;

            target.Auras.RefreshOrCreateAura(effect.AuraInfo, SpellInfo, OriginalCaster, this);
        }
    }
}
