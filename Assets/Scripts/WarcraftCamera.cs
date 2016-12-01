using UnityEngine;
using System.Collections;

public class WarcraftCamera : MonoBehaviour
{
	public float targetHeight = 1.7f;
	public float distance = 5.0f;
	public float offsetFromWall = 0.1f;

	public float maxDistance = 20;
	public float minDistance = .6f;
	public float speedDistance = 5;

	public float xSpeed = 200.0f;
	public float ySpeed = 200.0f;

	public int yMinLimit = -40;
	public int yMaxLimit = 80;

	public int zoomRate = 40;

	public float rotationDampening = 3.0f;
	public float zoomDampening = 5.0f;

	public LayerMask collisionLayers = -1;

    [SerializeField]
    Transform target;
    float xDeg = 0.0f;
	float yDeg = 0.0f;
	float currentDistance;
	float desiredDistance;
	float correctedDistance;

    public Transform Target
    {
        get
        {
            return target;
        }
        set
        {
            target = value;
        }
    }


	void Start ()
	{
		Vector3 angles = transform.eulerAngles;
		xDeg = angles.x;
		yDeg = angles.y;

		currentDistance = distance;
		desiredDistance = distance;
		correctedDistance = distance;
	}

	void LateUpdate ()
	{
		if (!target)
			return;

        Vector3 vTargetOffset;

        // If either mouse buttons are down, let the mouse govern camera position
        if (GUIUtility.hotControl == 0) {
			if (Input.GetMouseButton(0) || Input.GetMouseButton(1))
			{
				xDeg += Input.GetAxis ("Mouse X") * xSpeed * 0.02f;
				yDeg -= Input.GetAxis ("Mouse Y") * ySpeed * 0.02f;
			}

			// otherwise, ease behind the target if any of the directional keys are pressed
			else if (Input.GetAxis("Vertical") != 0 || Input.GetAxis("Horizontal") != 0)
			{
				float targetRotationAngle = target.eulerAngles.y;
				float currentRotationAngle = transform.eulerAngles.y;
				xDeg = Mathf.LerpAngle (currentRotationAngle, targetRotationAngle, rotationDampening * Time.deltaTime);
			}
		}


		// calculate the desired distance
		desiredDistance -= Input.GetAxis ("Mouse ScrollWheel") * Time.deltaTime * zoomRate * Mathf.Abs (desiredDistance) * speedDistance;
		desiredDistance = Mathf.Clamp (desiredDistance, minDistance, maxDistance);

		yDeg = ClampAngle(yDeg, yMinLimit, yMaxLimit);

		// set camera rotation
		Quaternion rotation = Quaternion.Euler(yDeg, xDeg, 0);
		correctedDistance = desiredDistance;

		// calculate desired camera position
		vTargetOffset = new Vector3 (0, -targetHeight, 0);
		Vector3 position = target.position - (rotation * Vector3.forward * desiredDistance + vTargetOffset);

		// check for collision using the true target's desired registration point as set by user using height
		RaycastHit collisionHit;
		Vector3 trueTargetPosition = new Vector3(target.position.x, target.position.y, target.position.z) - vTargetOffset;

		// if there was a collision, correct the camera position and calculate the corrected distance
		bool isCorrected = false;
		if (Physics.Linecast (trueTargetPosition, position, out collisionHit, collisionLayers.value))
		{
			// calculate the distance from the original estimated position to the collision location,
			// subtracting out a safety "offset" distance from the object we hit.  The offset will help
			// keep the camera from being right on top of the surface we hit, which usually shows up as
			// the surface geometry getting partially clipped by the camera's front clipping plane.
			correctedDistance = Vector3.Distance (trueTargetPosition, collisionHit.point) - offsetFromWall;
			isCorrected = true;
		}

		// For smoothing, lerp distance only if either distance wasn't corrected, or correctedDistance is more than currentDistance
		currentDistance = !isCorrected || correctedDistance > currentDistance ?
            Mathf.Lerp (currentDistance, correctedDistance, Time.deltaTime * zoomDampening) :
            correctedDistance;

		// keep within legal limits
		currentDistance = Mathf.Clamp (currentDistance, minDistance, maxDistance);

		// recalculate position based on the new currentDistance
		position = target.position - (rotation * Vector3.forward * currentDistance + vTargetOffset);

		transform.rotation = rotation;
		transform.position = position;
	}


	private static float ClampAngle (float angle, float min, float max)
	{
		if (angle < -360)
			angle += 360;
		if (angle > 360)
			angle -= 360;
		return Mathf.Clamp (angle, min, max);
	}
}