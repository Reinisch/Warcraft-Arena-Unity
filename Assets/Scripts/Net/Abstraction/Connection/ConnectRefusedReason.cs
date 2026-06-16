namespace Net
{
    /// <summary>Reason a server refused an incoming connection. Ported from the old project.</summary>
    public enum ConnectRefusedReason
    {
        None,
        InvalidToken,
        InvalidVersion,
        UnsupportedDevice,
    }
}
