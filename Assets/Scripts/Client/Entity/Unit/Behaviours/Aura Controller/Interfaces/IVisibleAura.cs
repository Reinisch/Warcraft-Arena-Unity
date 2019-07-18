namespace Client
{
    public interface IVisibleAura
    {
        bool HasActiveAura { get; }

        int AuraId { get; }
        int MaxDuration { get; }
        int DurationLeft { get; }
    }
}
