using JetBrains.Annotations;
using UnityEngine;

namespace Core
{
    [UsedImplicitly, CreateAssetMenu(fileName = "Effect Add Combo Points", menuName = "Game Data/Spells/Effects/Add Combo Points", order = 1)]
    public class EffectAddComboPoints : SpellEffectInfo
    {
        [SerializeField, UsedImplicitly, Header("Add Combo Points")]
        private int comboPoints;

        public int ComboPoints => comboPoints;

        public override float Value => comboPoints;
        public override SpellEffectType EffectType => SpellEffectType.AddComboPoints;

        internal override void Handle(Spell spell, int effectIndex, Unit target, SpellEffectHandleMode mode)
        {
            spell.EffectAddComboPoints(this, target, mode);
        }
    }

    public partial class Spell
    {
        internal void EffectAddComboPoints(EffectAddComboPoints effect, Unit target, SpellEffectHandleMode mode)
        {
            if (mode != SpellEffectHandleMode.HitFinal)
                return;

            target.ModifyComboPoints(effect.ComboPoints);
        }
    }
}
