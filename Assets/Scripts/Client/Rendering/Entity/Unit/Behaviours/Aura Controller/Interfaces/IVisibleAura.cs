namespace Client
{
    public interface IVisibleAura
    {
        bool HasActiveAura { get; }

        int AuraId { get; }
        int Charges { get; }
        int DurationMax { get; }
        int DurationLeft { get; }
    }
}
