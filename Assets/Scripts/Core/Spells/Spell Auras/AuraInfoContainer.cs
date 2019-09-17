using System.Collections.Generic;
using Common;
using JetBrains.Annotations;
using UnityEngine;

namespace Core
{
    [UsedImplicitly, CreateAssetMenu(fileName = "Aura Info Container", menuName = "Game Data/Containers/Aura Info", order = 1)]
    internal class AuraInfoContainer : ScriptableUniqueInfoContainer<AuraInfo>
    {
        [SerializeField, UsedImplicitly] private List<AuraInfo> auraInfos;

        protected override List<AuraInfo> Items => auraInfos;
    }
}
