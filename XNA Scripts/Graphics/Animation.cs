using System;

using Microsoft.Xna.Framework;

namespace BasicRpgEngine.Graphics
{
    public enum AnimationKey { Left, Right, StandLeft, StandRight, CastRight, CastLeft,
        RunCastRight, RunCastLeft, RunCastingRight, RunCastingLeft, CastingRight, CastingLeft }

    [Serializable]
    public class Animation : ICloneable
    {
        Rectangle[] frames;
        TimeSpan frameLength;
        TimeSpan frameTimer;
        int framesPerSecond;
        int currentFrame;
        int frameHeight;
        int frameWidth;
        int rows;

        public bool IsReverseToLeft
        { get; private set; }
        public float CenterShift
        { get; private set; }

        public int FrameWidth
        {
            get { return frameWidth; }
        }
        public int FrameHeight
        {
            get { return frameHeight; }
        }
        public Rectangle CurrentFrameRect
        {
            get { return frames[currentFrame]; }
        }

        public int FramesPerSecond
        {
            get { return framesPerSecond; }
            set
            {
                if (value < 1)
                    framesPerSecond = 1;
                else if (value > 60)
                    framesPerSecond = 60;
                else
                    framesPerSecond = value;
                frameLength = TimeSpan.FromSeconds(1 / (double)framesPerSecond);
            }
        }
        public int FramesCount
        { get { return frames.Length; } }
        public int CurrentFrame
        {
            get { return currentFrame; }
            set
            {
                currentFrame = (int)MathHelper.Clamp(value, 0, frames.Length - 1);
            }
        }

        public Animation(int frameCount, int frameWidth, int frameHeight, int xOffset, int yOffset,
            int framesPerSecond, bool reverseToLeft, float centerShift, int rows)
        {
            this.rows = rows;
            this.frames = new Rectangle[frameCount*rows];
            this.frameWidth = frameWidth;
            this.frameHeight = frameHeight;

            if (centerShift > 1 || centerShift < 0)
                CenterShift = 0.5f;
            else
                CenterShift = centerShift;

            IsReverseToLeft = reverseToLeft;
            FramesPerSecond = framesPerSecond;

            for (int j = 0; j < rows ; j++)
                for (int i = 0; i < frameCount; i++)
                    frames[i + j * frameCount] = new Rectangle(xOffset + (frameWidth * i),
                        yOffset + j * frameHeight, frameWidth, frameHeight);

            Reset();
        }

        public bool Update(TimeSpan elapsedGameTime)
        {
            frameTimer += elapsedGameTime;

            if (frameTimer >= frameLength)
            {
                frameTimer = TimeSpan.Zero;
                if (currentFrame + 1 == frames.Length)
                {
                    currentFrame = 0;
                    return true;
                }
                else
                {
                    currentFrame = currentFrame + 1;
                }
            }
            return false;
        }

        public void Reset()
        {
            currentFrame = 0;
            frameTimer = TimeSpan.Zero;
        }

        public object Clone()
        {
            Animation animationClone = new Animation(0, frameWidth, frameHeight,
                0, 0, framesPerSecond, IsReverseToLeft, CenterShift, rows);
            int frameCount = frames.Length;

            animationClone.frames = new Rectangle[frameCount];
            for (int i = 0; i < frameCount; i++)
                animationClone.frames[i] = frames[i];

            return animationClone;
        }
    }
}