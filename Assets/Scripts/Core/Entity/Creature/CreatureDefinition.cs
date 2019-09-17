using Common;
using JetBrains.Annotations;
using UnityEngine;

namespace Core
{
    [CreateAssetMenu(fileName = "Creature Definition", menuName = "Game Data/Entities/Creature Definition", order = 2)]
    internal sealed class CreatureDefinition : ScriptableUniqueInfo<CreatureDefinition>
    {
        [SerializeField, UsedImplicitly] private CreatureDefinitionContainer container;

        [UsedImplicitly, SerializeField] private string creatureNameId;
        [UsedImplicitly, SerializeField] private int modelId;

        protected override ScriptableUniqueInfoContainer<CreatureDefinition> Container => container;
        protected override CreatureDefinition Data => this;

        public new int Id => base.Id;

        public string CreatureNameId => creatureNameId;
        public int ModelId => modelId;
    }
}
