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

        private readonly Dictionary<int, CreatureInfo> creatureInfoById = new Dictionary<int, CreatureInfo>();

        public IReadOnlyDictionary<int, CreatureInfo> CreatureInfoById => creatureInfoById;

        public override void Register()
        {
            base.Register();

            creatureInfos.ForEach(creature => creatureInfoById.Add(creature.Id, creature));
        }

        public override void Unregister()
        {
            creatureInfoById.Clear();

            base.Unregister();
        }
    }
}
