namespace Net
{
    /// <summary>
    /// Receive-side context for a message. Carries the sender and whether it originated locally,
    /// replacing Bolt's event.RaisedBy / event.FromSelf.
    /// </summary>
    public readonly struct NetContext
    {
        /// <summary>The connection that sent the message (<see cref="NetId.None"/> for locally-raised messages).</summary>
        public NetId Sender { get; }

        /// <summary>True when this peer is also the sender (host / loopback).</summary>
        public bool FromSelf { get; }

        public NetContext(NetId sender, bool fromSelf)
        {
            Sender = sender;
            FromSelf = fromSelf;
        }
    }
}
