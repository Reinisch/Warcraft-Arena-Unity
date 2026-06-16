using System;

namespace Net
{
    /// <summary>
    /// Typed send/subscribe bus for one-shot messages. Replaces the per-event
    /// SomeEvent.Create(target).Send() / OnEvent(SomeEvent) pattern from Bolt with a single seam.
    /// </summary>
    public interface INetworkMessageBus
    {
        void Send<T>(T message, NetTarget target, NetReliability reliability = NetReliability.ReliableOrdered)
            where T : INetMessage;

        /// <summary>Subscribe to messages of type <typeparamref name="T"/>. Dispose to unsubscribe.</summary>
        IDisposable Subscribe<T>(Action<T, NetContext> handler) where T : INetMessage;
    }
}
