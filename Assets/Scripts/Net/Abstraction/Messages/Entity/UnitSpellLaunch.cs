using System.Collections.Generic;
using UnityEngine;

namespace Net
{
    /// <summary>One projectile/effect target of a launched spell: the target entity + its travel time.</summary>
    public readonly struct SpellLaunchTarget
    {
        public NetId TargetId { get; }
        public float Time { get; }

        public SpellLaunchTarget(NetId targetId, float time)
        {
            TargetId = targetId;
            Time = time;
        }
    }

    /// <summary>
    /// Server → observers: a unit launched a spell, for cast/projectile visuals. Carries NetIds (not Core
    /// entity ids) so it survives the wire — the server translates the Core SpellProcessingToken to this on
    /// send, and a remote client rebuilds a local token on receive. Mirrors Core <c>GameEvents.SpellLaunched</c>.
    /// </summary>
    public readonly struct UnitSpellLaunch : INetMessage
    {
        public NetId CasterId { get; }
        public int SpellId { get; }
        public Vector3 Source { get; }
        public Vector3 Destination { get; }
        public IReadOnlyList<SpellLaunchTarget> Targets { get; }

        public UnitSpellLaunch(NetId casterId, int spellId, Vector3 source, Vector3 destination,
            IReadOnlyList<SpellLaunchTarget> targets)
        {
            CasterId = casterId;
            SpellId = spellId;
            Source = source;
            Destination = destination;
            Targets = targets;
        }
    }
}
