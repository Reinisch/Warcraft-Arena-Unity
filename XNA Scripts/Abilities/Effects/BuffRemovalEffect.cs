using System;

using BasicRpgEngine.Characters;
using BasicRpgEngine.Spells;

namespace BasicRpgEngine.Spells
{
    public class BuffRemovalEffect : BaseEffect
    {
        Predicate<Buff> match;

        public BuffRemovalEffect(Predicate<Buff> condition)
            : base(AoeMode.None)
        {
            match = condition;
        }

        public override void Apply(ITargetable caster, ITargetable target, TimeSpan elapsedTime, SpellModificationInformation spellInfo, NetworkPlayerInterface playerUi)
        {
            target.Character.Entity.Buffs.RemoveAll(match);
        }

        public override object Clone()
        {
            return new BuffRemovalEffect((Predicate<Buff>)match.Clone());
        }
    }
}
