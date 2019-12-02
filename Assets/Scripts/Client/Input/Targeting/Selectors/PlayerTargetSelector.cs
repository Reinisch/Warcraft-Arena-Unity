using System;
using System.Collections.Generic;
using Core;
using UnityEngine;

namespace Client
{
    public partial class TargetingReference
    {
        private class PlayerTargetSelector : IUnitVisitor, IDisposable
        {
            private readonly TargetingSettings settings;
            private readonly List<Unit> previousTargets;
            private readonly TargetingOptions options;
            private readonly Player referer;

            private int bestTargetPrevousIndex;
            private float bestTargetDistance;

            public Unit BestTarget { get; private set; }

            public PlayerTargetSelector(TargetingSettings settings, Player referer, TargetingOptions options, List<Unit> previousTargets)
            {
                this.settings = settings;
                this.previousTargets = previousTargets;
                this.options = options;
                this.referer = referer;

                switch (options.Distance)
                {
                    case TargetingDistance.Any:
                        bestTargetPrevousIndex = int.MaxValue;
                        break;
                    case TargetingDistance.Near:
                        bestTargetDistance = float.MaxValue;
                        break;
                    case TargetingDistance.Far:
                        bestTargetDistance = float.MinValue;
                        break;
                    default:
                        throw new ArgumentOutOfRangeException(nameof(options.Distance));
                }
            }

            public void Dispose()
            {
                BestTarget = null;
            }

            public void Visit(Player player)
            {
                if (!options.EntityTypes.HasTargetFlag(TargetingEntityType.Players))
                    return;

                VisitUnit(player);
            }

            public void Visit(Creature creature)
            {
                if (!options.EntityTypes.HasTargetFlag(TargetingEntityType.Creatures))
                    return;

                VisitUnit(creature);
            }

            private void VisitUnit(Unit unit)
            {
                if (unit == referer)
                    return;

                if (!unit.InRangeSqr(referer, settings.TargetRangeSqr))
                    return;

                if (!options.DeathState.HasTargetFlag(TargetingDeathState.Alive) && unit.IsAlive)
                    return;

                if (!options.DeathState.HasTargetFlag(TargetingDeathState.Dead) && unit.IsDead)
                    return;

                if (options.MaxReferringAngle < 180.0f)
                {
                    Vector3 refererDirection = Vector3.ProjectOnPlane(referer.transform.forward, Vector3.up);
                    Vector3 targetDirection = Vector3.ProjectOnPlane(unit.Position - referer.Position, Vector3.up);
                    if (Vector3.Angle(refererDirection, targetDirection) > options.MaxReferringAngle)
                        return;
                }

                SelectBestTarget(unit);
            }

            private void SelectBestTarget(Unit nextTarget)
            {
                switch (options.Distance)
                {
                    case TargetingDistance.Any:
                        int unitPreviousIndex = previousTargets.IndexOf(nextTarget);
                        if (BestTarget == null || unitPreviousIndex < bestTargetPrevousIndex)
                        {
                            BestTarget = nextTarget;
                            bestTargetPrevousIndex = unitPreviousIndex;
                        }
                        break;
                    case TargetingDistance.Near:
                        float nextTargetNearDistance = referer.ExactDistanceTo(nextTarget);
                        if (BestTarget == null || nextTargetNearDistance < bestTargetDistance)
                        {
                            BestTarget = nextTarget;
                            bestTargetDistance = nextTargetNearDistance;
                        }
                        break;
                    case TargetingDistance.Far:
                        float nextTargetFarDistance = referer.ExactDistanceTo(nextTarget);
                        if (BestTarget == null || nextTargetFarDistance > bestTargetDistance)
                        {
                            BestTarget = nextTarget;
                            bestTargetDistance = nextTargetFarDistance;
                        }
                        break;
                    default:
                        throw new ArgumentOutOfRangeException(nameof(options.Distance));
                }
            }
        }
    }
}
