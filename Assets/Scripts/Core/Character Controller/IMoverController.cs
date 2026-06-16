using UnityEngine;

namespace Game.Core.CharacterController
{
    public interface IMoverController
    {
        /// <summary>
        /// This is called to let you tell the PhysicsMover where it should be right now
        /// </summary>
        void UpdateMovement(out Vector3 goalPosition, out Quaternion goalRotation, float deltaTime);
    }
}