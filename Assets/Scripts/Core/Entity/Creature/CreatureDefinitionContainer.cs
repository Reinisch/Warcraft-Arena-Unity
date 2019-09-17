using System.Collections.Generic;
using Common;
using JetBrains.Annotations;
using UnityEngine;

namespace Core
{
    [UsedImplicitly, CreateAssetMenu(fileName = "Creature Definition Container", menuName = "Game Data/Containers/Creature Definition", order = 1)]
    internal class CreatureDefinitionContainer : ScriptableUniqueInfoContainer<CreatureDefinition>
    {
        [SerializeField, UsedImplicitly] private List<CreatureDefinition> definitions;

        protected override List<CreatureDefinition> Items => definitions;
    }
}
