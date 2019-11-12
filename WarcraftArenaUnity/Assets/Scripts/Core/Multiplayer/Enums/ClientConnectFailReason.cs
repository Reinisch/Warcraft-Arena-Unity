namespace Core
{
    public enum ClientConnectFailReason
    {
        None = ConnectRefusedReason.None,
        InvalidToken = ConnectRefusedReason.InvalidToken,
        InvalidVersion = ConnectRefusedReason.InvalidVersion,
        UnsupportedDevice = ConnectRefusedReason.UnsupportedDevice,
        ServerRefusedConnection,
        FailedToConnectToMaster,
        FailedToConnectToSession,
        ConnectionTimeout,
    }
}
