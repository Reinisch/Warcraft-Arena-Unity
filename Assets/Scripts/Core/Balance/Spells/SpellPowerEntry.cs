using System;
using JetBrains.Annotations;
using UnityEngine;

namespace Core
{
    [Serializable]
    public class SpellPowerEntry
    {
        [SerializeField, UsedImplicitly] private SpellPowerType spellPowerType;
        [SerializeField, UsedImplicitly] private int powerCost;
        [SerializeField, UsedImplicitly] private float powerCostPercentage;
        [SerializeField, UsedImplicitly] private float powerCostPercentagePerSecond;

        public SpellPowerType SpellPowerType => spellPowerType;
        public int PowerCost => powerCost;
        public float PowerCostPercentage => powerCostPercentage;
        public float PowerCostPercentagePerSecond => powerCostPercentagePerSecond;
    }
}