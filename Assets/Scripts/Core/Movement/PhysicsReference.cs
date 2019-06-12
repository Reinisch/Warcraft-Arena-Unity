using JetBrains.Annotations;
using UnityEngine;

namespace Core
{
    [CreateAssetMenu(fileName = "Physics Reference", menuName = "Game Data/Physics/Physics Reference", order = 3)]
    public class PhysicsReference : ScriptableObject
    {
        [SerializeField, UsedImplicitly]
        private PhysicMaterial groundedUnitMaterial;
        [SerializeField, UsedImplicitly]
        private PhysicMaterial slidingUnitMaterial;

        public PhysicMaterial GroundedMaterial => groundedUnitMaterial;
        public PhysicMaterial SlidingMaterial => slidingUnitMaterial;
    }
}
