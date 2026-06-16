using UnityEngine;

namespace Game.Core.CharacterController
{
    public interface ICharacterController
    {
        /// <summary>
        /// This is called when the motor wants to know what its rotation should be right now
        /// </summary>
        void UpdateRotation(ref Quaternion currentRotation, float deltaTime);
        /// <summary>
        /// This is called when the motor wants to know what its velocity should be right now
        /// </summary>
        void UpdateVelocity(ref Vector3 currentVelocity, float deltaTime);
        /// <summary>
        /// This is called before the motor does anything
        /// </summary>
        void BeforeCharacterUpdate(float deltaTime);
        /// <summary>
        /// This is called after the motor has finished its ground probing, but before PhysicsMover/Velocity/etc.... handling
        /// </summary>
        void PostGroundingUpdate(float deltaTime);
        /// <summary>
        /// This is called after the motor has finished everything in its update
        /// </summary>
        void AfterCharacterUpdate(float deltaTime);
        /// <summary>
        /// This is called after when the motor wants to know if the collider can be collided with (or if we just go through it)
        /// </summary>
        bool IsColliderValidForCollisions(Collider coll);
        /// <summary>
        /// This is called when the motor's ground probing detects a ground hit
        /// </summary>
        void OnGroundHit(Collider hitCollider, Vector3 hitNormal, Vector3 hitPoint, ref HitStabilityReport hitStabilityReport) { }
        /// <summary>
        /// This is called when the motor's movement logic detects a hit
        /// </summary>
        void OnMovementHit(Collider hitCollider, Vector3 hitNormal, Vector3 hitPoint, ref HitStabilityReport hitStabilityReport) { }
        /// <summary>
        /// This is called after every move hit, to give you an opportunity to modify the HitStabilityReport to your liking
        /// </summary>
        void ProcessHitStabilityReport(Collider hitCollider, Vector3 hitNormal, Vector3 hitPoint, Vector3 atCharacterPosition, Quaternion atCharacterRotation, ref HitStabilityReport hitStabilityReport) { }
        /// <summary>
        /// This is called when the character detects discrete collisions (collisions that don't result from the motor's capsuleCasts when moving)
        /// </summary>
        void OnDiscreteCollisionDetected(Collider hitCollider) { }
    }
}