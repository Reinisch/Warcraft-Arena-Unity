using JetBrains.Annotations;
using UnityEngine;

namespace Core
{
    public class EffectApplyAura : SpellEffectInfo
    {
        [SerializeField, UsedImplicitly, Header("Apply Aura")] private AuraInfo auraInfo;

        internal AuraInfo AuraInfo => auraInfo;

        public override float Value => 1.0f;
        public override SpellEffectType EffectType => SpellEffectType.Resurrect;

        internal override void Handle(Spell spell, int effectIndex, Unit target, SpellEffectHandleMode mode)
        {
            spell.EffectApplyAura(this, target, mode);
        }
    }

    public partial class Spell
    {
        internal void EffectApplyAura(EffectApplyAura effect, Unit target, SpellEffectHandleMode mode)
        {
            if (mode != SpellEffectHandleMode.HitTarget || target == null || OriginalCaster == null)
                return;

            target.ApplicationAuraController.RefreshOrCreateAura(effect.AuraInfo, OriginalCaster);
        }
    }
}
