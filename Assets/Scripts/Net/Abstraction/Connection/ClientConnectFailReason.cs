namespace Net
{
    /// <summary>
    /// Why a client-side connection attempt failed. The first values intentionally share the
    /// numeric values of <see cref="ConnectRefusedReason"/> so a server refusal maps straight across.
    /// </summary>
    public enum ClientConnectFailReason
    {
        None = (int)ConnectRefusedReason.None,
        InvalidToken = (int)ConnectRefusedReason.InvalidToken,
        InvalidVersion = (int)ConnectRefusedReason.InvalidVersion,
        UnsupportedDevice = (int)ConnectRefusedReason.UnsupportedDevice,
        ServerRefusedConnection,
        FailedToConnectToServer,
        FailedToConnectToSession,
        ConnectionTimeout,
    }
}
