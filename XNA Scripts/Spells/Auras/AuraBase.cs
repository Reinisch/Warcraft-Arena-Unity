using System;

using BasicRpgEngine.Characters;
using BasicRpgEngine.Spells;

namespace BasicRpgEngine.Spells
{
    public abstract class AuraBase : ICloneable
    {
        public AuraType ModifierType { get; protected set; }
        public AuraControlEffect ControlEffect { get; protected set; }

        public float Duration { get; private set; }
        public TimeSpan TimeLeft { get; protected set; }

        public AuraBase(AuraType modifierType , AuraControlEffect controlEffect, float seconds)
        {
            ControlEffect = controlEffect;
            ModifierType = modifierType;
            Duration = seconds;

            if (seconds == -1)
                TimeLeft = TimeSpan.FromSeconds(1);
            else
                TimeLeft = TimeSpan.FromSeconds(seconds);
        }

        public abstract bool Update(TimeSpan elapsedTime, Entity entity);
        public abstract void Apply(Entity entity);
        public abstract void Reverse(Entity entity);
        public abstract int Absorb(DamageType damageType, int damageAmount, Entity entity);

        public abstract object Clone();
    }
}