using System;

namespace Net
{
    /// <summary>
    /// Transport-agnostic identifier for a networked entity or connection.
    /// Wraps the framework's native id (Bolt used NetworkId.PackedValue).
    /// </summary>
    public readonly struct NetId : IEquatable<NetId>
    {
        public static readonly NetId None = new NetId(0UL);

        public ulong Value { get; }

        public bool IsValid => Value != 0UL;

        public NetId(ulong value) => Value = value;

        public bool Equals(NetId other) => Value == other.Value;
        public override bool Equals(object obj) => obj is NetId other && Equals(other);
        public override int GetHashCode() => Value.GetHashCode();
        public override string ToString() => $"NetId({Value})";

        public static bool operator ==(NetId a, NetId b) => a.Value == b.Value;
        public static bool operator !=(NetId a, NetId b) => a.Value != b.Value;
    }
}
