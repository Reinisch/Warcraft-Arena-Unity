namespace Net
{
    /// <summary>
    /// A payload that can serialize itself. Replaces Bolt's IProtocolToken (Read/Write over UdpPacket).
    /// Used for connect / spawn / control-handover payloads (the old *Token types).
    /// </summary>
    public interface INetSerializable
    {
        void Write(INetWriter writer);
        void Read(INetReader reader);
    }
}
