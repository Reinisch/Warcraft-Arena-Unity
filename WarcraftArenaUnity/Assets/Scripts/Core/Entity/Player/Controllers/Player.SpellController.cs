using System.Collections.Generic;

namespace Core
{
    public partial class Player
    {
        internal new class SpellController : IUnitBehaviour
        {
            private readonly HashSet<SpellInfo> knownSpells = new HashSet<SpellInfo>();

            private ClassInfo appliedClass;
            private Player player;

            public bool HasClientLogic => false;
            public bool HasServerLogic => true;

            void IUnitBehaviour.DoUpdate(int deltaTime)
            {
            }

            void IUnitBehaviour.HandleUnitAttach(Unit unit)
            {
                player = (Player)unit;
            }

            void IUnitBehaviour.HandleUnitDetach()
            {
                player = null;
            }

            public bool HasKnownSpell(SpellInfo spellInfo) => knownSpells.Contains(spellInfo);

            public void AddClassSpells(ClassInfo classInfo)
            {
                appliedClass = classInfo;

                for (int i = 0; i < classInfo.ClassSpells.Count; i++)
                {
                    SpellInfo classSpell = classInfo.ClassSpells[i];

                    knownSpells.Add(classSpell);

                    if (classSpell.IsPassive)
                        player.Spells.TriggerSpell(classSpell, player);
                }
            }

            public void UpdateClassSpells(ClassInfo updatedClass)
            {
                if (appliedClass != null)
                {
                    for (int i = 0; i < appliedClass.ClassSpells.Count; i++)
                    {
                        SpellInfo appliedSpell = appliedClass.ClassSpells[i];

                        if (updatedClass.HasSpell(appliedSpell))
                            continue;

                        player.Auras.RemoveAuraWithSpellInfo(appliedSpell, AuraRemoveMode.Cancel);
                        knownSpells.Remove(appliedSpell);
                    }
                }

                AddClassSpells(updatedClass);
            }
        }
    }
}
