using JetBrains.Annotations;
using UnityEngine;

namespace Client
{
    [UsedImplicitly, CreateAssetMenu(fileName = "Targeting Settings", menuName = "Player Data/Input/Targeting Settings", order = 1)]
    public class TargetingSettings : ScriptableObject
    {
        [SerializeField, UsedImplicitly] private float targetRange;
        [SerializeField, HideInInspector, UsedImplicitly] private float targetRangeSqr;

        public float TargetRange => targetRange;
        public float TargetRangeSqr => targetRangeSqr;

        [UsedImplicitly]
        private void OnValidate()
        {
            targetRangeSqr = targetRange * targetRange;
        }
    }
}
