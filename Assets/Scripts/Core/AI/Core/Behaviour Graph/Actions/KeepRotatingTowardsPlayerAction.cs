using System;
using Unity.Behavior;
using Unity.Properties;
using UnityEngine;

namespace Core.BehaviorGraph
{
    [Serializable, GeneratePropertyBag]
    [NodeDescription(
        name: "Keep Rotating Towards Player",
        description: "Continuously rotates the unit towards the nearest player each frame. Returns Running indefinitely.",
        story: "[Unit] rotates to player with speed [RotationSpeed]",
        category: "Action/Unit",
        id: "f6a7b8c9d0e1f2a3b4c5d6e7f8a9b0c2")]
    public class KeepRotatingTowardsPlayerAction : BehaviourGraphAction
    {
        [SerializeReference] public BlackboardVariable<Unit> Unit;
        [SerializeReference] public BlackboardVariable<float> RotationSpeed;
        [SerializeReference] public BlackboardVariable<bool> AliveTargets;

        protected override Status OnStart() => Status.Running;

        protected override Status OnUpdate()
        {
            Player player = Unit.Value.World.UnitManager.FindNearby<Player>(Unit.Value.Position, TargetPredicate);
            if (player == null)
                return Status.Running;

            Vector3 direction = player.Position - Unit.Value.Position;
            direction.y = 0;
            if (direction.sqrMagnitude > Mathf.Epsilon)
                Unit.Value.CharacterController.Motor.SetRotation(
                    Quaternion.RotateTowards(
                        Unit.Value.Rotation,
                        Quaternion.LookRotation(direction),
                        RotationSpeed.Value * Time.deltaTime));

            return Status.Running;
        }

        private bool TargetPredicate(Player player)
        {
            if (AliveTargets.Value && player.IsDead)
                return false;

            return true;
        }
    }
}
