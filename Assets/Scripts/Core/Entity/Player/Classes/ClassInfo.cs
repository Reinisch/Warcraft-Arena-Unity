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

        protected override ScriptableUniqueInfoContainer<ClassInfo> Container => container;
        protected override ClassInfo Data => this;

        public ClassType ClassType => classType;
    }
}
