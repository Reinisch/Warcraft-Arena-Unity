using System;
using Unity.Netcode;

namespace Net.Ngo
{
    /// <summary>
    /// One active visible aura for replication (server→client), carried in a NetworkList. Unmanaged +
    /// IEquatable as NetworkList requires; INetworkSerializeByMemcpy so NGO generates serialization for it
    /// (all-int struct, no pointers). SlotIndex preserves which visual slot the aura occupies.
    /// </summary>
    internal struct NgoAuraSlot : IEquatable<NgoAuraSlot>, INetworkSerializeByMemcpy
    {
        public int SlotIndex;
        public int AuraId;
        public int DurationMax;
        public int DurationLeft;
        public int Charges;

        public bool Equals(NgoAuraSlot other) =>
            SlotIndex == other.SlotIndex && AuraId == other.AuraId && DurationMax == other.DurationMax &&
            DurationLeft == other.DurationLeft && Charges == other.Charges;

        public override bool Equals(object obj) => obj is NgoAuraSlot other && Equals(other);
        public override int GetHashCode() => HashCode.Combine(SlotIndex, AuraId, DurationMax, DurationLeft, Charges);
    }
}
