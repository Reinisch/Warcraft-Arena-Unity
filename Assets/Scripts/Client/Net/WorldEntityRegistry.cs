using Core;
using Net;

namespace Client
{
    /// <summary>
    /// Single-player <see cref="INetEntityRegistry"/>: a <see cref="NetId"/> is just the existing Core
    /// entity id, resolved through the live <see cref="UnitManager"/>. A real adapter replaces this with
    /// the framework's network-object id mapping — callers don't change.
    /// </summary>
    public sealed class WorldEntityRegistry : INetEntityRegistry
    {
        private readonly UnitManager unitManager;

        public WorldEntityRegistry(UnitManager unitManager)
        {
            this.unitManager = unitManager;
        }

        public NetId GetId(Unit entity) => entity != null ? new NetId(entity.Id) : NetId.None;

        public bool TryGet(NetId id, out Unit entity)
        {
            if (!id.IsValid)
            {
                entity = null;
                return false;
            }

            return unitManager.TryFind(id.Value, out entity);
        }
    }
}
