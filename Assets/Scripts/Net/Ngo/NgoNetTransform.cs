using System;
using Unity.Netcode;
using UnityEngine;

namespace Net.Ngo
{
    /// <summary>
    /// Minimal position+rotation+movement-flags payload for continuous transform replication on
    /// <see cref="EntityNetworkView"/>. Authority follows NetworkObject ownership, so the server can take
    /// movement control (Polymorph/Root) by re-owning the shadow without changing this path.
    ///
    /// <see cref="MovementFlags"/> carries the owner's full Core.MovementFlags. Receivers pick what they need:
    /// the renderer uses them for locomotion + jump animation, the server uses a clean subset
    /// (<see cref="Core.MovementFlags.MaskCastInterrupt"/>) for cast interruption.
    /// </summary>
    internal struct NgoNetTransform : INetworkSerializable, IEquatable<NgoNetTransform>
    {
        public Vector3 Position;
        public Quaternion Rotation;
        public int MovementFlags; // Core.MovementFlags as int (full)

        public NgoNetTransform(Vector3 position, Quaternion rotation, int movementFlags)
        {
            Position = position;
            Rotation = rotation;
            MovementFlags = movementFlags;
        }

        public void NetworkSerialize<T>(BufferSerializer<T> serializer) where T : IReaderWriter
        {
            serializer.SerializeValue(ref Position.x);
            serializer.SerializeValue(ref Position.y);
            serializer.SerializeValue(ref Position.z);
            serializer.SerializeValue(ref Rotation.x);
            serializer.SerializeValue(ref Rotation.y);
            serializer.SerializeValue(ref Rotation.z);
            serializer.SerializeValue(ref Rotation.w);
            serializer.SerializeValue(ref MovementFlags);
        }

        public bool Equals(NgoNetTransform other) =>
            Position == other.Position && Rotation == other.Rotation && MovementFlags == other.MovementFlags;
        public override bool Equals(object obj) => obj is NgoNetTransform other && Equals(other);
        public override int GetHashCode() => HashCode.Combine(Position, Rotation, MovementFlags);
    }
}
