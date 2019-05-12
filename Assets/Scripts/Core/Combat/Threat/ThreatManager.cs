using System;
using System.Collections.Generic;

namespace Core
{
    public class ThreatManager : ReferenceManager<Unit, ThreatManager>
    {
        public Unit Owner { get; private set; }
        public HostileReference CurrentVictim { get; private set; }

        public List<HostileReference> ThreatList => ThreatContainer.ThreatList;
        public List<HostileReference> OfflineThreatList => ThreatOfflineContainer.ThreatList;
        public ThreatContainer ThreatContainer { get; private set; }
        public ThreatContainer ThreatOfflineContainer { get; private set; }

        public ThreatManager(Unit owner)
        {
        }

        public void ClearReferences()
        {
        }

        public void AddThreat(Unit victim, float threat, SpellSchoolMask schoolMask = SpellSchoolMask.Normal, SpellInfo threatSpell = null)
        {
        }

        public void DoAddThreat(Unit victim, float threat)
        {
        }

        public void ModifyThreatPercent(Unit victim, int percent)
        {
        }

        public float GetThreat(Unit victim, bool alsoSearchOfflineList = false)
        {
            return 0.0f;
        }

        public bool IsNeedUpdateToClient(uint time)
        {
            return false;
        }

        public Unit GetHostilTarget()
        {
            return null;
        }

        public void TauntApply(Unit taunter)
        {
        }

        public void TauntFadeOut(Unit taunter)
        {
        }

        public void SetCurrentVictim(HostileReference hostileRef)
        {
        }

        public void SetDirty(bool isDirty)
        {
            ThreatContainer.IsDirty = isDirty;
        }

        public void ResetAllAggro()
        {
        }

        public void ResetAggro(Predicate<Unit> predicate)
        {
            if (ThreatContainer.ThreatList.Count == 0)
                return;

            foreach (var threat in ThreatContainer.ThreatList)
            {
                if (predicate(threat.Target))
                {
                    threat.SetThreat(0);
                    SetDirty(true);
                }
            }
        }
    }
}