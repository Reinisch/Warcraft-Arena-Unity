using Common;
using JetBrains.Annotations;
using UnityEngine;

namespace Core
{
    [UsedImplicitly, CreateAssetMenu(fileName = "Effect Resurrect", menuName = "Game Data/Spells/Effects/Resurrect", order = 3)]
    public class EffectResurrect : SpellEffectInfo
    {
        [SerializeField, UsedImplicitly, Header("Resurrect")] private int healthPercent = 20;

        public int HealthPercent => healthPercent;

        public override float Value => HealthPercent;
        public override SpellEffectType EffectType => SpellEffectType.Resurrect;

        internal override void Handle(Spell spell, int effectIndex, Unit target, SpellEffectHandleMode mode)
        {
            spell.EffectResurrect(this, target, mode);
        }
    }

    public partial class Spell
    {
        internal void EffectResurrect(EffectResurrect effect, Unit target, SpellEffectHandleMode mode)
        {
            if (mode != SpellEffectHandleMode.HitFinal || target == null || target.IsAlive)
                return;

            Caster.DealHeal(target, target.MaxHealth.CalculatePercentage(effect.HealthPercent));
            target.ModifyDeathState(DeathState.Alive);
        }
    }
}
