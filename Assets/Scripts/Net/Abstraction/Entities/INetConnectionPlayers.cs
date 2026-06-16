using Core;

namespace Net
{
    /// <summary>
    /// Server-side lookup from a connection to the authoritative <see cref="Player"/> it controls. Lets the
    /// command router act on the SENDER's player (resolved from <see cref="NetContext.Sender"/>) instead of
    /// the host's local one — without which every client's input would drive the host player.
    /// </summary>
    public interface INetConnectionPlayers
    {
        bool TryGetPlayer(NetId connection, out Player player);

        /// <summary>Reverse lookup: the connection that controls this player (for server→owning-client
        /// messages like speed/teleport). False for AI/host players not owned by a remote connection.</summary>
        bool TryGetConnection(Unit player, out NetId connection);
    }
}
