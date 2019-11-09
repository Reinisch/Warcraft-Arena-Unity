using Common;
using JetBrains.Annotations;
using UnityEngine;

namespace Core
{
    [CreateAssetMenu(fileName = "Spell Power Type Info", menuName = "Game Data/Spells/Power Type", order = 1)]
    public class SpellPowerTypeInfo : ScriptableUniqueInfo<SpellPowerTypeInfo>
    {
        [UsedImplicitly, SerializeField] private SpellPowerTypeInfoContainer container;
        [UsedImplicitly, SerializeField] private SpellPowerType powerType;
        [UsedImplicitly, SerializeField] private EntityAttributes attributeTypeCurrent;
        [UsedImplicitly, SerializeField] private EntityAttributes attributeTypeMax;
        [UsedImplicitly, SerializeField] private EntityAttributes attributeTypeMaxNoMods;
        [UsedImplicitly, SerializeField] private int minBasePower;
        [UsedImplicitly, SerializeField] private int maxBasePower;
        [UsedImplicitly, SerializeField] private int maxTotalPower;
        [UsedImplicitly, SerializeField] private float regeneration;

        protected override SpellPowerTypeInfo Data => this;
        protected override ScriptableUniqueInfoContainer<SpellPowerTypeInfo> Container => container;

        public SpellPowerType PowerType => powerType;
        public EntityAttributes AttributeTypeCurrent => attributeTypeCurrent;
        public EntityAttributes AttributeTypeMax => attributeTypeMax;
        public EntityAttributes AttributeTypeMaxNoMods => attributeTypeMaxNoMods;
        public int MinBasePower => minBasePower;
        public int MaxBasePower => maxBasePower;
        public int MaxTotalPower => maxTotalPower;
        public float Regeneration => regeneration;
    }
}
