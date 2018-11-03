using System;
using UnityEngine;

namespace Core
{
    public class HostileReference : Reference<Unit, ThreatManager>
    {
        // The Unit might be in water and the creature can not enter the water, but has range attack
        // in this case online = true, but accessible = false
        public bool IsAccessible { get; private set; }
        public bool IsOnline { get; private set; }

        public Guid UnitGuid { get; private set; }
        public Unit SourceUnit { get; private set; }

        public float TempThreatModifier { get; private set; } // used for taunt
        public float Threat { get; private set; }

        public HostileReference NextReference => Next as HostileReference;


        public HostileReference(Unit refUnit, ThreatManager threatManager, float threat)
        {
        }

        public void AddThreat(float modThreat)
        {
        }

        public void SetThreat(float threat)
        {
            AddThreat(threat - Threat);
        }

        public void AddThreatPercent(int percent)
        {
        }


        // used for temporary setting a threat and reducting it later again.
        // the threat modification is stored
        public void SetTempThreat(float threat)
        {
            AddTempThreat(threat - Threat);
        }

        public void AddTempThreat(float threat)
        {
            TempThreatModifier = threat;
            if (!Mathf.Approximately(TempThreatModifier, 0.0f))
                AddThreat(TempThreatModifier);
        }

        public void ResetTempThreat()
        {
            if (!Mathf.Approximately(TempThreatModifier, 0.0f))
            {
                AddThreat(-TempThreatModifier);
                TempThreatModifier = 0.0f;
            }
        }


        // check, if source can reach target and set the status
        public void UpdateOnlineStatus()
        {
        }

        public void SetOnlineOfflineState(bool isOnline)
        {
        }

        public void SetAccessibleState(bool isAccessible)
        {
        }

        // reference is not needed anymore. realy delete it !
        public void RemoveReference()
        {
        }


        public static bool operator ==(HostileReference a, HostileReference b)
        {
            if (ReferenceEquals(a, null))
                return ReferenceEquals(b, null);
            if (ReferenceEquals(b, null))
                return false;

            return a.UnitGuid == b.UnitGuid;
        }

        public static bool operator !=(HostileReference a, HostileReference b)
        {
            return !(a == b);
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj))
                return false;
            if (ReferenceEquals(this, obj))
                return true;
            if (obj.GetType() != GetType())
                return false;

            return UnitGuid.Equals(((HostileReference) obj).UnitGuid);
        }

        public override int GetHashCode()
        {
            return UnitGuid.GetHashCode();
        }


        // Tell our refTo (target) object that we have a link
        protected override void TargetObjectBuildLink()
        {
        }

        // Tell our refTo (taget) object, that the link is cut
        protected override void TargetObjectDestroyLink()
        {
        }

        // Tell our refFrom (source) object, that the link is cut (Target destroyed)
        protected override void SourceObjectDestroyLink()
        {
        }

        // Inform the source, that the status of that reference was changed
        private void FireStatusChanged(ThreatRefStatusChangeEvent threatRefStatusChangeEvent)
        {
        }
    }
}