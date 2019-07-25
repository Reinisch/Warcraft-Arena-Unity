using System;
using Core;
using JetBrains.Annotations;
using UnityEngine;

namespace Client.Spells
{
    [Serializable]
    public class AuraVisualSettings : IEffectPositionerSettings
    {
        [SerializeField, UsedImplicitly] private AuraInfo auraInfo;
        [SerializeField, UsedImplicitly] private Sprite auraIcon;
        [SerializeField, UsedImplicitly] private EffectTagType tagType;
        [SerializeField, UsedImplicitly] private EffectSettings effectSettings;

        public AuraInfo AuraInfo => auraInfo;
        public Sprite AuraIcon => auraIcon;
        public EffectSettings EffectSettings => effectSettings;
        public EffectTagType EffectTagType => tagType;
        public bool AttachToTag => true;
        public bool KeepOriginalRotation => true;
        public bool KeepAliveWithNoParticles => true;
    }
}
