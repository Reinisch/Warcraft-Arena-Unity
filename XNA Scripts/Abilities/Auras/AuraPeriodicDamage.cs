using System;

using BasicRpgEngine.Characters;
using BasicRpgEngine.Spells;

namespace BasicRpgEngine.Spells
{
    public class AuraPeriodicDamage : AuraBase
    {
        DamageType damageType;
        int tickDamage;
        int fullDamage;
        int modifier;
        float seconds;
        int ticks;
        TimeSpan tickTime;
        TimeSpan tickTimeLeft;

        public AuraPeriodicDamage(DamageType damageType, int fullDamage, int ticks, int modifier, float seconds)
            : base(AuraType.DamageOverTime,AuraControlEffect.None,seconds)
        {
            this.damageType = damageType;
            this.fullDamage = fullDamage;
            this.seconds = seconds;
            this.modifier = modifier;
            this.ticks = ticks;
            this.fullDamage = fullDamage;

            tickDamage = (fullDamage + modifier) / ticks;
            tickTime = TimeSpan.FromSeconds(seconds / ticks);
            tickTimeLeft = tickTime;
        }

        public override bool Update(TimeSpan elapsedTime, Entity entity)
        {
            if (TimeLeft == TimeSpan.Zero)
                return false;

            if (Duration != -1)
            {
                TimeLeft -= elapsedTime;
                if (TimeLeft.TotalMilliseconds < 0)
                    TimeLeft = TimeSpan.Zero;
            }

            tickTimeLeft -= elapsedTime;

            if (tickTimeLeft.TotalMilliseconds < 0)
            {
                tickTimeLeft = tickTime + tickTimeLeft;

                int amount = tickDamage;

                foreach (Buff buff in entity.Buffs)
                {
                    if (buff.HasAbsorbAura)
                    {
                        foreach (AuraBase auraAbsorb in buff.Auras)
                        {
                            amount = auraAbsorb.Absorb(damageType, amount, entity);
                            if (amount == 0)
                                return true;
                        }
                    }
                }

                entity.Health.Decrease((ushort)amount);
            }
            return true;
        }
        public override void Apply(Entity entity)
        {}
        public override void Reverse(Entity entity)
        {}
        public override int Absorb(DamageType damageType, int damageAmount, Entity entity)
        {
            return 0;
        }

        public override object Clone()
        {
            return new AuraPeriodicDamage(damageType, fullDamage, ticks, modifier, seconds);
        }
    }
}