namespace Net
{
    /// <summary>
    /// Lifecycle callbacks for a networked entity. Replaces Bolt's
    /// Attached / Detached / ControlGained / ControlLost overrides on EntityBehaviour.
    /// The framework adapter (shadow view) raises these as it binds to / unbinds from a Core entity.
    /// </summary>
    public interface INetworkEntityListener
    {
        void OnNetworkAttached(INetworkEntity entity);
        void OnNetworkDetached(INetworkEntity entity);
        void OnControlGained(INetworkEntity entity);
        void OnControlLost(INetworkEntity entity);
    }
}
