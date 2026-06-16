using Core;

namespace Net
{
    /// <summary>
    /// Server → client: a spell missed/was avoided. Mirrors the Core <c>GameEvents.SpellMissDone</c>
    /// (caster, target, miss type) so a remote client can re-raise it.
    /// </summary>
    public readonly struct SpellMissDone : INetMessage
    {
        public NetId CasterId { get; }
        public NetId TargetId { get; }
        public SpellMissType MissType { get; }

        public SpellMissDone(NetId casterId, NetId targetId, SpellMissType missType)
        {
            CasterId = casterId;
            TargetId = targetId;
            MissType = missType;
        }
    }
}
