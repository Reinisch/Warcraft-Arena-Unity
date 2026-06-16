namespace Net
{
    /// <summary>Outcome of a client connection attempt (replaces the old onConnectFail(reason) callback).</summary>
    public readonly struct ConnectResult
    {
        public bool Success { get; }
        public ClientConnectFailReason FailReason { get; }

        private ConnectResult(bool success, ClientConnectFailReason failReason)
        {
            Success = success;
            FailReason = failReason;
        }

        public static readonly ConnectResult Ok = new ConnectResult(true, ClientConnectFailReason.None);

        public static ConnectResult Fail(ClientConnectFailReason reason) => new ConnectResult(false, reason);
    }
}
