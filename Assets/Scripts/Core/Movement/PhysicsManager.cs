using Common;
using JetBrains.Annotations;
using UnityEngine;

namespace Core
{
    public class PhysicsManager : SingletonBehaviour<PhysicsManager>
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

        [SerializeField, UsedImplicitly]
        private PhysicMaterial groundedUnitMaterial;
        [SerializeField, UsedImplicitly]
        private PhysicMaterial slidingUnitMaterial;

        public static PhysicMaterial GroundedMaterial => Instance.groundedUnitMaterial;
        public static PhysicMaterial SlidingMaterial => Instance.slidingUnitMaterial;

        public new void Initialize()
        {
            base.Initialize();

            Layer.Characters = LayerMask.NameToLayer("Characters");
            Layer.Ground = LayerMask.NameToLayer("Ground");
            Layer.NoCollision = LayerMask.NameToLayer("NoCollision");

            Mask.Characters = 1 << Layer.Characters;
            Mask.Ground = 1 << Layer.Ground;
            Mask.NoCollision = 1 << Layer.NoCollision;
        }

        public new void Deinitialize()
        {
            base.Deinitialize();
        }
    }
}