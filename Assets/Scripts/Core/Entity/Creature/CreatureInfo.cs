using Common;
using JetBrains.Annotations;
using UnityEngine;

namespace Core
{
    [CreateAssetMenu(fileName = "Creature Info", menuName = "Game Data/Entities/Creature Info", order = 2)]
    public sealed class CreatureInfo : ScriptableUniqueInfo<CreatureInfo>
    {
        [UsedImplicitly, SerializeField] private UnitAttributeDefinition attributes;
        [SerializeField, UsedImplicitly] private VehicleInfo vehicleInfo;
        [UsedImplicitly, SerializeField] private string creatureName;
        [UsedImplicitly, SerializeField] private int modelId;
        [UsedImplicitly, SerializeField] private float nameplateSizeModifier = 1;

        public new int Id => base.Id;
        public int ModelId => modelId;
        public string CreatureName => creatureName;
        public UnitAttributeDefinition Attributes => attributes;
        public float NameplateSizeModifier => nameplateSizeModifier;
    }
}
