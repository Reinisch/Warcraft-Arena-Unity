using JetBrains.Annotations;
using UnityEngine;

namespace Core
{
    [UsedImplicitly, CreateAssetMenu(fileName = "Effect Apply Aura With Duration Override", menuName = "Game Data/Spells/Effects/Apply Aura With Duration Override", order = 1)]
    public class EffectApplyAuraDurationOverride : SpellEffectInfo
    {
        [SerializeField, UsedImplicitly, Header("Apply Aura Override")] private AuraInfo auraInfo;
        [SerializeField, UsedImplicitly] private int baseDuration;
        [SerializeField, UsedImplicitly] private int durationPerCombo;

        internal AuraInfo AuraInfo => auraInfo;

        public int BaseDuraiton => baseDuration;
        public int DurationPerCombo => durationPerCombo;

        public override float Value => 1.0f;
        public override SpellEffectType EffectType => SpellEffectType.ApplyAuraOverride;

        internal override void Handle(Spell spell, int effectIndex, Unit target, SpellEffectHandleMode mode)
        {
            spell.EffectApplyAuraOverride(this, target, mode);
        }
    }

    public partial class Spell
    {
        internal void EffectApplyAuraOverride(EffectApplyAuraDurationOverride effect, Unit target, SpellEffectHandleMode mode)
        {
            if (mode != SpellEffectHandleMode.HitFinal || target == null || OriginalCaster == null)
                return;

            if (target.IsDead && !effect.AuraInfo.HasAttribute(AuraAttributes.DeathPersistent))
                return;

            int overrideDuration = effect.BaseDuraiton + ConsumedComboPoints * effect.DurationPerCombo;
            target.Auras.RefreshOrCreateAura(effect.AuraInfo, SpellInfo, OriginalCaster, this, overrideDuration);
        }
    }
}
