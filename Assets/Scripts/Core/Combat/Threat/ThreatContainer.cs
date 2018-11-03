using System.Collections.Generic;

namespace Core
{
    public class ThreatContainer
    {
        public bool IsDirty { get; set; }
        public bool IsEmpty => ThreatList.Count == 0;

        public HostileReference MostHated => ThreatList.Count == 0 ? null : ThreatList[0];
        public List<HostileReference> ThreatList { get; }


        public ThreatContainer()
        {
            ThreatList = new List<HostileReference>();
            IsDirty = false;
        }

        public void ModifyThreatPercent(Unit victim, int percent)
        {
        }


        public HostileReference AddThreat(Unit victim, float threat)
        {
            return null;
        }

        public HostileReference SelectNextVictim(Creature attacker, HostileReference currentVictim)
        {
            return null;
        }

        public HostileReference GetReferenceByTarget(Unit victim)
        {
            return null;
        }


        private void ClearReferences()
        {
        }

        private void Remove(HostileReference hostileRef)
        {
            ThreatList.Remove(hostileRef);
        }

        private void AddReference(HostileReference hostileRef)
        {
            ThreatList.Add(hostileRef);
        }


        /// <summary>
        /// Sort the list if necessary.
        /// </summary>
        private void Update()
        {
        }
    }
}