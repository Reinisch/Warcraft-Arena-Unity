using Core;
using JetBrains.Annotations;
using UnityEngine;

namespace Client
{
    [RequireComponent(typeof(Camera))]
    public class WarcraftCamera : MonoBehaviour
    {
        [SerializeField, UsedImplicitly]
        private InputReference input;

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
        private float speedDistance = 5;

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

        private Unit target;
        private Vector3 targetPosition;
        private Vector3 targetPositionVelocity;

        private float xDeg;
        private float yDeg;
        private float currentDistance;
        private float desiredDistance;
        private float correctedDistance;
        private float currentActualHeight;

        public Camera Camera => targetCamera;

        public Unit Target
        {
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
            currentActualHeight = Mathf.MoveTowards(currentActualHeight, target.IsAlive ? targetHeight : deadTargetHeight, targetHeightDampening * Time.deltaTime);

            // If either mouse buttons are down, let the mouse govern camera position
            if (GUIUtility.hotControl == 0)
            {
                if (Input.GetMouseButton(0) && !InterfaceUtils.IsPointerOverUI || Input.GetMouseButton(1))
                {
                    xDeg += Input.GetAxis("Mouse X") * xSpeed * 0.02f;
                    yDeg -= Input.GetAxis("Mouse Y") * ySpeed * 0.02f;
                }
                // otherwise, ease behind the target if any of the directional keys are pressed
                else if (!Mathf.Approximately(Input.GetAxis("Vertical"), 0) || !Mathf.Approximately(Input.GetAxis("Horizontal"), 0))
                {
                    if (target.IsAlive && input.IsPlayerInputAllowed)
                    {
                        float targetRotationAngle = target.transform.eulerAngles.y;
                        float currentRotationAngle = transform.eulerAngles.y;
                        xDeg = Mathf.LerpAngle(currentRotationAngle, targetRotationAngle, rotationDampening * Time.deltaTime);
                    }
                }
            }

            // calculate the desired distance
            if (!InterfaceUtils.IsPointerOverUI)
                desiredDistance -= Input.GetAxis("Mouse ScrollWheel") * Time.deltaTime * zoomRate * Mathf.Abs(desiredDistance) * speedDistance;

            desiredDistance = Mathf.Clamp(desiredDistance, minDistance, maxDistance);

            yDeg = ClampAngle(yDeg, yMinLimit, yMaxLimit);

            // set camera rotation
            Quaternion rotation = Quaternion.Euler(yDeg, xDeg, 0);
            correctedDistance = desiredDistance;

            // calculate desired camera position
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
