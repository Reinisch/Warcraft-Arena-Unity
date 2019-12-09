namespace Core
{
    internal sealed partial class MapGrid
    {
        private class CreatureVisibilityNotifier : IUnitVisitor
        {
            private readonly MapGrid mapGrid;
            private Creature creature;

            public CreatureVisibilityNotifier(MapGrid mapGrid) => this.mapGrid = mapGrid;

            public void Configure(Creature creature) => this.creature = creature;

            public void Complete() => creature = null;
            
            private void HandleUnitVisibility(Unit target) => mapGrid.visibilityChangedEntities.Add(target);

            void IUnitVisitor.Visit(Player player)
            {
                HandleUnitVisibility(player);

                if (!player.IsVisibilityChanged)
                {
                    player.Visibility.UpdateVisibilityOf(creature);
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