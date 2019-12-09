using UnityEngine;

namespace Core
{
    internal sealed partial class MapGrid
    {
        private class CellRelocator : IUnitVisitor
        {
            private readonly MapGrid mapGrid;

            public CellRelocator(MapGrid mapGrid)
            {
                this.mapGrid = mapGrid;
            }

            private bool IsOutOfCellBounds(Vector3 position, Cell cell)
            {
                if (position.x + MovementUtils.GridCellSwitchDifference < cell.MinBounds.x)
                    return true;

                if (position.x > cell.MaxBounds.x + MovementUtils.GridCellSwitchDifference)
                    return true;

                if (position.z + MovementUtils.GridCellSwitchDifference < cell.MinBounds.z)
                    return true;

                if (position.z > cell.MaxBounds.z + MovementUtils.GridCellSwitchDifference)
                    return true;

                return false;
            }

            private void HandleRelocation(Unit unit)
            {
                if (unit.Position.y > MovementUtils.MaxHeight || unit.Position.y < MovementUtils.MinHeight)
                    unit.Position = mapGrid.map.Settings.DefaultSpawnPoint.position;

                if (IsOutOfCellBounds(unit.Position, unit.CurrentCell))
                    mapGrid.relocatableEntities.Add(unit);
            }

            void IUnitVisitor.Visit(Player player)
            {
                if (player.World.HasServerLogic && (player.IsVisibilityChanged || mapGrid.gridCellOutOfRangeTimer.Passed))
                {
                    mapGrid.UpdateVisibility(player, false);
                    mapGrid.visibilityChangedEntities.Add(player);
                }

                HandleRelocation(player);
            }

            void IUnitVisitor.Visit(Creature creature)
            {
                if (creature.World.HasServerLogic && creature.IsVisibilityChanged)
                {
                    mapGrid.UpdateVisibility(creature);
                    mapGrid.visibilityChangedEntities.Add(creature);
                }

                HandleRelocation(creature);
            }
        }
    }
}