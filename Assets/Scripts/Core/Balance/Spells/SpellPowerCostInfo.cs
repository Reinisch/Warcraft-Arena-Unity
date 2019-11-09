using System;
using JetBrains.Annotations;
using UnityEngine;

namespace Core
{
    [Serializable]
    public class SpellPowerCostInfo
    {
        [SerializeField, UsedImplicitly] private SpellPowerType spellPowerType;
        [SerializeField, UsedImplicitly] private float powerCostPercentage;
        [SerializeField, UsedImplicitly] private int powerCost;

        public SpellPowerType SpellPowerType => spellPowerType;
        public float PowerCostPercentage => powerCostPercentage;
        public int PowerCost => powerCost;
    }
}