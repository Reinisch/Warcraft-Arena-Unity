namespace Net
{
    /// <summary>Server → everyone: a chat message from a unit, broadcast to all. (Bolt: UnitChatMessageEvent)</summary>
    public readonly struct UnitChatMessage : INetMessage
    {
        public NetId SenderId { get; }
        public string SenderName { get; }
        public string Message { get; }

        public UnitChatMessage(NetId senderId, string senderName, string message)
        {
            SenderId = senderId;
            SenderName = senderName;
            Message = message;
        }
    }
}
