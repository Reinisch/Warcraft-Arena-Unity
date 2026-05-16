using System;
using Core;
using JetBrains.Annotations;
using System.Collections.Generic;
using UnityEngine;

namespace Client
{
    [RequireComponent(typeof(Camera))]
    public class WarcraftCamera : MonoBehaviour
    {
        [Serializable]
        private class WarcraftCameraMovementModeType
        {
            [field:SerializeField]
            public WarcraftCameraMovementMode Mode { get; private set; }

            [field: SerializeField]
            public MovementMode Type { get; private set; }
        }

        [SerializeField, UsedImplicitly]
        private InputReference input;

        [SerializeField, UsedImplicitly]
        private List<WarcraftCameraMovementModeType> movementModes = new();

        [SerializeField, UsedImplicitly]
        private float targetHeight = 1.7f;
        [SerializeField, UsedImplicitly]
        private float deadTargetHeight = 0.5f;
        [SerializeField, UsedImplicitly]
        private float distance = 5.0f;
        [SerializeField, UsedImplicitly]
        private float offsetFromWall = 0.1f;

        [SerializeField, UsedImplicitly]
        private float maxDistance = 20;
        [SerializeField, UsedImplicitly]
        private float minDistance = 0.6f;

        [SerializeField, UsedImplicitly]
        private float xSpeed = 200.0f;
        [SerializeField, UsedImplicitly]
        private float ySpeed = 200.0f;
        [SerializeField, UsedImplicitly]
        private int yMinLimit = -40;
        [SerializeField, UsedImplicitly]
        private int yMaxLimit = 80;
        [SerializeField, UsedImplicitly]
        private int zoomRate = 40;

        [SerializeField, UsedImplicitly]
        private float targetHeightDampening = 3.0f;
        [SerializeField, UsedImplicitly]
        private float rotationDampening = 3.0f;
        [SerializeField, UsedImplicitly]
        private float zoomDampening = 5.0f;
        [SerializeField, UsedImplicitly]
        private float targetSmoothTime = 0.05f;
        [SerializeField, UsedImplicitly]
        private LayerMask collisionLayers = -1;
        [SerializeField, UsedImplicitly, HideInInspector]
        private Camera targetCamera;

        private readonly Dictionary<MovementMode, WarcraftCameraMovementMode> movementModesByType = new();

        private Unit target;
        private Vector3 targetPosition;
        private Vector3 targetPositionVelocity;

        private float xDeg;
        private float yDeg;
        private float currentDistance;
        private float desiredDistance;
        private float correctedDistance;
        private float currentActualHeight;

        public float RotationDampening => rotationDampening;
        public float MaxDistance => maxDistance;
        public float MinDistance => minDistance;
        public float SpeedX => xSpeed;
        public float SpeedY => ySpeed;
        public int ZoomRate => zoomRate;
        public Camera Camera => targetCamera;

        public Unit Target
        {
            get => target;
            set
            {
                target = value;
                currentActualHeight = target == null || target.IsAlive ? targetHeight : deadTargetHeight;

                if (target != null)
                    UpdateTargetPosition(true);
            }
        }

        [UsedImplicitly]
        private void OnValidate()
        {
            targetCamera = GetComponent<Camera>();
        }

        [UsedImplicitly]
        private void Awake()
        {
            movementModes.ForEach(item => movementModesByType.Add(item.Type, item.Mode));
        }

        [UsedImplicitly]
        private void Start()
        {
            Vector3 angles = transform.eulerAngles;
            xDeg = angles.x;
            yDeg = angles.y;

            currentDistance = distance;
            desiredDistance = distance;
            correctedDistance = distance;
        }

        [UsedImplicitly]
        private void LateUpdate()
        {
            if (!target)
                return;

            UpdateTargetPosition(false);

            movementModesByType[target.MovementMode].PollInput(this, Time.deltaTime, ref desiredDistance, ref xDeg, ref yDeg);

            desiredDistance = Mathf.Clamp(desiredDistance, minDistance, maxDistance);

            yDeg = ClampAngle(yDeg, yMinLimit, yMaxLimit);

            // set camera rotation
            Quaternion rotation = Quaternion.Euler(yDeg, xDeg, 0);
            correctedDistance = desiredDistance;

            // calculate desired camera position
            currentActualHeight = Mathf.MoveTowards(currentActualHeight, target.IsAlive ? targetHeight : deadTargetHeight, targetHeightDampening * Time.deltaTime);
            var vTargetOffset = new Vector3(0, -currentActualHeight, 0);
            Vector3 position = targetPosition - (rotation * Vector3.forward * desiredDistance + vTargetOffset);

            // check for collision using the true target's desired registration point as set by user using height
            Vector3 trueTargetPosition = targetPosition - vTargetOffset;

            // if there was a collision, correct the camera position and calculate the corrected distance
            bool isCorrected = false;
            if (Physics.Linecast(trueTargetPosition, position, out var collisionHit, collisionLayers.value))
            {
                // calculate the distance from the original estimated position to the collision location,
                // subtracting out a safety "offset" distance from the object we hit.  The offset will help
                // keep the camera from being right on top of the surface we hit, which usually shows up as
                // the surface geometry getting partially clipped by the camera's front clipping plane.
                correctedDistance = Vector3.Distance(trueTargetPosition, collisionHit.point) - offsetFromWall;
                isCorrected = true;
            }

            // For smoothing, lerp distance only if either distance wasn't corrected, or correctedDistance is more than currentDistance
            currentDistance = !isCorrected || correctedDistance > currentDistance ?
                Mathf.Lerp(currentDistance, correctedDistance, Time.deltaTime * zoomDampening) :
                correctedDistance;

            // keep within legal limits
            currentDistance = Mathf.Clamp(currentDistance, minDistance, maxDistance);

            // recalculate position based on the new currentDistance
            position = targetPosition - (rotation * Vector3.forward * currentDistance + vTargetOffset);

            transform.rotation = rotation;
            transform.position = position;
        }

        private void UpdateTargetPosition(bool instantly)
        {
            targetPosition = instantly ? target.Position : Vector3.SmoothDamp(targetPosition, target.Position, ref targetPositionVelocity, targetSmoothTime);
        }

        private static float ClampAngle(float angle, float min, float max)
        {
            if (angle < -360)
                angle += 360;
            if (angle > 360)
                angle -= 360;
            return Mathf.Clamp(angle, min, max);
        }
    }
}
