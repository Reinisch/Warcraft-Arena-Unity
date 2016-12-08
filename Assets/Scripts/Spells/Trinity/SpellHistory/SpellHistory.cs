using UnityEngine;
using System.Collections.Generic;

public class SpellHistory
{
    public Dictionary<int, CooldownEntry> SpellCooldowns { get; private set; }
    public Dictionary<int, ChargeEntry> SpellCharges { get; private set; }
    public float GlobalCooldown { get; private set; }

    public SpellHistory()
    {
        SpellCooldowns = new Dictionary<int, CooldownEntry>();
        SpellCharges = new Dictionary<int, ChargeEntry>();
    }

    public bool IsReady(TrinitySpellInfo spellInfo, bool isIgnoringCooldowns)
    {
        if (HasCooldown(spellInfo.Id, isIgnoringCooldowns))
            return false;

        return true;
    }

    public bool HasCooldown(int spellInfoId, bool ignoreCategoryCooldown)
    {
        if (SpellCooldowns.ContainsKey(spellInfoId))
            return true;

        return false;
    }

    public bool HasGlobalCooldown()
    {
        return Time.time < GlobalCooldown;
    }
}
