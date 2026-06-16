using System;
using Unity.Netcode;

namespace Net.Ngo
{
    /// <summary>
    /// Replicated cast state for the cast bar: the spell being cast + its total cast time (SpellId 0 = not
    /// casting). Only changes on cast start/stop — the client ticks the bar down locally between updates.
    /// </summary>
    internal struct NgoCastState : INetworkSerializable, IEquatable<NgoCastState>
    {
        public int SpellId;
        public int CastTime;

        public void NetworkSerialize<T>(BufferSerializer<T> serializer) where T : IReaderWriter
        {
            serializer.SerializeValue(ref SpellId);
            serializer.SerializeValue(ref CastTime);
        }

        public bool Equals(NgoCastState other) => SpellId == other.SpellId && CastTime == other.CastTime;
        public override bool Equals(object obj) => obj is NgoCastState other && Equals(other);
        public override int GetHashCode() => HashCode.Combine(SpellId, CastTime);
    }
}
