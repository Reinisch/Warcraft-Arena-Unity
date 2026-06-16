using System;

namespace Net
{
    /// <summary>
    /// A replicated state block for one entity. Replaces Bolt's GetState&lt;T&gt;() + AddCallback(path, cb).
    /// The framework adapter (the shadow NetworkBehaviour) implements this and is responsible for
    /// replication; game code reads <see cref="Current"/> and subscribes to per-field changes by name.
    /// On the authoritative peer, game code pushes updates via <see cref="Set"/>.
    /// </summary>
    /// <typeparam name="T">A plain snapshot type describing the replicated fields.</typeparam>
    public interface INetState<T>
    {
        T Current { get; }

        void Set(T value);

        /// <summary>Raised when a replicated field changes. The argument is the changed field name (nameof).</summary>
        event Action<string> Changed;
    }
}
