namespace Net
{
    /// <summary>
    /// Server's reason for refusing a connection, returned to the rejected client.
    /// (Bolt: ClientRefuseToken, an IProtocolToken.)
    /// </summary>
    public sealed class ClientRefuseToken : INetSerializable
    {
        public ConnectRefusedReason Reason { get; private set; }

        public ClientRefuseToken()
        {
            Reason = ConnectRefusedReason.None;
        }

        public ClientRefuseToken(ConnectRefusedReason reason)
        {
            Reason = reason;
        }

        public void Write(INetWriter writer)
        {
            writer.WriteInt((int)Reason);
        }

        public void Read(INetReader reader)
        {
            Reason = (ConnectRefusedReason)reader.ReadInt();
        }
    }
}
