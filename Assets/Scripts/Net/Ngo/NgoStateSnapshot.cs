using Unity.Netcode;

namespace Net.Ngo
{
    /// <summary>
    /// Per-unit continuous state streamed UNRELIABLY at a fixed rate (server → observing clients): transform +
    /// vitals + target + cast bundled into one small packet. The FULL state is sent every time, so a dropped
    /// packet is self-correcting — the next one overwrites it (latest-wins, like the old Bolt state channel).
    ///
    /// Deliberately NOT a <see cref="NetworkVariable{T}"/>: NetworkVariables are reliable AND their current
    /// values are bundled into the connection-approval burst at spawn, which overflows for many-unit scenarios
    /// and stalls the join. Streaming this off-band keeps the spawn payload tiny and the steady state cheap.
    /// </summary>
    internal struct NgoStateSnapshot : INetworkSerializable
    {
        public NgoNetTransform Transform;
        public NgoUnitVitals Vitals;
        public ulong TargetId;
        public NgoCastState Cast;

        public void NetworkSerialize<T>(BufferSerializer<T> serializer) where T : IReaderWriter
        {
            Transform.NetworkSerialize(serializer);
            Vitals.NetworkSerialize(serializer);
            serializer.SerializeValue(ref TargetId);
            Cast.NetworkSerialize(serializer);
        }
    }
}
