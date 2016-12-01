using System;

using Microsoft.Xna.Framework;

using BasicRpgEngine.Characters;

namespace BasicRpgEngine
{
    public class SkillEvent : TextEvent
    {
        public string SkillName { get; private set; }

        public SkillEvent(string skillName, float time, ITargetable target, float distance)
            :base(time, target, distance)
        {
            SkillName = skillName;
        }

        public override void Update(GameTime gameTime)
        {
            FloatingTime -= gameTime.ElapsedGameTime;
            if (FloatingTime.TotalSeconds <= 0)
            {
                Target = null;
                FloatingTime = TimeSpan.Zero;
            }

            Offset += floatingSpeed * (float)gameTime.ElapsedGameTime.TotalSeconds;
        }
    }
}