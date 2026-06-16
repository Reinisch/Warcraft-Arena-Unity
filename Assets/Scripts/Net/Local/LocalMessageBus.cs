using System;
using System.Collections.Generic;

namespace Net.Local
{
    /// <summary>
    /// In-process message bus for single-player / host. Every <see cref="Send{T}"/> is delivered
    /// synchronously to all local subscribers (server logic and client logic run in the same process),
    /// so behaviour matches the old direct calls one-for-one. <see cref="NetTarget"/> is ignored here —
    /// a real transport adapter routes by it.
    /// </summary>
    public sealed class LocalMessageBus : INetworkMessageBus
    {
        private static readonly NetContext LocalContext = new NetContext(NetId.None, fromSelf: true);

        private readonly Dictionary<Type, List<Delegate>> handlers = new Dictionary<Type, List<Delegate>>();

        public void Send<T>(T message, NetTarget target, NetReliability reliability = NetReliability.ReliableOrdered)
            where T : INetMessage
        {
            if (!handlers.TryGetValue(typeof(T), out List<Delegate> list) || list.Count == 0)
                return;

            // Snapshot so a handler may subscribe/unsubscribe during dispatch.
            Delegate[] snapshot = list.ToArray();
            for (int i = 0; i < snapshot.Length; i++)
                ((Action<T, NetContext>)snapshot[i]).Invoke(message, LocalContext);
        }

        public IDisposable Subscribe<T>(Action<T, NetContext> handler) where T : INetMessage
        {
            if (handler == null)
                throw new ArgumentNullException(nameof(handler));

            if (!handlers.TryGetValue(typeof(T), out List<Delegate> list))
            {
                list = new List<Delegate>();
                handlers[typeof(T)] = list;
            }

            list.Add(handler);
            return new Subscription(list, handler);
        }

        private sealed class Subscription : IDisposable
        {
            private List<Delegate> list;
            private Delegate handler;

            public Subscription(List<Delegate> list, Delegate handler)
            {
                this.list = list;
                this.handler = handler;
            }

            public void Dispose()
            {
                if (list == null)
                    return;

                list.Remove(handler);
                list = null;
                handler = null;
            }
        }
    }
}
