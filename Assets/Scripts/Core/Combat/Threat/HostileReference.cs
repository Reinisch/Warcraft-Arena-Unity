using UnityEngine;

namespace Core
{
    public class HostileReference : Reference<Unit, ThreatManager>
    {
        public bool IsAccessible { get; private set; }
        public bool IsOnline { get; private set; }

        public ulong UnitId { get; private set; }
        public Unit SourceUnit { get; private set; }

        public float TempThreatModifier { get; private set; }
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

        public void UpdateOnlineStatus()
        {
        }

        public void SetOnlineOfflineState(bool isOnline)
        {
        }

        public void SetAccessibleState(bool isAccessible)
        {
        }

        public void RemoveReference()
        {
        }

        public static bool operator ==(HostileReference a, HostileReference b)
        {
            if (ReferenceEquals(a, null))
                return ReferenceEquals(b, null);
            if (ReferenceEquals(b, null))
                return false;

            return a.UnitId == b.UnitId;
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

            return UnitId.Equals(((HostileReference) obj).UnitId);
        }

        public override int GetHashCode()
        {
            return UnitId.GetHashCode();
        }
    }
}