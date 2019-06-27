using JetBrains.Annotations;
using UnityEngine;

namespace Core
{
    [CreateAssetMenu(fileName = "Creature Definition", menuName = "Game Data/Entities/Creature Definition", order = 2)]
    internal class CreatureDefinition : ScriptableObject
    {
        [UsedImplicitly, SerializeField] private string creatureNameId;

        public string CreatureNameId => creatureNameId;
    }
}
