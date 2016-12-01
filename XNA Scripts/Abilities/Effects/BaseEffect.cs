using System;

using BasicRpgEngine.Characters;
using BasicRpgEngine.Spells;

namespace BasicRpgEngine.Spells
{
    public abstract class BaseEffect : ICloneable
    {
        public AoeMode AoeMode { get; private set; }

        public BaseEffect(AoeMode aoeMode)
        {
            AoeMode = aoeMode;
        }

        public abstract void Apply(ITargetable caster, ITargetable target, TimeSpan elapsedTime, SpellModificationInformation spellInfo, NetworkPlayerInterface playerUi);

        public abstract object Clone();
    }  
}