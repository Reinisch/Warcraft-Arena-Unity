using System;
using Client.Effects;
using JetBrains.Annotations;
using UnityEngine;

namespace Client.Spells
{
    [Serializable]
    public class SpellVisualEffect
    {
        public enum UsageType
        {
            Cast,
            Projectile,
            Impact,
            Aura,
        }

        [SerializeField, UsedImplicitly] private UsageType visualUsageType;
        [SerializeField, UsedImplicitly] private EffectSettings effectSettings;

        public UsageType VisualUsageType => visualUsageType;
        public EffectSettings EffectSettings => effectSettings;
    }
}
