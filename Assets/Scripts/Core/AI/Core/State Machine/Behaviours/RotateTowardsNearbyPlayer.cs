using Common;
using JetBrains.Annotations;
using System.Collections.Generic;
using UnityEngine;

namespace Core
{
    [UsedImplicitly]
    public class RotateTowardsNearbyPlayer: UnitStateMachineBehaviour
    {
        [SerializeField, UsedImplicitly] private float rotationSpeed;
        [SerializeField, UsedImplicitly] private bool aliveTargets;

        protected override void OnActiveUpdate(int deltaTime)
        {
            base.OnActiveUpdate(deltaTime);

            Player player = Unit.World.UnitManager.FindNearby<Player>(Unit.Position, TargetPredicate);
            if (player == null)
                return;

            Vector3 direction = player.Position - Unit.Position;
            direction.y = 0;
            Unit.Rotation = Quaternion.RotateTowards(Unit.Rotation, Quaternion.LookRotation(direction), rotationSpeed * deltaTime / 1000);
        }

        private bool TargetPredicate(Player player)
        {
            if (aliveTargets && player.IsDead)
                return false;

            return true;
        }

    }
}