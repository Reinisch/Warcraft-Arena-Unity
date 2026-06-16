using UnityEngine;

namespace Net.Local
{
    /// <summary>Single-player clock. Uses the engine frame count as a stand-in for the server frame.</summary>
    public sealed class LocalNetworkTime : INetworkTime
    {
        public int ServerFrame => Time.frameCount;
    }
}
