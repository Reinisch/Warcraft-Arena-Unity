namespace Net
{
    /// <summary>
    /// Server → client: load this scenario's map locally (scene only, no server logic). The index refers
    /// to <c>BalanceReference.Scenarios</c>, which is identical on every peer (same balance asset), so it is
    /// a stable cross-network identifier. Sent on map load and to each late-joining client.
    /// </summary>
    public readonly struct LoadScenarioCommand : INetMessage
    {
        public int ScenarioIndex { get; }

        public LoadScenarioCommand(int scenarioIndex)
        {
            ScenarioIndex = scenarioIndex;
        }
    }
}
