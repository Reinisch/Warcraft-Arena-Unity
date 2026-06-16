namespace Net
{
    /// <summary>
    /// Routing destination for a message. Unifies Bolt's two routing models:
    /// global targets (GlobalTargets: OnlyServer / Everyone / a specific connection) and
    /// entity-scoped targets (EntityTargets, routed to an entity's observers, see <see cref="EntityScope"/>).
    /// </summary>
    public readonly struct NetTarget
    {
        public enum TargetKind
        {
            /// <summary>Only the server processes it (GlobalTargets.OnlyServer).</summary>
            Server,

            /// <summary>Every client, excluding the server.</summary>
            AllClients,

            /// <summary>Every peer, including the server/host (GlobalTargets.Everyone).</summary>
            Everyone,

            /// <summary>One specific connection (was sending to event.RaisedBy / a Controller connection).</summary>
            Connection,

            /// <summary>Peers observing <see cref="Entity"/>, filtered by <see cref="Scope"/> (entity events).</summary>
            EntityObservers,
        }

        public TargetKind Kind { get; }

        /// <summary>A connection id when <see cref="Kind"/> is Connection; an entity id when EntityObservers.</summary>
        public NetId Id { get; }

        /// <summary>Audience filter, only meaningful when <see cref="Kind"/> is EntityObservers.</summary>
        public EntityScope Scope { get; }

        private NetTarget(TargetKind kind, NetId id, EntityScope scope)
        {
            Kind = kind;
            Id = id;
            Scope = scope;
        }

        public static readonly NetTarget Server = new NetTarget(TargetKind.Server, NetId.None, EntityScope.All);
        public static readonly NetTarget AllClients = new NetTarget(TargetKind.AllClients, NetId.None, EntityScope.All);
        public static readonly NetTarget Everyone = new NetTarget(TargetKind.Everyone, NetId.None, EntityScope.All);

        public static NetTarget To(NetId connection) =>
            new NetTarget(TargetKind.Connection, connection, EntityScope.All);

        public static NetTarget Observers(NetId entity, EntityScope scope = EntityScope.All) =>
            new NetTarget(TargetKind.EntityObservers, entity, scope);
    }
}
