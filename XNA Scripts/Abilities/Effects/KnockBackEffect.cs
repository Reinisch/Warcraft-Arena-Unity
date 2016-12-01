using System;

using Microsoft.Xna.Framework;

using BasicRpgEngine.Characters;
using BasicRpgEngine.Spells;

namespace BasicRpgEngine.Spells
{
    public class KnockBackEffect : BaseEffect
    {
        int horizontalValue;
        int verticalValue;

        public KnockBackEffect(int horizontalValue, int verticalValue)
            : base(AoeMode.None)
        {
            this.horizontalValue = horizontalValue;
            this.verticalValue = verticalValue;
        }

        public override void Apply(ITargetable caster, ITargetable target, TimeSpan elapsedTime, SpellModificationInformation spellInfo, NetworkPlayerInterface playerUi)
        {
            target.IsKnockedBack = true;
            target.IsFlying = true;
            target.IsGrounded = false;
            if (caster.Position.X < target.Position.X)
                target.Velocity = new Vector2(horizontalValue, -verticalValue);
            else
                target.Velocity = new Vector2(-horizontalValue, -verticalValue);

            if (target is NetworkPlayer)
            {
                NetworkPlayer player = target as NetworkPlayer;
                player.AlignStates();
            }
        }

        public override object Clone()
        {
            return new KnockBackEffect(horizontalValue, verticalValue);
        }
    }
}