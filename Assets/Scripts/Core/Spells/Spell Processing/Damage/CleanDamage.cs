namespace Core
{
    public class CleanDamage
    {
        public int AbsorbedDamage { get; set; }
        public int MitigatedDamage { get; set; }

        public CleanDamage(int absorb, int mitigated)
        {
            AbsorbedDamage = absorb;
            MitigatedDamage = mitigated;
        }
    }
}