using System;

using BasicRpgEngine.Characters;
using BasicRpgEngine.Spells;

namespace BasicRpgEngine.Spells
{
    public class AoeEffect : BaseEffect
    {
        public BaseEffect Effect { get; private set; }
        public int Radius { get; private set; }

        public AoeEffect(BaseEffect baseEffect, int radius, AoeMode aoeMode)
            : base(aoeMode)
        {
            Effect = (BaseEffect)baseEffect.Clone();
            this.Radius = radius;
        }

        public override void Apply(ITargetable caster, ITargetable target, TimeSpan elapsedTime, SpellModificationInformation spellInfo, NetworkPlayerInterface playerUi)
        {
            Effect.Apply(caster, target, elapsedTime, spellInfo, playerUi);
        }

        public override object Clone()
        {
            return new AoeEffect(Effect,Radius,AoeMode);
        }
    }
}