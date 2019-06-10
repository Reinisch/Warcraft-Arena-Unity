using System;
using JetBrains.Annotations;
using UnityEngine;

namespace Client
{
    [Serializable]
    public class TagContainer : IEffectPositioner
    {
        [SerializeField, UsedImplicitly] private Transform defaultTag;
        [SerializeField, UsedImplicitly] private Transform bottomTag;
        [SerializeField, UsedImplicitly] private Transform footTag;
        [SerializeField, UsedImplicitly] private Transform impactTag;
        [SerializeField, UsedImplicitly] private Transform rightHandTag;
        [SerializeField, UsedImplicitly] private Transform leftHandTag;
        [SerializeField, UsedImplicitly] private Transform damageTag;

        public void ApplyPositioning(IEffectEntity effectEntity, IEffectPositionerSettings settings)
        {
            Transform targetTag;
            switch (settings.EffectTagType)
            {
                case EffectTagType.Bottom:
                    targetTag = bottomTag ?? defaultTag;
                    break;
                case EffectTagType.Foot:
                    targetTag = footTag ?? defaultTag;
                    break;
                case EffectTagType.Impact:
                    targetTag = impactTag ?? defaultTag;
                    break;
                case EffectTagType.RightHand:
                    targetTag = rightHandTag ?? defaultTag;
                    break;
                case EffectTagType.LeftHand:
                    targetTag = leftHandTag ?? defaultTag;
                    break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(settings.EffectTagType));
            }

            if(settings.AttachToTag)
                effectEntity.Transform.SetParent(targetTag);

            effectEntity.Transform.position = targetTag.position;
        }

        public void ApplyPositioning(FloatingText floatingText)
        {
            floatingText.transform.position = (damageTag ?? defaultTag).position;
        }
    }
}
