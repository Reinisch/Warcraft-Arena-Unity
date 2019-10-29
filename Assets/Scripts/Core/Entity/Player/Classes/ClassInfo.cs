using System.Collections.Generic;
using Common;
using JetBrains.Annotations;
using UnityEngine;

namespace Core
{
    [UsedImplicitly, CreateAssetMenu(fileName = "Class Info", menuName = "Game Data/Classes/Class Info", order = 1)]
    public class ClassInfo : ScriptableUniqueInfo<ClassInfo>
    {
        [SerializeField, UsedImplicitly] private ClassInfoContainer container;
        [SerializeField, UsedImplicitly] private ClassType classType;
        [SerializeField, UsedImplicitly] private List<SpellInfo> classSpells;

        private readonly HashSet<SpellInfo> spellInfoHashSet = new HashSet<SpellInfo>();

        protected override ScriptableUniqueInfoContainer<ClassInfo> Container => container;
        protected override ClassInfo Data => this;

        public ClassType ClassType => classType;
        public IReadOnlyList<SpellInfo> ClassSpells => classSpells;

        protected override void OnRegister()
        {
            base.OnRegister();

            spellInfoHashSet.IntersectWith(ClassSpells);
        }

        protected override void OnUnregister()
        {
            spellInfoHashSet.Clear();

            base.OnUnregister();
        }

        public bool HasSpell(SpellInfo spellInfo) => spellInfoHashSet.Contains(spellInfo);
    }
}
