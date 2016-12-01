using System;

using Microsoft.Xna.Framework;

using BasicRpgEngine.Characters;

namespace BasicRpgEngine
{
    public class SkillDamageEvent : TextEvent
    {
        public bool IsBinded { get; private set; }
        public bool IsCritical { get; private set; }
        public Vector2 Position { get; private set; }
        public string SkillName { get; private set; }
        public int Damage { get; private set; }
        public string Message { get; private set; }

        public SkillDamageEvent(string skillName, string message, int damage, float time, ITargetable target, float distance, bool isBinded, bool isCritical)
            :base(time, target, distance)
        {
            IsBinded = isBinded;
            IsCritical = isCritical;
            SkillName = skillName;
            Message = message;
            Damage = damage;
            if (!IsBinded)
                Position = target.Position;
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