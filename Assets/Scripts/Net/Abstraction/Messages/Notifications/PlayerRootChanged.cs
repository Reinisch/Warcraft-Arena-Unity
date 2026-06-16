namespace Net
{
    /// <summary>Server → controlling client: root (movement-lock) was applied or removed. (Bolt: PlayerRootChangedEvent)</summary>
    public readonly struct PlayerRootChanged : INetMessage
    {
        public bool Applied { get; }

        public PlayerRootChanged(bool applied)
        {
            Applied = applied;
        }
    }
}
