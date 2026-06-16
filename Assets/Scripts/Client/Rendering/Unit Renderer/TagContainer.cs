using Core;
using JetBrains.Annotations;
using System;
using System.Collections.Generic;
using System.IO;
using System.Runtime.Serialization;
using System.Text;
using UnityEngine;

namespace Client
{
    public class ListNode
    {
        public ListNode Prev;
        public ListNode Next;
        public ListNode Rand;
        public string Data;
    }

    public class ListRand
    {
        public ListNode Head;
        public ListNode Tail;
        public int Count;

        public void Serialize(FileStream fileStream)
        {
            if (Count < 0)
                throw new SerializationException("Count should not be negative");

            using var writer = new BinaryWriter(fileStream, Encoding.UTF8, leaveOpen: true);

            Dictionary<ListNode, int> indicesByNode = new();
            ListNode nodeToIndex = Head;
            for (int i = 0; nodeToIndex != null; i++, nodeToIndex = nodeToIndex.Next)
                if (!indicesByNode.TryAdd(nodeToIndex, i))
                    throw new SerializationException("Duplicate nodes found, potentially infinite list");

            if (indicesByNode.Count != Count)
                throw new SerializationException("Amount of items differs from Count");

            writer.Write(indicesByNode.Count);
            for (ListNode nodeToSave = Head; nodeToSave != null; nodeToSave = nodeToSave.Next)
            {
                bool hasData = nodeToSave.Data != null;
                writer.Write(hasData);

                if (hasData)
                    writer.Write(nodeToSave.Data);

                writer.Write(nodeToSave.Rand != null ? indicesByNode[nodeToSave.Rand] : -1);
            }
        }

        public void Deserialize(FileStream fileStream)
        {
            using var reader = new BinaryReader(fileStream, Encoding.UTF8, leaveOpen: true);
            Head = Tail = null;
            Count = reader.ReadInt32();

            if (Count < 0)
                throw new SerializationException("Count should not be negative");

            ListNode[] nodes = FillEmpty();

            for (int i = 0; i < Count; i++)
            {
                if (i == 0)
                    Head = nodes[i];

                if (i == Count - 1)
                    Tail = nodes[i];

                if (i > 0)
                {
                    nodes[i - 1].Next = nodes[i];
                    nodes[i].Prev = nodes[i - 1];
                }

                ListNode nodeToLoad = nodes[i];
                bool hasData = reader.ReadBoolean();
                nodeToLoad.Data = hasData ? reader.ReadString() : null;
                int randIndex = reader.ReadInt32();
                nodeToLoad.Rand = randIndex >= 0 ? nodes[randIndex] : null;
            }
        }

        private ListNode[] FillEmpty()
        {
            if (Count == 0)
                return Array.Empty<ListNode>();

            ListNode[] nodes = new ListNode[Count];
            for (int i = 0; i < Count; i++)
            {
                nodes[i] = new ListNode();
            }
            return nodes;
        }
    }

    [Serializable]
    public class TagContainer : IEffectPositioner
    {
        [Serializable]
        public class TagEntry
        {
            [field: SerializeField]
            public ProjectileTagInfo ProjectileTag { get; private set; }

            [field: SerializeField]
            public Transform Target { get; private set; }
        }

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
        [SerializeField, UsedImplicitly] private List<TagEntry> tagSetups = new();

        private Dictionary<ProjectileTagInfo, TagEntry> entriesByProjectileTag = new();

        public void OnAwake()
        {
            tagSetups.ForEach(item => entriesByProjectileTag.Add(item.ProjectileTag, item));
        }

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

        public Vector3 FindProjectileTag(ProjectileTagInfo tag)
        {
            if (entriesByProjectileTag.TryGetValue(tag, out TagEntry entry))
            {
                return entry.Target.position;
            }

            return FindDefaultLaunchTag();
        }

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
