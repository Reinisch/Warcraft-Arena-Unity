using JetBrains.Annotations;
using UnityEngine;

namespace Core
{
    [CreateAssetMenu(fileName = "Unit Attribute Definition", menuName = "Game Data/Entities/Unit Attribute Definition", order = 1)]
    internal class UnitAttributeDefinition : ScriptableObject
    {
        [UsedImplicitly, SerializeField] private int baseHealth;
        [UsedImplicitly, SerializeField] private int baseMaxHealth;
        [UsedImplicitly, SerializeField] private int baseMana;
        [UsedImplicitly, SerializeField] private int baseMaxMana;

        [UsedImplicitly, SerializeField] private float critPercentage;
        [UsedImplicitly, SerializeField] private float rangedCritPercentage;
        [UsedImplicitly, SerializeField] private float spellCritPercentage;

        internal int BaseHealth => baseHealth;
        internal int BaseMaxHealth => baseMaxHealth;
        internal int BaseMana => baseMana;
        internal int BaseMaxMana => baseMaxMana;

        internal float CritPercentage => critPercentage;
        internal float RangedCritPercentage => rangedCritPercentage;
        internal float SpellCritPercentage => spellCritPercentage;
    }
}
