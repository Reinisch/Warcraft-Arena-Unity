namespace Client
{
    public interface IVisibleAura
    {
        bool HasActiveAura { get; }

        int AuraId { get; }
        int ServerRefreshFrame { get; }
        int RefreshDuration { get; }
        int MaxDuration { get; }
        int DurationLeft { get; }
    }
}
