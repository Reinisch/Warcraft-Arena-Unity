using JetBrains.Annotations;
using UnityEngine;

namespace Core
{
    public class PhysicsManager : MonoBehaviour
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
        private PhysicsReference physicsReference;

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
            Layer.Characters = 0;
            Layer.Ground = 0;
            Layer.NoCollision = 0;

            Mask.Characters = 0;
            Mask.Ground = 0;
            Mask.NoCollision = 0;
        }
    }
}