using UnityEngine;

namespace Core
{
    internal sealed class PounceMovement : MovementGenerator
    {
        private readonly Vector3 targetPoint;
        private readonly Vector3[] bezierCurvePoints = new Vector3[4];
        private readonly float pounceSpeed;
        private float currentCurveTime;
        private float curveSpeed;

        public override MovementType Type => MovementType.Charge;

        public PounceMovement(Vector3 targetPoint, float pounceSpeed)
        {
            this.targetPoint = targetPoint;
            this.pounceSpeed = pounceSpeed;
        }

        public override void Begin(Unit unit)
        {
            unit.UpdateControlState(UnitControlState.Charging, true);
            unit.Motion.UsesKinematicMovement = true;

            bezierCurvePoints[0] = unit.Position;
            bezierCurvePoints[1] = unit.Position + Vector3.up * 5f;
            bezierCurvePoints[2] = targetPoint + Vector3.up * 5f;
            bezierCurvePoints[3] = targetPoint;

            currentCurveTime = 0.0f;
            curveSpeed = pounceSpeed / Vector3.Distance(unit.Position, targetPoint);

            unit.Rotation = Quaternion.LookRotation(Vector3.ProjectOnPlane(targetPoint - unit.Position, Vector3.up));

            unit.SetMovementFlag(MovementFlags.StrafeLeft, false);
            unit.SetMovementFlag(MovementFlags.StrafeRight, false);
            unit.SetMovementFlag(MovementFlags.Flying, true);
            unit.SetMovementFlag(MovementFlags.Charging, true);
        }

        public override void Finish(Unit unit)
        {
            unit.SetMovementFlag(MovementFlags.Charging, false);
            unit.UpdateControlState(UnitControlState.Charging, false);
            unit.Motion.UsesKinematicMovement = false;

            unit.StopMoving();
        }

        public override void Reset(Unit unit)
        {
            unit.StopMoving();
        }

        public override bool Update(Unit unit, int deltaTime)
        {
            if (unit.HasAnyState(UnitControlState.Root | UnitControlState.Stunned | UnitControlState.Distracted))
            {
                unit.SetMovementFlag(MovementFlags.MaskMoving, false);
                return false;
            }

            currentCurveTime += deltaTime * curveSpeed / 1000.0f;
            if (currentCurveTime >= 1.0f)
            {
                unit.Position = bezierCurvePoints[3];
                return false;
            }

            Vector3 p0 = bezierCurvePoints[0];
            Vector3 p1 = bezierCurvePoints[1];
            Vector3 p2 = bezierCurvePoints[2];
            Vector3 p3 = bezierCurvePoints[3];
            float t = currentCurveTime;

            Vector3 bezierCurvePoint = Mathf.Pow(1 - t, 3) * p0 + Mathf.Pow(t, 3) * p3 +
                3 * Mathf.Pow(1 - t, 2) * t * p1 + 3 * (1 - t) * Mathf.Pow(t, 2) * p2;

            unit.Position = bezierCurvePoint;
            return true;
        }
    }
}