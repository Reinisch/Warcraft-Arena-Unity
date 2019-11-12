namespace Client
{
    public interface IVisibleAuraHandler
    {
        void AuraApplied(IVisibleAura visibleAura);
        void AuraUnapplied(IVisibleAura visibleAura);
        void AuraRefreshed(IVisibleAura visibleAura);
    }
}
