using System;
using JetBrains.Annotations;
using UnityEngine;

namespace Client.Spells
{
    [Serializable]
    public class EffectSpellSettings : IEffectPositionerSettings
    {
        public enum UsageType
        {
            Cast,
            Projectile,
            Impact,
            Destination
        }

        [SerializeField, UsedImplicitly] private bool attachToTag;
        [SerializeField, UsedImplicitly] private UsageType visualUsageType;
        [SerializeField, UsedImplicitly] private EffectTagType tagType;
        [SerializeField, UsedImplicitly] private EffectSettings effectSettings;

        public UsageType VisualUsageType => visualUsageType;
        public EffectTagType EffectTagType => tagType;
        public EffectSettings EffectSettings => effectSettings;
        public bool AttachToTag => attachToTag;
        public bool KeepOriginalRotation => false;
        public bool KeepAliveWithNoParticles => visualUsageType == UsageType.Projectile;
    }
}
