using JetBrains.Annotations;
using UnityEngine;

namespace Core
{
    [UsedImplicitly, CreateAssetMenu(fileName = "Effect Add Power", menuName = "Game Data/Spells/Effects/Add Power", order = 4)]
    public class EffectAddPower : SpellEffectInfo
    {
        [Header("Add Power")]
        [SerializeField, UsedImplicitly] private SpellPowerType powerType;
        [SerializeField, UsedImplicitly] private int powerAmount;

        public override float Value => powerAmount;
        public override SpellEffectType EffectType => SpellEffectType.AddPower;

        public SpellPowerType PowerType => powerType;

        internal override void Handle(Spell spell, int effectIndex, Unit target, SpellEffectHandleMode mode)
        {
            spell.EffectAddPower(this, target, mode);
        }
    }

    public partial class Spell
    {
        internal void EffectAddPower(EffectAddPower effect, Unit target, SpellEffectHandleMode mode)
        {
            if (mode != SpellEffectHandleMode.HitStart || target == null || !target.IsAlive)
                return;

            target.Attributes.ModifyPower(effect.PowerType, (int)effect.Value);
        }
    }
}
