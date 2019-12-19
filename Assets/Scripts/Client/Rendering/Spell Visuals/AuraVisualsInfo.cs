using Common;
using Core;
using JetBrains.Annotations;
using UnityEngine;

namespace Client.Spells
{
    [UsedImplicitly, CreateAssetMenu(fileName = "Aura Visual Info", menuName = "Game Data/Visuals/Aura Visual Info", order = 3)]
    public class AuraVisualsInfo : ScriptableUniqueInfo<AuraVisualsInfo>, IEffectPositionerSettings
    {
        [SerializeField, UsedImplicitly] private AuraVisualsInfoContainer container;
        [SerializeField, UsedImplicitly] private AuraInfo auraInfo;
        [SerializeField, UsedImplicitly] private Sprite auraIcon;
        [SerializeField, UsedImplicitly] private EffectTagType tagType;
        [SerializeField, UsedImplicitly] private EffectSettings effectSettings;
        [SerializeField, UsedImplicitly] private bool keepOriginalRotation = true;
        [SerializeField, UsedImplicitly] private bool preventAnimation;

        protected override AuraVisualsInfo Data => this;
        protected override ScriptableUniqueInfoContainer<AuraVisualsInfo> Container => container;

        public AuraInfo AuraInfo => auraInfo;
        public Sprite AuraIcon => auraIcon;
        public EffectSettings EffectSettings => effectSettings;
        public EffectTagType EffectTagType => tagType;
        public bool AttachToTag => true;
        public bool KeepOriginalRotation => keepOriginalRotation;
        public bool PreventAnimation => preventAnimation;
        public bool KeepAliveWithNoParticles => true;
    }
}