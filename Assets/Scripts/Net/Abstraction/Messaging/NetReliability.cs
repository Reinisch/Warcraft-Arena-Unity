namespace Net
{
    /// <summary>Delivery guarantee for a message. Replaces Bolt's ReliabilityModes.</summary>
    public enum NetReliability
    {
        ReliableOrdered,
        Unreliable,
    }
}
