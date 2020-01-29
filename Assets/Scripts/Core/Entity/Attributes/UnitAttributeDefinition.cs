using JetBrains.Annotations;
using UnityEngine;

namespace Core
{
    [CreateAssetMenu(fileName = "Unit Attribute Definition", menuName = "Game Data/Entities/Unit Attribute Definition", order = 1)]
    internal class UnitAttributeDefinition : ScriptableObject
    {
        [UsedImplicitly, SerializeField] private int baseHealth;
        [UsedImplicitly, SerializeField] private int baseMaxHealth;
        [UsedImplicitly, SerializeField] private int baseSpellPower;
        [UsedImplicitly, SerializeField] private int baseIntellect;
        [UsedImplicitly, SerializeField] private float critPercentage;

        internal int BaseHealth => baseHealth;
        internal int BaseMaxHealth => baseMaxHealth;
        internal int BaseSpellPower => baseSpellPower;
        internal int BaseIntellect => baseIntellect;
        internal float CritPercentage => critPercentage;
    }
}
