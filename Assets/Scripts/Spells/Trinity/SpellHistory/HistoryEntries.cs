using UnityEngine;
using System.Collections;

public struct CooldownEntry
{
    public int SpellId { get; set; }
    public float CooldownEnd { get; set; }
    public bool OnHold { get; set; }
};

public struct ChargeEntry
{
    public float RechargeStart { get; set; }
    public float RechargeEnd { get; set; }
};