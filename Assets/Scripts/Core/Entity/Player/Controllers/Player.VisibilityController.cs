using System.Collections.Generic;
using Common;

namespace Core
{
    public partial class Player
    {
        internal class VisibilityController : IUnitBehaviour, ILogicBehaviour
        {
            private readonly HashSet<ulong> visibleEntities = new HashSet<ulong>();

            public bool HasClientLogic => false;
            public bool HasServerLogic => true;

            public IReadOnlyCollection<ulong> VisibleEntities => visibleEntities;

            void IUnitBehaviour.DoUpdate(int deltaTime)
            {
            }

            void IUnitBehaviour.HandleUnitAttach(Unit unit)
            {
            }

            void IUnitBehaviour.HandleUnitDetach()
            {
                visibleEntities.Clear();
            }

            public bool HasClientVisiblityOf(WorldEntity target) => true;
        }
    }
}
