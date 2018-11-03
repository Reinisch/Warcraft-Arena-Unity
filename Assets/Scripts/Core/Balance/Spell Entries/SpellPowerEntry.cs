using System;
using JetBrains.Annotations;
using UnityEngine;

namespace Core
{
    [Serializable]
    public class SpellPowerEntry
    {
        [SerializeField, UsedImplicitly] private PowerType powerType;
        [SerializeField, UsedImplicitly] private int powerCost;
        [SerializeField, UsedImplicitly] private float powerCostPercentage;
        [SerializeField, UsedImplicitly] private float powerCostPercentagePerSecond;

        public PowerType PowerType => powerType;
        public int PowerCost => powerCost;
        public float PowerCostPercentage => powerCostPercentage;
        public float PowerCostPercentagePerSecond => powerCostPercentagePerSecond;
    }
}