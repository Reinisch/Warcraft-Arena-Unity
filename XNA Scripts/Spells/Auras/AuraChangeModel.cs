using System;

using BasicRpgEngine.Characters;
using BasicRpgEngine.Spells;
using BasicRpgEngine.Graphics;

namespace BasicRpgEngine.Spells
{
    public class AuraChangeModel : AuraBase
    {
        public string ModelName { get; private set; }

        public AuraChangeModel(string modelName, float seconds)
            : base(AuraType.ModelChange, AuraControlEffect.None, seconds)
        {
            ModelName = modelName;
        }

        public override bool Update(TimeSpan elapsedTime, Entity entity)
        {
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
        public override int Absorb(DamageType damageType, int damageAmount, Entity entity)
        {
            return 0;
        }
        public override void Apply(Entity entity)
        {
            AnimatedSprite newSprite;
            if (entity.ReplacingModels.TryGetValue(ModelName, out newSprite))
            {
                entity.ModelChangedStateCount++;
                entity.CurrentReplacedModel = newSprite;
            }
        }
        public override void Reverse(Entity entity)
        {
            entity.ModelChangedStateCount--;
        }

        public override object Clone()
        {
            return new AuraChangeModel(ModelName, Duration);
        }
    }
}