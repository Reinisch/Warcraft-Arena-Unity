using System;
using System.Collections.Generic;
using Core;

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

            public Unit BestTarget { get; private set; }

            public PlayerTargetSelector(TargetingSettings settings, Player referer, TargetingOptions options, List<Unit> previousTargets)
            {
                this.settings = settings;
                this.previousTargets = previousTargets;
                this.options = options;
                this.referer = referer;
            }

            public void Dispose()
            {
                BestTarget = null;
            }

            public void Visit(Player entity)
            {
                if (!options.EntityTypes.HasTargetFlag(TargetingEntityType.Players))
                    return;

                VisitUnit(entity);
            }

            public void Visit(Creature entity)
            {
                if (!options.EntityTypes.HasTargetFlag(TargetingEntityType.Creatures))
                    return;

                VisitUnit(entity);
            }

            private void VisitUnit(Unit unit)
            {
                if (unit == referer)
                    return;

                if (!unit.InRangeSqr(referer, settings.TargetRangeSqr))
                    return;

                int unitPreviousIndex = previousTargets.IndexOf(unit);
                if (BestTarget == null || unitPreviousIndex < bestTargetPrevousIndex)
                {
                    BestTarget = unit;
                    bestTargetPrevousIndex = unitPreviousIndex;
                }
            }
        }
    }
}
