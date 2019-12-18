using UnityEngine;
using UnityEngine.AI;

namespace Core
{
    internal sealed class ChargeMovement : MovementGenerator
    {
        private readonly Vector3 targetPoint;
        private readonly float chargeSpeed;
        private bool startedMovement;

        public override MovementType Type => MovementType.Charge;

        public ChargeMovement(Vector3 targetPoint, float chargeSpeed)
        {
            this.targetPoint = targetPoint;
            this.chargeSpeed = chargeSpeed;
        }

        public override void Begin(Unit unit)
        {
            unit.AI.NavMeshAgentEnabled = true;
            unit.AI.UpdatePosition = false;
            unit.AI.UpdateRotation = true;
            unit.AI.Speed = chargeSpeed;
            unit.AI.AngularSpeed = MovementUtils.ChargeRotationSpeed;

            unit.UpdateControlState(UnitControlState.Charging, true);
        }

        public override void Finish(Unit unit)
        {
            unit.UpdateControlState(UnitControlState.Charging, false);

            unit.AI.NavMeshAgentEnabled = false;
            unit.StopMoving();
        }

        public override void Reset(Unit unit)
        {
            unit.StopMoving();
        }

        public override bool Update(Unit unit, int deltaTime)
        {
            if (!startedMovement)
            {
                startedMovement = true;

                if (unit is Creature)
                    return unit.AI.SetDestination(targetPoint);

                NavMeshPath chargePath = new NavMeshPath();
                if (NavMesh.CalculatePath(unit.Position, targetPoint, MovementUtils.WalkableAreaMask, chargePath))
                    return unit.AI.SetPath(chargePath);

                return false;
            }

            if (unit.HasAnyState(UnitControlState.Root | UnitControlState.Stunned | UnitControlState.Distracted))
            {
                unit.AI.NextPosition = unit.Position;
                unit.MovementInfo.RemoveMovementFlag(MovementFlags.MaskMoving);
                return false;
            }

            if (unit.AI.HasPendingPath)
                return true;

            if (!unit.AI.HasPath)
                return false;

            unit.Position = unit.AI.NextPosition;
            unit.MovementInfo.SetMovementFlag(MovementFlags.Forward, true);

            return unit.AI.RemainingDistance > MovementUtils.PointArrivalRange;
        }
    }
}