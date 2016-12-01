using System;

using BasicRpgEngine.Characters;
using BasicRpgEngine.Spells;

namespace BasicRpgEngine.Spells
{
    public class HealEffect : BaseEffect
    {
        int minValue;
        int maxValue;
        int modifier;

        public HealEffect(int MinValue, int MaxValue, int Modifier)
            : base(AoeMode.None)
        {
            modifier = Modifier;
            minValue = MinValue;
            maxValue = MaxValue;
        }

        public override void Apply(ITargetable caster, ITargetable target, TimeSpan elapsedTime, SpellModificationInformation spellInfo, NetworkPlayerInterface playerUi)
        {
            int amount = modifier;

            amount += Mechanics.Roll(minValue, maxValue);

            if (amount < 1)
                amount = 1;

            target.Character.Entity.Health.Increase((ushort)amount);
        }

        public override object Clone()
        {
            return new HealEffect(minValue, maxValue, modifier);
        }
    }
}