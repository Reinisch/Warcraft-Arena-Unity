using System;

using BasicRpgEngine.Characters;
using BasicRpgEngine.Spells;

namespace BasicRpgEngine.Spells
{
    public class RootRemovalEffect : BaseEffect
    {
        bool magicRemoval;
        bool physicRemoval;

        public RootRemovalEffect(bool magic, bool physic)
            : base(AoeMode.None)
        {
            magicRemoval = magic;
            physicRemoval = physic;
        }

        public override void Apply(ITargetable caster, ITargetable target, TimeSpan elapsedTime, SpellModificationInformation spellInfo, NetworkPlayerInterface playerUi)
        {
            target.Character.Entity.Buffs.RemoveAll(item => item.Auras.Find(aura => aura.ModifierType == AuraType.Root) != null);
        }

        public override object Clone()
        {
            return new RootRemovalEffect(magicRemoval, physicRemoval);
        }
    }
}