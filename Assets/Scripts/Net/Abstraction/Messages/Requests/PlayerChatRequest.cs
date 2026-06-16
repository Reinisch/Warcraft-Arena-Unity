namespace Net
{
    /// <summary>Client → server: send a chat message. (Bolt: PlayerChatRequestEvent)</summary>
    public readonly struct PlayerChatRequest : INetMessage
    {
        public string Message { get; }

        public PlayerChatRequest(string message)
        {
            Message = message;
        }
    }
}
