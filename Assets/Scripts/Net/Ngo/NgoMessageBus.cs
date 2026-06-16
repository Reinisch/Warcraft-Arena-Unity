using System;
using System.Collections.Generic;
using Unity.Netcode;
using Zenject;

namespace Net.Ngo
{
    /// <summary>
    /// <see cref="INetworkMessageBus"/> over NGO. Each message is sent as a single named message
    /// "<see cref="MessageName"/>" carrying [ushort typeId][payload], serialized by the shared
    /// <see cref="NetMessageCodec"/>. <see cref="NetTarget"/> is resolved to NGO client ids; recipients
    /// that are the local peer are dispatched in-process (so host doesn't rely on loopback).
    ///
    /// Limitations (basic pass): EntityObservers falls back to all clients (proper interest management
    /// comes with the spawn/replication step), and messages the codec hasn't registered yet
    /// (UnitSpellLaunch) are skipped.
    /// </summary>
    public sealed class NgoMessageBus : INetworkMessageBus, IInitializable, IDisposable
    {
        private const string MessageName = "wcb.net";
        private const int InitialWriterSize = 256;
        private const int MaxWriterSize = 64 * 1024;

        private readonly NetMessageCodec codec = new NetMessageCodec();
        private readonly Dictionary<Type, List<Action<INetMessage, NetContext>>> handlers =
            new Dictionary<Type, List<Action<INetMessage, NetContext>>>();
        private readonly List<ulong> remipients = new List<ulong>();

        private bool registered;
        private bool hooked;

        public NgoMessageBus()
        {
            EnsureHooked();
        }

        // Runs in Zenject's Start phase — AFTER every MonoBehaviour Awake, so NetworkManager.Singleton is
        // guaranteed to exist. This is the reliable hook point: without an early bus.Send to trigger
        // EnsureHooked (e.g. when booting straight into the session-less lobby), the ctor call above may have
        // run before the NetworkManager existed, leaving the bus unhooked so it never registers its named
        // message handler on Start — and a freshly-joined client would silently receive nothing.
        void IInitializable.Initialize() => EnsureHooked();

        // NetworkManager.Singleton may not exist yet when this is constructed (script order vs the
        // NetworkManager GameObject), so hook lazily — also from Send/Subscribe — once it's available.
        private void EnsureHooked()
        {
            if (hooked)
                return;

            NetworkManager nm = Manager;
            if (nm == null)
                return;

            nm.OnServerStarted += RegisterHandler;
            nm.OnClientStarted += RegisterHandler;
            nm.OnServerStopped += OnStopped;
            nm.OnClientStopped += OnStopped;
            hooked = true;

            if (nm.IsListening)
                RegisterHandler();
        }

        private static NetworkManager Manager => NetworkManager.Singleton;

        public void Send<T>(T message, NetTarget target, NetReliability reliability = NetReliability.ReliableOrdered)
            where T : INetMessage
        {
            EnsureHooked();

            NetworkManager nm = Manager;
            if (nm == null || !nm.IsListening)
                return;

            // Not yet serializable (e.g. UnitSpellLaunch) — skip rather than crash gameplay.
            if (!codec.TryGetId(typeof(T), out ushort typeId))
                return;

            bool dispatchLocal = ResolveAudience(nm, target);

            if (remipients.Count > 0)
            {
                NetworkDelivery delivery = reliability == NetReliability.Unreliable
                    ? NetworkDelivery.Unreliable
                    : NetworkDelivery.ReliableFragmentedSequenced;

                using var writer = new NgoWriter(InitialWriterSize, MaxWriterSize);
                writer.WriteUInt(typeId);
                codec.Write(writer, message);

                for (int i = 0; i < remipients.Count; i++)
                    nm.CustomMessagingManager.SendNamedMessage(MessageName, remipients[i], writer.Buffer, delivery);
            }

            if (dispatchLocal)
                Dispatch(message, typeof(T), new NetContext(new NetId(nm.LocalClientId), fromSelf: true));
        }

        public IDisposable Subscribe<T>(Action<T, NetContext> handler) where T : INetMessage
        {
            EnsureHooked();

            if (handler == null)
                throw new ArgumentNullException(nameof(handler));

            if (!handlers.TryGetValue(typeof(T), out List<Action<INetMessage, NetContext>> list))
            {
                list = new List<Action<INetMessage, NetContext>>();
                handlers[typeof(T)] = list;
            }

            void Wrapper(INetMessage m, NetContext c) => handler((T)m, c);
            list.Add(Wrapper);
            return new Subscription(list, Wrapper);
        }

        /// <summary>Fills <see cref="remipients"/> with remote client ids; returns whether to dispatch locally.</summary>
        private bool ResolveAudience(NetworkManager nm, NetTarget target)
        {
            remipients.Clear();
            ulong localId = nm.LocalClientId;
            bool local = false;

            switch (target.Kind)
            {
                case NetTarget.TargetKind.Server:
                    if (nm.IsServer) local = true;
                    else remipients.Add(NetworkManager.ServerClientId);
                    break;

                case NetTarget.TargetKind.Connection:
                    ulong c = target.Id.Value;
                    if (c == localId) local = true;
                    else remipients.Add(c);
                    break;

                case NetTarget.TargetKind.AllClients:
                    if (nm.IsServer) local = AddClients(nm, localId);
                    break;

                case NetTarget.TargetKind.Everyone:
                    if (nm.IsServer) { AddClients(nm, localId); local = true; }
                    else remipients.Add(NetworkManager.ServerClientId);
                    break;

                case NetTarget.TargetKind.EntityObservers:
                    // TODO: filter by the entity's observers + EntityScope. Falls back to all clients.
                    if (nm.IsServer) local = AddClients(nm, localId);
                    break;
            }

            return local;
        }

        /// <summary>Adds all connected clients except the local peer to recipients; returns true if the local peer is among them (host).</summary>
        private bool AddClients(NetworkManager nm, ulong localId)
        {
            bool includesLocal = false;
            IReadOnlyList<ulong> ids = nm.ConnectedClientsIds;
            for (int i = 0; i < ids.Count; i++)
            {
                if (ids[i] == localId) includesLocal = true;
                else remipients.Add(ids[i]);
            }

            return includesLocal;
        }

        private void Dispatch(INetMessage message, Type type, NetContext context)
        {
            if (!handlers.TryGetValue(type, out List<Action<INetMessage, NetContext>> list) || list.Count == 0)
                return;

            Action<INetMessage, NetContext>[] snapshot = list.ToArray();
            for (int i = 0; i < snapshot.Length; i++)
                snapshot[i](message, context);
        }

        private void OnNamedMessage(ulong senderClientId, FastBufferReader payload)
        {
            var reader = new NgoReader(payload);
            var typeId = (ushort)reader.ReadUInt();
            INetMessage message = codec.Read(typeId, reader);
            Dispatch(message, message.GetType(), new NetContext(new NetId(senderClientId), fromSelf: false));
        }

        private void RegisterHandler()
        {
            if (registered)
                return;

            NetworkManager nm = Manager;
            if (nm?.CustomMessagingManager == null)
                return;

            nm.CustomMessagingManager.RegisterNamedMessageHandler(MessageName, OnNamedMessage);
            registered = true;
        }

        private void OnStopped(bool _)
        {
            NetworkManager nm = Manager;
            if (registered && nm?.CustomMessagingManager != null)
                nm.CustomMessagingManager.UnregisterNamedMessageHandler(MessageName);

            registered = false;
        }

        public void Dispose()
        {
            NetworkManager nm = Manager;
            if (nm != null && hooked)
            {
                nm.OnServerStarted -= RegisterHandler;
                nm.OnClientStarted -= RegisterHandler;
                nm.OnServerStopped -= OnStopped;
                nm.OnClientStopped -= OnStopped;
                hooked = false;
            }

            OnStopped(false);
        }

        private sealed class Subscription : IDisposable
        {
            private List<Action<INetMessage, NetContext>> list;
            private Action<INetMessage, NetContext> handler;

            public Subscription(List<Action<INetMessage, NetContext>> list, Action<INetMessage, NetContext> handler)
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
