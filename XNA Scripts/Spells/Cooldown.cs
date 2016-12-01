using System;

namespace BasicRpgEngine.Spells
{
    [Serializable]
    public class Cooldown : ICloneable
    {
        public TimeSpan Duration { get; private set; }
        public TimeSpan TimeLeft { get; set; }
        public bool NoCooldown { get; private set; }

        public Cooldown(float seconds)
        {
            Duration = TimeSpan.FromSeconds(seconds);
            TimeLeft = TimeSpan.Zero;
            NoCooldown = true;
        }

        public void Update(TimeSpan elapsedTime)
        {
            if (NoCooldown)
                return;

            TimeLeft -= elapsedTime;
            if (TimeLeft.TotalMilliseconds < 0)
            {
                TimeLeft = TimeSpan.Zero;
                NoCooldown = true;
            }
        }
        public void Apply(TimeSpan latency)
        {
            if (Duration == TimeSpan.Zero)
                return;
            TimeLeft = Duration - latency;
            NoCooldown = false;
        }
        public void ApplyModified(TimeSpan latency, float haste)
        {
            if (Duration == TimeSpan.Zero)
                return;
            TimeLeft = TimeSpan.FromSeconds(Duration.TotalSeconds/(1+haste/100)) - latency;
            NoCooldown = false;
        }

        public object Clone()
        {
            Cooldown clone = new Cooldown(0);
            clone.Duration = this.Duration;
            clone.TimeLeft = this.TimeLeft;
            clone.NoCooldown = this.NoCooldown;
            return clone;
        }
    }
}
