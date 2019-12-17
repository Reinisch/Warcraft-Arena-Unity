using Common;
using UnityEngine;
using UnityEngine.AI;

namespace Core
{
    internal sealed class ConfusedMovement : MovementGenerator
    {
        private TimeTracker nextMoveTime = new TimeTracker();
        private readonly NavMeshPath confusedNavMeshPath = new NavMeshPath();

        public override MovementType Type => MovementType.Confused;

        public override void Begin(Unit unit)
        {
            nextMoveTime.Reset(10);

            unit.AI.NavMeshAgentEnabled = true;
            unit.AI.UpdatePosition = false;
        }

        public override void Finish(Unit unit)
        {
            unit.AI.NavMeshAgentEnabled = false;
            unit.RemoveState(UnitControlState.ConfusedMove);
            unit.StopMoving();

            nextMoveTime.Reset(0);
        }

        public override void Reset(Unit unit)
        {
            nextMoveTime.Reset(0);
            unit.StopMoving();
        }

        public override bool Update(Unit unit, int deltaTime)
        {
            bool cantMove = unit.HasState(UnitControlState.Root | UnitControlState.Stunned | UnitControlState.Distracted);
            unit.AI.UpdateRotation = !cantMove;

            if (cantMove)
            {
                unit.AI.NextPosition = unit.Position;
                unit.MovementInfo.RemoveMovementFlag(MovementFlags.MaskMoving);
            }
            else if (unit.AI.HasPath)
            {
                Vector3 localDirection = unit.transform.TransformDirection(unit.AI.NextPosition - unit.Position);

                unit.MovementInfo.SetMovementFlag(MovementFlags.Forward, localDirection.z > MovementUtils.DirectionalMovementThreshold);
                unit.MovementInfo.SetMovementFlag(MovementFlags.Backward, localDirection.z < -MovementUtils.DirectionalMovementThreshold);
                unit.MovementInfo.SetMovementFlag(MovementFlags.StrafeLeft, localDirection.x < -MovementUtils.DirectionalMovementThreshold);
                unit.MovementInfo.SetMovementFlag(MovementFlags.StrafeRight, localDirection.x > MovementUtils.DirectionalMovementThreshold);

                unit.Position = unit.AI.NextPosition;
            }

            if (cantMove)
                return true;

            if (nextMoveTime.Passed)
                nextMoveTime.Reset(RandomUtils.Next(1000, 3000));
            else
            {
                nextMoveTime.Update(deltaTime);
                if (nextMoveTime.Passed)
                {
                    unit.AddState(UnitControlState.ConfusedMove);

                    Vector2 randomCircle = Random.insideUnitCircle * 4;
                    Vector3 randomPosition = unit.Position + new Vector3(randomCircle.x, 0, randomCircle.y);
                    if (!NavMesh.SamplePosition(randomPosition, out NavMeshHit hit, MovementUtils.MaxNavMeshSampleRange, MovementUtils.WalkableAreaMask))
                        return TryAgainSoon();

                    randomPosition = hit.position;
                    if (!NavMesh.CalculatePath(unit.Position, randomPosition, MovementUtils.WalkableAreaMask, confusedNavMeshPath))
                        return TryAgainSoon();

                    if (!unit.AI.SetPath(confusedNavMeshPath))
                        return TryAgainSoon();

                    if (unit.AI.RemainingDistance > MovementUtils.MaxConfusedPath)
                        return TryAgainSoon();
                }
            }

            return true;

            bool TryAgainSoon()
            {
                nextMoveTime.Reset(100);
                return true;
            }
        }
    }
}