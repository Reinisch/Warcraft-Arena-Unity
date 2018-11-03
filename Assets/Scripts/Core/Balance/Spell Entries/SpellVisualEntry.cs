using System;
using JetBrains.Annotations;
using UnityEngine;

namespace Core
{
    [Serializable]
    public class SpellVisualEntry
    {
        public enum UsageType
        {
            Cast,
            Projectile,
            Impact,
            Aura,
        }

        [SerializeField, UsedImplicitly] private UsageType visualUsageType;
        [SerializeField, UsedImplicitly] private GameObject effectPrototype;

        public UsageType VisualUsageType => visualUsageType;
        public GameObject EffectPrototype => effectPrototype;
    }
}