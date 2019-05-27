using Common;

namespace Core
{
    public class HostileReferenceManager : ReferenceManager<Unit, ThreatManager>
    {
        public Unit Owner { get; private set; }

        public HostileReferenceManager(Unit owner)
        {
            Owner = owner;
        }

        public void ThreatAssist(Unit victim, float baseThreat, SpellInfo threatSpell = null)
        {
        }

        public void AddTempThreat(float threat, bool apply)
        {
        }

        public void AddThreatPercent(int percent)
        {
        }

        public void DeleteReferences()
        {
        }

        public void DeleteReferencesForFaction(uint faction)
        {
        }

        public void DeleteReference(Unit creature)
        {
        }

        public void UpdateVisibility()
        {
        }

        public void UpdateThreatTables()
        {
        }

        public void SetOnlineOfflineState(bool isOnline)
        {
        }

        public void SetOnlineOfflineState(Unit creature, bool isOnline)
        {
        }
    }
}