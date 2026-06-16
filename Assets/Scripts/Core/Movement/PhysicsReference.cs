using Common;
using JetBrains.Annotations;
using UnityEngine;

namespace Core
{
    public class PhysicsReference : ScriptableReference
    {
        public static class Mask
        {
            public static int Characters { get; internal set; }
            public static int Ground { get; internal set; }
            public static int Teleportation { get; internal set; }
            public static int Interactable { get; internal set; }
        }

        public static class Layer
        {
            public static int Characters { get; internal set; }
            public static int Ground { get; internal set; }
            public static int Interactable { get; internal set; }
            public static int CharacterOnly { get; internal set; }
        }

        [SerializeField, UsedImplicitly]
        private PhysicsMaterial groundedUnitMaterial;
        [SerializeField, UsedImplicitly]
        private PhysicsMaterial slidingUnitMaterial;

        public PhysicsMaterial GroundedMaterial => groundedUnitMaterial;
        public PhysicsMaterial SlidingMaterial => slidingUnitMaterial;

        protected override void OnRegistered()
        {
            MovementUtils.Initialize();

            Layer.Characters = LayerMask.NameToLayer("Characters");
            Layer.Ground = LayerMask.NameToLayer("Ground");
            Layer.Interactable = LayerMask.NameToLayer("Interactable");
            Layer.CharacterOnly = LayerMask.NameToLayer("Character Only");

            Mask.Characters = 1 << Layer.Characters;
            Mask.Ground = 1 << Layer.Ground;
            Mask.Interactable = 1 << Layer.Interactable;
            Mask.Teleportation = Mask.Ground | (1 << Layer.CharacterOnly);
        }

        protected override void OnUnregister()
        {
            Layer.Characters = 0;
            Layer.Ground = 0;

            Mask.Characters = 0;
            Mask.Ground = 0;
        }
    }
}
