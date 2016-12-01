using System;
using System.Collections.Generic;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

using BasicRpgEngine.Physics;
using BasicRpgEngine.Characters;

namespace BasicRpgEngine.Graphics
{
    public class AnimatedSprite : ICloneable
    {
        Dictionary<AnimationKey, Animation> animations;
        AnimationKey currentAnimation;
        Texture2D texture;

        public bool PerformAnimation
        { get; set; }
        public int CurrentAnimationFrame
        {
            get { return animations[currentAnimation].CurrentFrame; }
            set { animations[currentAnimation].CurrentFrame = value; }
        }
        public int AnimationFramesCount
        { get { return animations[currentAnimation].FramesCount; } }
        public AnimationKey CurrentAnimation
        {
            get { return currentAnimation; }
            set { currentAnimation = value; }
        }
        public int Width
        {
            get { return (int)(animations[currentAnimation].FrameWidth*Scale); }
        }
        public int Height
        {
            get { return (int)(animations[currentAnimation].FrameHeight*Scale); }
        }
        public int OriginWidth
        {
            get { return (animations[currentAnimation].FrameWidth); }
        }
        public int OriginHeight
        {
            get { return (animations[currentAnimation].FrameHeight); }
        }
        public float Scale
        { get; private set; }

        public AnimatedSprite(Texture2D sprite, Dictionary<AnimationKey, Animation> animationDictionary, float scale)
        {
            Scale = scale;
            texture = sprite;
            PerformAnimation = false;
            animations = new Dictionary<AnimationKey, Animation>();
            foreach (AnimationKey key in animationDictionary.Keys)
                animations.Add(key, (Animation)animationDictionary[key].Clone());
        }

        public void Update(TimeSpan elapsedGameTime)
        {
            if (animations[currentAnimation].Update(elapsedGameTime))
                PerformAnimation = false;
        }
        public void Draw(SpriteBatch spriteBatch, Character character, BoundRectangle boundRect, float rotation = 0, bool isProjectile = false)
        {
            Color color = Color.White;
            int shift = 0;

            if (!isProjectile)
            {
                if (character.Entity.IsInvisible)
                    return;
                if (character.Entity.IsFreezed)
                    color = Color.Blue;
            }

            shift = (int)((0.5f - animations[currentAnimation].CenterShift)
                    * animations[currentAnimation].FrameWidth * Scale);

            if (animations[currentAnimation].IsReverseToLeft)
                shift = -shift;

            spriteBatch.Draw(texture, boundRect.BottomCenter + new Vector2(shift, - Height / 2), animations[currentAnimation].CurrentFrameRect, color, rotation,
                new Vector2(animations[currentAnimation].FrameWidth / 2, animations[currentAnimation].FrameHeight / 2), Scale, animations[currentAnimation].IsReverseToLeft ? SpriteEffects.FlipHorizontally : SpriteEffects.None, 0);
        }

        public void ResetCurrentAnimation()
        {
            animations[CurrentAnimation].Reset();
        }
        public object Clone()
        {
            AnimatedSprite newSprite = new AnimatedSprite(texture, new Dictionary<AnimationKey, Animation>(), Scale);
            foreach (AnimationKey key in animations.Keys)
                newSprite.animations.Add(key, (Animation)animations[key].Clone());
            return newSprite;
        }
    }
}