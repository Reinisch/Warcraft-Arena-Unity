namespace Net
{
    /// <summary>
    /// Server → client: the current scenario/map is over (the server unloaded it). The client unloads its
    /// own map and units; the replicated shadows despawn on their own via sync.
    /// </summary>
    public readonly struct EndScenarioCommand : INetMessage
    {
    }
}
