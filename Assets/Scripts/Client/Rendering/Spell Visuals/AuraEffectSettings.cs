using Core;
using JetBrains.Annotations;
using UnityEngine;

namespace Client.Spells
{
    [UsedImplicitly, CreateAssetMenu(fileName = "Aura Effect Settings", menuName = "Game Data/Visuals/Aura Effect Settings", order = 3)]
    public class AuraEffectSettings : ScriptableObject, IEffectPositionerSettings
    {
        [SerializeField, UsedImplicitly] private AuraInfo auraInfo;
        [SerializeField, UsedImplicitly] private Sprite auraIcon;
        [SerializeField, UsedImplicitly] private EffectTagType tagType;
        [SerializeField, UsedImplicitly] private EffectSettings effectSettings;
        [SerializeField, UsedImplicitly] private bool keepOriginalRotation = true;

        public AuraInfo AuraInfo => auraInfo;
        public Sprite AuraIcon => auraIcon;
        public EffectSettings EffectSettings => effectSettings;
        public EffectTagType EffectTagType => tagType;
        public bool AttachToTag => true;
        public bool KeepOriginalRotation => keepOriginalRotation;
        public bool KeepAliveWithNoParticles => true;
    }
}

