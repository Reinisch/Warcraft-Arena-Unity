using UnityEngine;

namespace Core
{
    public class PhysicsManager : SingletonGameObject<PhysicsManager>
    {
        public static class Mask
        {
            public static int Characters;
            public static int Ground;
            public static int NoCollision;
        }

        public static class Layer
        {
            public static int Characters;
            public static int Ground;
            public static int NoCollision;
        }

        [SerializeField]
        private PhysicMaterial groundedUnitMaterial;
        [SerializeField]
        private PhysicMaterial slidingUnitMaterial;

        public static PhysicMaterial GroundedMaterial => Instance.groundedUnitMaterial;
        public static PhysicMaterial SlidingMaterial => Instance.slidingUnitMaterial;

        public void Initialize()
        {
            Layer.Characters = LayerMask.NameToLayer("Characters");
            Layer.Ground = LayerMask.NameToLayer("Ground");
            Layer.NoCollision = LayerMask.NameToLayer("NoCollision");

            Mask.Characters = 1 << Layer.Characters;
            Mask.Ground = 1 << Layer.Ground;
            Mask.NoCollision = 1 << Layer.NoCollision;
        }

        public void Deinitialize()
        {
            Mask.Ground = 0;
        }
    }
}