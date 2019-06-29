using JetBrains.Annotations;
using UnityEngine;

namespace Core
{
    public class GroundChecker : MonoBehaviour
    {
        public int GroundCollisions { get; private set; }

        [UsedImplicitly]
        private void OnTriggerEnter(Collider collider)
        {
            if (collider.gameObject.layer == PhysicsReference.Layer.Ground)
                GroundCollisions++;
        }

        [UsedImplicitly]
        private void OnTriggerExit(Collider collider)
        {
            if (collider.gameObject.layer == PhysicsReference.Layer.Ground)
                GroundCollisions--;
        }
    }
}
