using System.Collections.Generic;
using Common;
using JetBrains.Annotations;
using UnityEngine;

namespace Core
{
    [UsedImplicitly, CreateAssetMenu(fileName = "Class Info Container", menuName = "Game Data/Containers/Class Info", order = 1)]
    public class ClassInfoContainer : ScriptableUniqueInfoContainer<ClassInfo>
    {
        [SerializeField, UsedImplicitly] private List<ClassInfo> classes;

        protected override List<ClassInfo> Items => classes;
    }
}
