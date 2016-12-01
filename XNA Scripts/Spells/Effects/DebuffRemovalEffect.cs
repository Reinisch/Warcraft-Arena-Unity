using System;

using BasicRpgEngine.Characters;
using BasicRpgEngine.Spells;

namespace BasicRpgEngine.Spells
{
    public class DebuffRemovalEffect : BaseEffect
    {
        public DebuffRemovalEffect()
            : base(AoeMode.None)
        {}

        public override void Apply(ITargetable caster, ITargetable target, TimeSpan elapsedTime, SpellModificationInformation spellInfo, NetworkPlayerInterface playerUi)
        {
            target.Character.Entity.Buffs.RemoveAll(item => item.BuffType == BuffType.Debuff);
        }

        public override object Clone()
        {
            return new DebuffRemovalEffect();
        }
    }
}