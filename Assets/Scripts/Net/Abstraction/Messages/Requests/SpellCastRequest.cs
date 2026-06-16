using Core;

namespace Net
{
    /// <summary>Client → server: request to cast a spell. (Bolt: SpellCastRequestEvent)</summary>
    public readonly struct SpellCastRequest : INetMessage
    {
        public int SpellId { get; }
        public MovementFlags MovementFlags { get; }

        public SpellCastRequest(int spellId, MovementFlags movementFlags)
        {
            SpellId = spellId;
            MovementFlags = movementFlags;
        }
    }
}
