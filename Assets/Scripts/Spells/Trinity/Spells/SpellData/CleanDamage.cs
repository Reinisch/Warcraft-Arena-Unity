public struct CleanDamage
{
    public int AbsorbedDamage;
    public int MitigatedDamage;

    public CleanDamage(int absorb, int mitigated)
    {
        AbsorbedDamage = absorb;
        MitigatedDamage = mitigated;
    }
};