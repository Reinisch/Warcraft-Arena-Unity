using System.Collections.Generic;
using Core;

namespace Net.Ngo
{
    /// <summary>
    /// NGO <see cref="INetEntityRegistry"/>: a <see cref="NetId"/> is the NGO <c>NetworkObjectId</c>.
    /// Populated by <see cref="EntityNetworkView"/> as networked entities spawn/despawn, so server and
    /// client resolve the same ids (replaces the single-player <c>WorldEntityRegistry</c> in the NGO path).
    /// </summary>
    public sealed class NgoEntityRegistry : INetEntityRegistry
    {
        private readonly Dictionary<ulong, Unit> unitsById = new Dictionary<ulong, Unit>();
        private readonly Dictionary<Unit, NetId> idsByUnit = new Dictionary<Unit, NetId>();

        internal void Register(NetId id, Unit unit)
        {
            if (unit == null)
                return;

            unitsById[id.Value] = unit;
            idsByUnit[unit] = id;
        }

        internal void Unregister(NetId id, Unit unit)
        {
            unitsById.Remove(id.Value);
            if (unit != null)
                idsByUnit.Remove(unit);
        }

        public NetId GetId(Unit entity) =>
            entity != null && idsByUnit.TryGetValue(entity, out NetId id) ? id : NetId.None;

        public bool TryGet(NetId id, out Unit entity)
        {
            if (!id.IsValid)
            {
                entity = null;
                return false;
            }

            return unitsById.TryGetValue(id.Value, out entity);
        }
    }
}
