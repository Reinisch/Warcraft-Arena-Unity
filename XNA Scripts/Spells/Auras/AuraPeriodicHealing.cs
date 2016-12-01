using System;

using BasicRpgEngine.Characters;
using BasicRpgEngine.Spells;

namespace BasicRpgEngine.Spells
{
    public class AuraPeriodicHealing : AuraBase
    {
        int tickHeal;
        int fullHeal;
        int modifier;
        float seconds;
        int ticks;
        TimeSpan tickTime;
        TimeSpan tickTimeLeft;

        public AuraPeriodicHealing(int fullHeal, int ticks, int modifier, float seconds)
            : base(AuraType.HealOverTime, AuraControlEffect.None, seconds)
        {
            this.fullHeal = fullHeal;
            this.seconds = seconds;
            this.modifier = modifier;
            this.ticks = ticks;
            this.fullHeal = fullHeal;

            tickHeal = (fullHeal + modifier) / ticks;
            tickTime = TimeSpan.FromSeconds(seconds / ticks);
            tickTimeLeft = tickTime;
        }

        public override bool Update(TimeSpan elapsedTime, Entity entity)
        {
            if (TimeLeft == TimeSpan.Zero)
                return true;

            if (Duration != -1)
            {
                TimeLeft -= elapsedTime;
                if (TimeLeft.TotalMilliseconds < 0)
                {
                    TimeLeft = TimeSpan.Zero;
                }
            }

            tickTimeLeft -= elapsedTime;

            if (tickTimeLeft.TotalMilliseconds < 0)
            {
                tickTimeLeft = tickTime + tickTimeLeft;

                int amount = tickHeal;

                entity.Health.Increase((ushort)amount);
            }
            if (TimeLeft == TimeSpan.Zero)
                return true;
            return false;
        }
        public override void Apply(Entity entity)
        { }
        public override void Reverse(Entity entity)
        { }
        public override int Absorb(DamageType damageType, int damageAmount, Entity entity)
        {
            return damageAmount;
        }

        public override object Clone()
        {
            return new AuraPeriodicHealing(fullHeal, ticks, modifier, seconds);
        }
    }
}