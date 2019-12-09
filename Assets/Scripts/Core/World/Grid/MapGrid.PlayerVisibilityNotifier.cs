using System.Collections.Generic;

namespace Core
{
    internal sealed partial class MapGrid
    {
        private class PlayerVisibilityNotifier : IUnitVisitor
        {
            private readonly MapGrid mapGrid;
            private readonly List<ulong> unhandledEntities = new List<ulong>();
            private Player player;
            private bool forceUpdateOthers;

            public PlayerVisibilityNotifier(MapGrid mapGrid)
            {
                this.mapGrid = mapGrid;
            }

            public void Configure(Player player, bool forceUpdateOthers)
            {
                this.player = player;
                this.forceUpdateOthers = forceUpdateOthers;

                unhandledEntities.AddRange(player.Visibility.VisibleEntities);
                unhandledEntities.Remove(player.Id);
            }

            public void Complete()
            {
                player.Visibility.ScopeOutOf(unhandledEntities);
                unhandledEntities.Clear();
                player = null;
            }

            private void HandleUnitVisibility(Unit target)
            {
                unhandledEntities.Remove(target.Id);

                player.Visibility.UpdateVisibilityOf(target);
                mapGrid.visibilityChangedEntities.Add(target);
            }

            void IUnitVisitor.Visit(Player player)
            {
                HandleUnitVisibility(player);

                if (forceUpdateOthers || !player.IsVisibilityChanged)
                {
                    player.Visibility.UpdateVisibilityOf(this.player);
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