using System;

using BasicRpgEngine.Characters;
using BasicRpgEngine.Spells;

namespace BasicRpgEngine.Spells
{
    public class AuraAbsorbDamage : AuraBase
    {
        public bool IsAbsorbingFrost { get; private set; }
        public bool IsAbsorbingPhysical { get; private set; }
        public bool IsAbsorbingFire { get; private set; }
        public bool IsAbsorbingArcane { get; private set; }
        public bool IsAbsorbingNature { get; private set; }
        public bool IsAbsorbingShadow { get; private set; }
        public bool IsAbsorbingHoly { get; private set; }
        public int AbsorbAmount { get; private set; }

        public AuraAbsorbDamage(bool absorbFrost, bool absorbPhysical, bool absorbFire, bool absorbArcane,
            bool absorbNature, bool absorbShadow, bool absorbHoly, int absorbAmount, float seconds)
            :base(AuraType.AbsorbDamage, AuraControlEffect.None, seconds)
        {
            IsAbsorbingFrost = absorbFrost;
            IsAbsorbingPhysical = absorbPhysical;
            IsAbsorbingFire = absorbFire;
            IsAbsorbingArcane = absorbArcane;
            IsAbsorbingNature = absorbNature;
            IsAbsorbingShadow = absorbShadow;
            IsAbsorbingHoly = absorbHoly;
            AbsorbAmount = absorbAmount;
        }

        public override bool Update(TimeSpan elapsedTime, Entity entity)
        {
            if (AbsorbAmount == 0)
            {
                TimeLeft = TimeSpan.Zero;
                return true;
            }

            if (TimeLeft == TimeSpan.Zero)
                return true;

            if (Duration == -1)
                return false;

            TimeLeft -= elapsedTime;
            if (TimeLeft.TotalMilliseconds < 0)
            {
                TimeLeft = TimeSpan.Zero;
                return true;
            }
            return false;
        }
        public override void Apply(Entity entity)
        {}
        public override void Reverse(Entity entity)
        {}
        public override int Absorb(DamageType damageType, int damageAmount, Entity entity)
        {
            if (AbsorbAmount > damageAmount)
            {
                AbsorbAmount -= damageAmount;
                return 0;
            }
            else
            {
                damageAmount -= AbsorbAmount;
                AbsorbAmount = 0;
                return damageAmount;
            }
        }

        public override object Clone()
        {
            return new AuraAbsorbDamage(IsAbsorbingFrost, IsAbsorbingPhysical, IsAbsorbingFire,
                IsAbsorbingArcane, IsAbsorbingNature, IsAbsorbingShadow, IsAbsorbingHoly, AbsorbAmount, Duration);
        }
    }
}