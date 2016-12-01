using System;

using BasicRpgEngine.Characters;
using BasicRpgEngine.Spells;
using BasicRpgEngine.Graphics;

namespace BasicRpgEngine.Spells
{
    public class ModelChangeEffect : BaseEffect
    {
        AnimatedSprite newModel;
        string newModelName;

        public ModelChangeEffect(AnimatedSprite model, string modelName, AoeMode aoeMode)
            : base(aoeMode)
        {
            newModel = model;
            newModelName = modelName;
        }

        public override void Apply(ITargetable caster, ITargetable target, TimeSpan elapsedTime, SpellModificationInformation spellInfo, NetworkPlayerInterface playerUi)
        {
            if (!target.Character.Entity.ReplacingModels.ContainsKey(newModelName))
            {
                AnimatedSprite newAnimationSprite = (AnimatedSprite)newModel.Clone();
                newAnimationSprite.CurrentAnimation = target.Character.Sprite.CurrentAnimation;
                newAnimationSprite.CurrentAnimationFrame = target.Character.Sprite.CurrentAnimationFrame;
                newAnimationSprite.PerformAnimation = target.Character.Sprite.PerformAnimation;
                target.Character.Entity.ReplacingModels.Add(newModelName, newAnimationSprite);
            }
        }

        public override object Clone()
        {
            return new ModelChangeEffect(newModel, newModelName, AoeMode);
        }
    }
}