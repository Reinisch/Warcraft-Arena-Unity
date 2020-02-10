using System.Collections.Generic;
using Common;
using JetBrains.Annotations;
using UnityEngine;

namespace Core
{
    [UsedImplicitly, CreateAssetMenu(fileName = "Creature Info Container", menuName = "Game Data/Containers/Creature Info", order = 1)]
    internal class CreatureInfoContainer : ScriptableUniqueInfoContainer<CreatureInfo>
    {
        [SerializeField, UsedImplicitly] private List<CreatureInfo> creatureInfos;

        protected override List<CreatureInfo> Items => creatureInfos;
    }
}
