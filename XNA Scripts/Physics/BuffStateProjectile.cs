using System;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

using BasicRpgEngine.Graphics;
using BasicRpgEngine.Characters;
using BasicRpgEngine.Spells;

namespace BasicRpgEngine.Physics
{
    public enum ProjectileBinding { BottomToBottom, CenterToBottom, CenterToCenter }

    public class BuffStateProjectile : StaticProjectile
    {
        private int loopFrameNumber;
        private int endFrameNumber;
        private ProjectileBinding binding;
        private Buff buffRef;

        public BuffStateProjectile(AnimatedSprite sprite, ITargetable target, Game gameRef, Buff buff, ProjectileBinding bindingPlace)
            :base(sprite, target, gameRef)
        {
            sprite.CurrentAnimation = AnimationKey.Left;
            binding = bindingPlace;
            if (binding == ProjectileBinding.BottomToBottom)
                Position = Target.BoundRect.BottomCenter - new Vector2(projectileSprite.Width / 2, projectileSprite.Height / 2);
            else if (binding == ProjectileBinding.CenterToBottom)
                Position = Target.BoundRect.BottomCenter - new Vector2(projectileSprite.Width, projectileSprite.Height);
            else if (binding == ProjectileBinding.CenterToCenter)
                Position = Target.BoundRect.Center - new Vector2(projectileSprite.Width / 2, projectileSprite.Height / 2);
            sprite.CurrentAnimation = AnimationKey.Right;
            loopFrameNumber = sprite.OriginWidth;
            endFrameNumber = sprite.OriginHeight;
            sprite.CurrentAnimation = AnimationKey.Left;
            buffRef = buff;
        }

        public override bool Update(TimeSpan elapsedGameTime, World world)
        {
            projectileSprite.Update(elapsedGameTime);
            if (projectileSprite.CurrentAnimationFrame == endFrameNumber)
                projectileSprite.CurrentAnimationFrame = loopFrameNumber;

            if (binding == ProjectileBinding.CenterToBottom)
                Position = Target.BoundRect.BottomCenter - new Vector2(projectileSprite.Width / 2, projectileSprite.Height / 2);
            else if (binding == ProjectileBinding.BottomToBottom)
                Position = Target.BoundRect.BottomCenter - new Vector2(projectileSprite.Width / 2, projectileSprite.Height);
            else if (binding == ProjectileBinding.CenterToCenter)
                Position = Target.BoundRect.Center - new Vector2(projectileSprite.Width / 2, projectileSprite.Height / 2);

            if (buffRef.NeedsRemoval == true)
                return true;
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
            buffRef = null;
            Target = null;
        }
    }
}