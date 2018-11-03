using System;
using UnityEngine;

namespace Core
{
    public class RedirectThreatInfo
    {
        public Guid TargetGUID { get; private set; }
        public int ThreatPercent { get; private set; }


        public RedirectThreatInfo()
        {
            ThreatPercent = 0;
        }

        public void Set(Guid guid, int pct)
        {
            TargetGUID = guid;
            ThreatPercent = pct;
        }

        public void ModifyThreatPct(int amount)
        {
            amount += ThreatPercent;
            ThreatPercent = Mathf.Max(0, amount);
        }
    }
}