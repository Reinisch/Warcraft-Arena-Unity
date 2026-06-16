using Core;

namespace Net
{
    /// <summary>
    /// Server → casting client: outcome of a cast request (success/failure). Launch visuals are driven
    /// separately by <see cref="UnitSpellLaunch"/>. (Bolt: SpellCastRequestAnswerEvent)
    /// </summary>
    public readonly struct SpellCastResultNotification : INetMessage
    {
        public int SpellId { get; }
        public SpellCastResult Result { get; }

        public SpellCastResultNotification(int spellId, SpellCastResult result)
        {
            SpellId = spellId;
            Result = result;
        }
    }
}
