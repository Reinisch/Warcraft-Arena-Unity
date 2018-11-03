using System;
using JetBrains.Annotations;
using UnityEngine;

namespace Core
{
    [Serializable]
    public struct TargetingType
    {
        [SerializeField, UsedImplicitly] private TargetEntities targetEntities;
        [SerializeField, UsedImplicitly] private TargetReferences referenceType;
        [SerializeField, UsedImplicitly] private TargetSelections selectionCategory;
        [SerializeField, UsedImplicitly] private TargetChecks selectionCheckType;
        [SerializeField, UsedImplicitly] private TargetDirections directionType;

        public TargetEntities TargetEntities => targetEntities;
        public TargetReferences ReferenceType => referenceType;
        public TargetSelections SelectionCategory => selectionCategory;
        public TargetChecks SelectionCheckType => selectionCheckType;
        public TargetDirections DirectionType => directionType;
        public bool IsArea => SelectionCategory == TargetSelections.Area || SelectionCategory == TargetSelections.Cone;
        public bool IsAreaLookup => IsArea || SelectionCategory == TargetSelections.Nearby;

        public override bool Equals(object obj)
        {
            return obj is TargetingType && this == (TargetingType)obj;
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