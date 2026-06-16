namespace Net
{
    /// <summary>Client → server: cancel the in-progress cast. Carries no payload. (Bolt: SpellCastCancelRequestEvent)</summary>
    public readonly struct SpellCastCancelRequest : INetMessage
    {
    }
}
