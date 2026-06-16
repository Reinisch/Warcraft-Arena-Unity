using Core;

namespace Net
{
    /// <summary>
    /// Resolves between a <see cref="NetId"/> and the game entity it refers to. In single-player the id
    /// is simply the existing Core entity id; a real adapter maps it to the framework's network id.
    /// Lets message handlers reference entities over the wire without binding to Core's lookups directly.
    /// </summary>
    public interface INetEntityRegistry
    {
        /// <summary>The network id for an entity (<see cref="NetId.None"/> for null).</summary>
        NetId GetId(Unit entity);

        /// <summary>Resolves a network id to its entity. Returns false (entity null) if unknown or <see cref="NetId.None"/>.</summary>
        bool TryGet(NetId id, out Unit entity);
    }
}
