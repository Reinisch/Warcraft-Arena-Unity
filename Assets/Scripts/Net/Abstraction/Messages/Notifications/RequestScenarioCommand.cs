namespace Net
{
    /// <summary>
    /// Client → server: "I'm fully connected — tell me which scenario/map to load." Sent by the client once
    /// its own connection is established (so its message handler is guaranteed registered), which avoids the
    /// race where the server's unsolicited <see cref="LoadScenarioCommand"/> arrives before the client is ready
    /// to receive it (more likely over higher-latency Relay) and is silently dropped. The server replies with
    /// <see cref="LoadScenarioCommand"/>.
    /// </summary>
    public readonly struct RequestScenarioCommand : INetMessage
    {
    }
}
