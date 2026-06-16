using System.Collections.Generic;

namespace Core
{
    internal sealed partial class MapGrid
    {
        private class PlayerVisibilityNotifier : IUnitVisitor
        {
            private readonly MapGrid mapGrid;
            private bool forceUpdateOthers;

            public PlayerVisibilityNotifier(MapGrid mapGrid)
            {
                this.mapGrid = mapGrid;
            }

            public void Configure(Player player, bool forceUpdateOthers)
            {
                this.forceUpdateOthers = forceUpdateOthers;
            }

            public void Complete()
            {
            }

            private void HandleUnitVisibility(Unit target)
            {
                mapGrid.visibilityChangedEntities.Add(target);
            }

            void IUnitVisitor.Visit(Player player)
            {
                HandleUnitVisibility(player);

                if (forceUpdateOthers || !player.IsVisibilityChanged)
                {
                    mapGrid.visibilityChangedEntities.Add(player);
                }
            }

            void IUnitVisitor.Visit(Creature creature)
            {
                HandleUnitVisibility(creature);
            }
        }
    }
}