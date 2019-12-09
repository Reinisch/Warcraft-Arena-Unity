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
        [SerializeField, UsedImplicitly] private bool isAvailable;
        [SerializeField, UsedImplicitly] private ClassType classType;
        [SerializeField, UsedImplicitly] private List<SpellPowerTypeInfo> powerTypes;
        [SerializeField, UsedImplicitly] private List<SpellInfo> classSpells;

        private readonly HashSet<SpellInfo> spellInfoHashSet = new HashSet<SpellInfo>();
        private readonly HashSet<SpellPowerType> spellPowerTypeInfoHashSet = new HashSet<SpellPowerType>();

        protected override ScriptableUniqueInfoContainer<ClassInfo> Container => container;
        protected override ClassInfo Data => this;

        public bool IsAvailable => isAvailable;
        public ClassType ClassType => classType;
        public SpellPowerType MainPowerType => powerTypes[0].PowerType;
        public IReadOnlyList<SpellInfo> ClassSpells => classSpells;
        public IReadOnlyList<SpellPowerTypeInfo> PowerTypes => powerTypes;

        protected override void OnRegister()
        {
            base.OnRegister();

            spellInfoHashSet.IntersectWith(ClassSpells);
            foreach (var powerTypeInfo in powerTypes)
                spellPowerTypeInfoHashSet.Add(powerTypeInfo.PowerType);
        }

        protected override void OnUnregister()
        {
            spellInfoHashSet.Clear();
            spellPowerTypeInfoHashSet.Clear();

            base.OnUnregister();
        }

        public bool HasSpell(SpellInfo spellInfo) => spellInfoHashSet.Contains(spellInfo);
        public bool HasPower(SpellPowerType spellPowerType) => spellPowerTypeInfoHashSet.Contains(spellPowerType);
    }
}
