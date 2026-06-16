using Unity.Netcode;

namespace Net.Ngo
{
    /// <summary>
    /// <see cref="INetworkTime"/> backed by NGO's network clock. Returns the server tick while running,
    /// 0 otherwise.
    /// </summary>
    public sealed class NgoNetworkTime : INetworkTime
    {
        public int ServerFrame
        {
            get
            {
                NetworkManager nm = NetworkManager.Singleton;
                return nm != null && nm.IsListening ? nm.ServerTime.Tick : 0;
            }
        }
    }
}
