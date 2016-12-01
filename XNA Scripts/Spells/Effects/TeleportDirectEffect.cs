using System;

using Microsoft.Xna.Framework;

using BasicRpgEngine.Characters;
using BasicRpgEngine.Spells;

namespace BasicRpgEngine.Spells
{
    public class TeleportDirectEffect : BaseEffect
    {
        int horizontalValue;
        int verticalValue;

        public TeleportDirectEffect(int horizontalValue, int verticalValue)
            : base(AoeMode.None)
        {
            this.horizontalValue = horizontalValue;
            this.verticalValue = verticalValue;
        }

        public override void Apply(ITargetable caster, ITargetable target, TimeSpan elapsedTime, SpellModificationInformation spellInfo, NetworkPlayerInterface playerUi)
        {
            Vector2 prevVelocity = target.Velocity;

            target.IsGrounded = false;
            target.Velocity = target.Character.IsDirectedRight ? new Vector2(horizontalValue, verticalValue) : new Vector2(-horizontalValue, verticalValue);
            for (int i = 0; i < target.GameMapRef.MapObjects.Length; i++)
                target.GameMapRef.MapObjects[i].CheckCollision(target, Vector2.Zero);

            target.Position += target.Velocity;
            target.Position = Vector2.Clamp(target.Position, new Vector2(100, -800), new Vector2(target.GameMapRef.MapWidth - 100, target.GameMapRef.MapHeight + 800));

            if (target is NetworkPlayer)
            {
                NetworkPlayer player = target as NetworkPlayer;
                target.Velocity = prevVelocity;
                player.AlignStates();
            }
            else
                target.Velocity = prevVelocity;
        }

        public override object Clone()
        {
            return new TeleportDirectEffect(horizontalValue, verticalValue);
        }
    }
}