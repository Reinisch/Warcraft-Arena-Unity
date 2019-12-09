using System.Collections.Generic;
using Common;

namespace Core
{
    public partial class Player
    {
        internal class VisibilityController : IUnitBehaviour
        {
            private readonly HashSet<ulong> visibleEntities = new HashSet<ulong>();

            private Player player;

            public bool HasClientLogic => false;
            public bool HasServerLogic => true;

            public IReadOnlyCollection<ulong> VisibleEntities => visibleEntities;

            void IUnitBehaviour.DoUpdate(int deltaTime)
            {
            }

            void IUnitBehaviour.HandleUnitAttach(Unit unit)
            {
                player = (Player)unit;
            }

            void IUnitBehaviour.HandleUnitDetach()
            {
                visibleEntities.Clear();
                player = null;
            }

            public bool HasClientVisiblityOf(WorldEntity target) => visibleEntities.Contains(target.Id) || target == player;

            public void SetScopeOf(WorldEntity target, bool inScope)
            {
                if (player.BoltEntity.Controller != null)
                {
                    if (inScope)
                        target.PrepareForScoping();

                    target.BoltEntity.SetScope(player.BoltEntity.Controller, inScope);
                }

                if (player.IsLocalServerPlayer)
                    EventHandler.ExecuteEvent(player.World, GameEvents.ServerVisibilityChanged, target, inScope);
            }

            public void ScopeOutOf(IReadOnlyList<ulong> targets)
            {
                for (int i = 0; i < targets.Count; i++)
                {
                    ulong invisibleEntityId = targets[i];
                    visibleEntities.Remove(invisibleEntityId);

                    if (player.World.UnitManager.TryFind(invisibleEntityId, out Unit target))
                        SetScopeOf(target, false);
                }
            }

            public void UpdateVisibilityOf(WorldEntity target)
            {
                if (HasClientVisiblityOf(target))
                {
                    if (!player.CanSeeOrDetect(target, false, true))
                    {
                        SetScopeOf(target, false);

                        visibleEntities.Remove(target.Id);
                    }
                }
                else
                {
                    if (player.CanSeeOrDetect(target, false, true))
                    {
                        SetScopeOf(target, true);

                        visibleEntities.Add(target.Id);
                    }
                }
            }
        }
    }
}
