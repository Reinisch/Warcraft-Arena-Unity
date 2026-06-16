using Core;
using UnityEngine;

namespace Net
{
    /// <summary>
    /// Server → client: a unit took spell damage. Mirrors the Core <c>GameEvents.SpellDamageDone</c>
    /// (caster, target, amount, hit type, optional hit position) so a remote client can re-raise it for
    /// damage numbers + hit reactions.
    /// </summary>
    public readonly struct SpellDamageDone : INetMessage
    {
        public NetId CasterId { get; }
        public NetId TargetId { get; }
        public int Damage { get; }
        public HitType HitType { get; }
        public Vector3 HitPosition { get; }
        public bool HasHitPosition { get; }

        public SpellDamageDone(NetId casterId, NetId targetId, int damage, HitType hitType, Vector3 hitPosition, bool hasHitPosition)
        {
            CasterId = casterId;
            TargetId = targetId;
            Damage = damage;
            HitType = hitType;
            HitPosition = hitPosition;
            HasHitPosition = hasHitPosition;
        }
    }
}
