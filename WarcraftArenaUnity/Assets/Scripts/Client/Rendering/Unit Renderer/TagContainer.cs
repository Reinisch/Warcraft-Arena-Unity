using System;
using JetBrains.Annotations;
using UnityEngine;

namespace Client
{
    [Serializable]
    public class TagContainer : IEffectPositioner
    {
        [SerializeField, UsedImplicitly] private EffectTagType defaultLaunchTag = EffectTagType.LeftHand;
        [SerializeField, UsedImplicitly] private Transform defaultTag;
        [SerializeField, UsedImplicitly] private Transform bottomTag;
        [SerializeField, UsedImplicitly] private Transform footTag;
        [SerializeField, UsedImplicitly] private Transform impactTag;
        [SerializeField, UsedImplicitly] private Transform impactStaticTag;
        [SerializeField, UsedImplicitly] private Transform rightHandTag;
        [SerializeField, UsedImplicitly] private Transform leftHandTag;
        [SerializeField, UsedImplicitly] private Transform damageTag;
        [SerializeField, UsedImplicitly] private Transform nameplateTag;

        public Vector3 FindTag(EffectTagType tagType)
        {
            switch (tagType)
            {
                case EffectTagType.Bottom:
                    return (bottomTag ?? defaultTag).position;
                case EffectTagType.Foot:
                    return (footTag ?? defaultTag).position;
                case EffectTagType.Impact:
                    return (impactTag ?? defaultTag).position;
                case EffectTagType.ImpactStatic:
                    return (impactStaticTag ?? defaultTag).position;
                case EffectTagType.RightHand:
                    return (rightHandTag ?? defaultTag).position;
                case EffectTagType.LeftHand:
                    return (leftHandTag ?? defaultTag).position;
                default:
                    throw new ArgumentOutOfRangeException(nameof(tagType));
            }
        }

        public Vector3 FindNameplateTag() => (nameplateTag ?? defaultTag).transform.position;

        public Vector3 FindDefaultLaunchTag() => FindTag(defaultLaunchTag);

        public void TransferChildren(TagContainer otherContainer)
        {
            TransferChildren(defaultTag, otherContainer.defaultTag);
            TransferChildren(bottomTag, otherContainer.bottomTag);
            TransferChildren(footTag, otherContainer.footTag);
            TransferChildren(impactTag, otherContainer.impactTag);
            TransferChildren(impactStaticTag, otherContainer.impactStaticTag);
            TransferChildren(rightHandTag, otherContainer.rightHandTag);
            TransferChildren(leftHandTag, otherContainer.leftHandTag);
            TransferChildren(damageTag, otherContainer.damageTag);
            TransferChildren(nameplateTag, otherContainer.nameplateTag);
        }

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
                case EffectTagType.ImpactStatic:
                    targetTag = impactStaticTag ?? defaultTag;
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

            effectEntity.KeepAliveWithNoParticles = settings.KeepAliveWithNoParticles;
            effectEntity.KeepOriginalRotation = settings.KeepOriginalRotation;
            effectEntity.Transform.position = targetTag.position;
        }

        public void ApplyPositioning(FloatingText floatingText)
        {
            floatingText.transform.position = (damageTag ?? defaultTag).position;
        }

        private void TransferChildren(Transform source, Transform destination)
        {
            if (source == destination)
                return;

            while (source.childCount > 0)
                source.GetChild(0).SetParent(destination, false);
        }
    }
}
