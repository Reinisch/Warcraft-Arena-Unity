namespace Core
{
    public class HostileReferenceManager : ReferenceManager<Unit, ThreatManager>
    {
        public Unit Owner { get; private set; }

        public HostileReferenceManager(Unit owner)
        {
            Owner = owner;
        }

        /// <summary>
        /// Send threat to all my hateres for the victim. The victim is hated than by them as well.
        /// Used for buffs and healing threat functionality.
        /// </summary>
        public void ThreatAssist(Unit victim, float baseThreat, SpellInfo threatSpell = null)
        {
        }

        public void AddTempThreat(float threat, bool apply)
        {
        }

        public void AddThreatPercent(int percent)
        {
        }

        /// <summary>
        /// The references are not needed anymore. Tell the source to remove them from the list and free the mem.
        /// </summary>
        public void DeleteReferences()
        {
        }

        /// <summary>
        /// Remove specific faction references.
        /// </summary>
        public void DeleteReferencesForFaction(uint faction)
        {
        }

        /// <summary>
        /// Delete one reference, defined by Unit.
        /// </summary>
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

        /// <summary>
        /// Set state for one reference, defined by Unit.
        /// </summary>
        public void SetOnlineOfflineState(Unit creature, bool isOnline)
        {
        }
    }
}