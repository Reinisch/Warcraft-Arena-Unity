using System;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

using BasicRpgEngine.Spells;
using BasicRpgEngine.Graphics;
using BasicRpgEngine.Characters;
using BasicRpgEngine;

namespace BasicRpgEngine.Physics
{
    public class TargetingProjectile : Projectile
    {
        public float Speed { get; set; }
        public Spell Spell { get; set; }
        public ITargetable Caster { get; set; }
        public ITargetable Target { get; set; }

        public TargetingProjectile(AnimatedSprite sprite, Spell spell, float speed, ITargetable caster, ITargetable target, Game gameRef)
            :base()
        {
            projectileSprite = (AnimatedSprite)sprite.Clone();
            projectileSprite.CurrentAnimation = AnimationKey.Left;
            Spell = spell;
            BoundRect = new BoundRectangle(0, 0, sprite.Width, sprite.Height);
            if (caster.Character.IsDirectedRight)
                {
                    Position = new Vector2(caster.BoundRect.Right - sprite.Width / 2,
                        caster.Position.Y + caster.BoundRect.Height / 4 - sprite.Height / 2);
                }
                else
                {
                    Position = new Vector2(caster.BoundRect.Left - sprite.Width / 2,
                        caster.Position.Y + caster.BoundRect.Height / 4 - sprite.Height / 2);
                }
            Speed = speed;
            Caster = caster;
            Target = target;
        }

        public override bool Update(TimeSpan elapsedGameTime, World world)
        {
            projectileSprite.Update(elapsedGameTime);
            float distance;
            Vector2 velocity;
            Speed += 0.3f;

            velocity = (Target.BoundRect.Center - BoundRect.Center);
            distance = velocity.Length();
            velocity = Vector2.Normalize(velocity) * Speed;

            if (distance < Speed * 3)
                return true;
            else
            {
                Position += velocity;
                return false;
            }
        }
        public override void Draw(GameTime gameTime, SpriteBatch spriteBatch)
        {
            Vector2 direction = Target.BoundRect.Center - boundRect.Center;
            projectileSprite.Draw(spriteBatch, null, boundRect,
                (float)Math.Atan2(direction.X, -direction.Y) + (float)Math.PI / 2, true);
        }

        public override void Dispose()
        {
            NeedsDispose = true;
            Caster = null;
            Target = null;
            Spell = null;
        }
    }
}