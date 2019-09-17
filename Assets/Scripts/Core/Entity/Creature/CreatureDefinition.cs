using JetBrains.Annotations;
using UnityEngine;

namespace Core
{
    [CreateAssetMenu(fileName = "Creature Definition", menuName = "Game Data/Entities/Creature Definition", order = 2)]
    internal class CreatureDefinition : ScriptableObject
    {
        [UsedImplicitly, SerializeField] private string creatureNameId;
        [UsedImplicitly, SerializeField] private int modelId;

        public string CreatureNameId => creatureNameId;
        public int ModelId => modelId;
    }
}
