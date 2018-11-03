using UnityEngine;

namespace Core
{
    public class PhysicsManager : SingletonGameObject<PhysicsManager>
    {
        public static class Mask
        {
            public static int Ground;
        }

        [SerializeField]
        private PhysicMaterial groundedUnitMaterial;
        [SerializeField]
        private PhysicMaterial slidingUnitMaterial;

        public static PhysicMaterial GroundedMaterial => Instance.groundedUnitMaterial;
        public static PhysicMaterial SlidingMaterial => Instance.slidingUnitMaterial;

        public override void Initialize()
        {
            base.Initialize();

            Mask.Ground = 1 << LayerMask.NameToLayer("Ground");
        }
    }
}