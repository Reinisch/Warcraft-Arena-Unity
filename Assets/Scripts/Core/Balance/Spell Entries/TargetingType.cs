using System;
using JetBrains.Annotations;
using UnityEngine;

namespace Core
{
    [Serializable]
    public struct TargetingType
    {
        [SerializeField, UsedImplicitly] private SpellTargetEntities targetEntities;
        [SerializeField, UsedImplicitly] private SpellTargetReferences referenceType;
        [SerializeField, UsedImplicitly] private SpellTargetSelection selectionCategory;
        [SerializeField, UsedImplicitly] private SpellTargetChecks selectionCheckType;
        [SerializeField, UsedImplicitly] private SpellTargetDirections directionType;

        public SpellTargetEntities TargetEntities => targetEntities;
        public SpellTargetReferences ReferenceType => referenceType;
        public SpellTargetSelection SelectionCategory => selectionCategory;
        public SpellTargetChecks SelectionCheckType => selectionCheckType;
        public SpellTargetDirections DirectionType => directionType;
        public bool IsArea => SelectionCategory == SpellTargetSelection.Area || SelectionCategory == SpellTargetSelection.Cone;
        public bool IsAreaLookup => IsArea || SelectionCategory == SpellTargetSelection.Nearby;

        public override bool Equals(object obj)
        {
            return obj is TargetingType type && this == type;
        }

        public override int GetHashCode()
        {
            return (int)TargetEntities * (int)ReferenceType + (int)SelectionCategory + (int)SelectionCheckType * (int)DirectionType;
        }

        public static bool operator ==(TargetingType left, TargetingType right)
        {
            return left.targetEntities == right.targetEntities &&
                   left.referenceType == right.referenceType &&
                   left.selectionCategory == right.selectionCategory &&
                   left.selectionCheckType == right.selectionCheckType &&
                   left.directionType == right.directionType;
        }

        public static bool operator !=(TargetingType left, TargetingType right)
        {
            return !(left == right);
        }
    }
}