namespace Net
{
    /// <summary>Client → server: change the player's selected target. (Bolt: TargetSelectionRequestEvent)</summary>
    public readonly struct TargetSelectionRequest : INetMessage
    {
        public NetId TargetId { get; }

        public TargetSelectionRequest(NetId targetId)
        {
            TargetId = targetId;
        }
    }
}
