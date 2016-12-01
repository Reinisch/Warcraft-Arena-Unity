using System;

using BasicRpgEngine.Graphics;
using BasicRpgEngine.Spells;
using BasicRpgEngine.Physics;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace BasicRpgEngine.Characters
{
    public class Character : IDisposable
    {
        public ITargetable Target { get; set; }
        public SpellCast SpellCast { get; set; }
        public Cooldown GlobalCooldown { get; set;}
        public Entity Entity { get; private set; }
        public AnimatedSprite Sprite { get; private set; }

        public bool IsDirectedRight
        {
            get
            {
                if (Entity.IsModelChanged && Entity.CurrentReplacedModel != null)
                    return Entity.CurrentReplacedModel.CurrentAnimation == AnimationKey.CastRight || Entity.CurrentReplacedModel.CurrentAnimation == AnimationKey.Right
                    || Entity.CurrentReplacedModel.CurrentAnimation == AnimationKey.RunCastRight || Entity.CurrentReplacedModel.CurrentAnimation == AnimationKey.StandRight
                    || Entity.CurrentReplacedModel.CurrentAnimation == AnimationKey.CastingRight || Entity.CurrentReplacedModel.CurrentAnimation == AnimationKey.RunCastingRight;
                else
                    return Sprite.CurrentAnimation == AnimationKey.CastRight || Sprite.CurrentAnimation == AnimationKey.Right
                    || Sprite.CurrentAnimation == AnimationKey.RunCastRight || Sprite.CurrentAnimation == AnimationKey.StandRight
                    || Sprite.CurrentAnimation == AnimationKey.CastingRight || Sprite.CurrentAnimation == AnimationKey.RunCastingRight;
            }
        }

        public Character(Entity entity, AnimatedSprite sprite)
        {
            Entity = entity;
            Sprite = sprite;
            GlobalCooldown = new Cooldown(1);
            Target = null;
        }

        public void Update(TimeSpan elapsedGameTime, bool maybeBuffChanged)
        {
            Entity.Update(elapsedGameTime, maybeBuffChanged);
            if (SpellCast != null)
            {
                if (!SpellCast.Update(Entity, elapsedGameTime))
                {
                    SpellCast.Dispose();
                    SpellCast = null;
                }
            }

            if (Entity.IsModelChanged && Entity.CurrentReplacedModel != null)
                Entity.CurrentReplacedModel.Update(elapsedGameTime);
            else
                Sprite.Update(elapsedGameTime);

            GlobalCooldown.Update(elapsedGameTime);
        }
        public virtual void Draw(SpriteBatch spriteBatch, BoundRectangle boundRect)
        {
            if (Entity.IsModelChanged && Entity.CurrentReplacedModel != null)
            {
                Sprite.PerformAnimation = false;
                Sprite.ResetCurrentAnimation();
                Entity.CurrentReplacedModel.Draw(spriteBatch, this, boundRect);
            }
            else
                Sprite.Draw(spriteBatch, this, boundRect);
        }

        public void Dispose()
        {
            Target = null;
            Entity.Dispose();
            if (SpellCast != null)
                SpellCast.Dispose();
        }
    }
}