using UnityEngine;
using System.Collections;

public struct CooldownEntry
{
    public int SpellId { get; set; }
    public float CooldownEnd { get; set; }
    public bool OnHold { get; set; }

    public CooldownEntry(int spellId, float cooldownEnd, bool onHold = false)
    {
        SpellId = spellId;
        CooldownEnd = cooldownEnd;
        OnHold = onHold;
    }
};

public struct ChargeEntry
{
    public float RechargeStart { get; set; }
    public float RechargeEnd { get; set; }
};