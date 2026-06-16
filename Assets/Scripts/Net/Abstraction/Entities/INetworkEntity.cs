namespace Net
{
    /// <summary>
    /// The networking identity of an entity, exposed to game code without leaking the framework.
    /// Mirrors the surface Bolt's EntityBehaviour exposed (NetworkId / IsOwner / HasControl).
    /// In the shadow design this is implemented by the framework-owned view, not by the Core entity.
    /// </summary>
    public interface INetworkEntity
    {
        NetId Id { get; }

        /// <summary>True on the peer that has authority over this entity (server, or host).</summary>
        bool IsOwner { get; }

        /// <summary>True on the peer currently controlling this entity (input authority).</summary>
        bool IsController { get; }
    }
}
