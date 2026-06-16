using Core;
using UnityEngine;

namespace Net
{
    /// <summary>Client → server: request to cast a spell at a ground destination. (Bolt: SpellCastRequestDestinationEvent)</summary>
    public readonly struct SpellCastDestinationRequest : INetMessage
    {
        public int SpellId { get; }
        public MovementFlags MovementFlags { get; }
        public Vector3 Destination { get; }

        public SpellCastDestinationRequest(int spellId, MovementFlags movementFlags, Vector3 destination)
        {
            SpellId = spellId;
            MovementFlags = movementFlags;
            Destination = destination;
        }
    }
}
