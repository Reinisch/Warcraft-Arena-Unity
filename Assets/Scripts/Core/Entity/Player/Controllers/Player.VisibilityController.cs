using System.Collections.Generic;
using UnityEngine;

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

            public bool HasClientVisiblityOf(WorldEntity target) => visibleEntities.Contains(target.Id);

            public void SetScopeOf(WorldEntity target, bool inScope)
            {
                if (player.BoltEntity.Controller != null)
                    target.BoltEntity.SetScope(player.BoltEntity.Controller, inScope);
            }

            public void ScopeOutOf(IEnumerable<ulong> targets)
            {
                foreach (ulong invisibleEntityId in targets)
                {
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
                        target.PrepareForScoping();

                        SetScopeOf(target, true);

                        visibleEntities.Add(target.Id);
                    }
                }
            }
        }
    }
}
