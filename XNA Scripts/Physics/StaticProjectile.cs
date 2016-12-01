using System;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

using BasicRpgEngine.Graphics;
using BasicRpgEngine.Characters;

namespace BasicRpgEngine.Physics
{
    public class StaticProjectile : Projectile
    {
        public ITargetable Target { get; set; }

        public StaticProjectile(AnimatedSprite sprite, ITargetable target, Game gameRef)
            :base()
        {
            projectileSprite = (AnimatedSprite)sprite.Clone();
            projectileSprite.CurrentAnimation = AnimationKey.Left;
            Target = target;
            projectileSprite.PerformAnimation = true;
            BoundRect = new BoundRectangle(0, 0, sprite.Width, sprite.Height);
            Position = Target.BoundRect.BottomCenter - new Vector2(projectileSprite.Width / 2, projectileSprite.Height / 2);
        }

        public override bool Update(TimeSpan elapsedGameTime, World world)
        {
            projectileSprite.Update(elapsedGameTime);
            Position = Target.BoundRect.BottomCenter - new Vector2(projectileSprite.Width / 2, projectileSprite.Height / 2);
            if (projectileSprite.PerformAnimation == false)
                NeedsDispose = true;
            return false;
            
        }
        public override void Draw(GameTime gameTime, SpriteBatch spriteBatch)
        {
            if (!Target.Character.Entity.IsInvisible)
                projectileSprite.Draw(spriteBatch, null, boundRect, 0, true);
        }

        public override void Dispose()
        {
            NeedsDispose = true;
            Target = null;
        }
    }
}
