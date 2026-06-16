namespace Net
{
    /// <summary>
    /// Marker for a one-shot networked message (RPC/event). Replaces Bolt's generated Event types.
    /// Implementations are plain data types in this assembly; the adapter maps them to the
    /// framework's transport. They serialize themselves via <see cref="INetSerializable"/> when needed.
    /// </summary>
    public interface INetMessage
    {
    }
}
