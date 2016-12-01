using System;

using Microsoft.Xna.Framework;

using BasicRpgEngine.Characters;

namespace BasicRpgEngine
{
    public abstract class TextEvent
    {
        protected float floatingSpeed;

        public float Offset { get; protected set; }
        public TimeSpan FullDuration { get; protected set; }
        public TimeSpan FloatingTime { get; protected set; }
        public ITargetable Target { get; protected set; }

        public TextEvent(float time, ITargetable target, float distance)
        {
            Offset = 0;
            FloatingTime = TimeSpan.FromSeconds(time);
            FullDuration = FloatingTime;
            Target = target;
            floatingSpeed = distance / time;
        }

        public abstract void Update(GameTime gameTime);
    }
}