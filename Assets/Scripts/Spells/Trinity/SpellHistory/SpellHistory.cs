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

    public void HandleCooldowns(TrinitySpellInfo spellInfo, TrinitySpell spell)
    {
        if (spellInfo.IsPassive() || (spell != null && spell.IsIgnoringCooldowns()))
            return;

        StartCooldown(spellInfo, spell);
    }

    public void StartCooldown(TrinitySpellInfo spellInfo, TrinitySpell spell = null)
    {
        float cooldown = spellInfo.RecoveryTime;

        float currentTime = Time.time;
        float recTime = currentTime;

        // #TODO : Add modifiers

        // replace negative cooldowns by 0
        if (cooldown < 0)
            cooldown = 0;

        // no cooldown after applying spell mods
        if (cooldown == 0)
            return;

        recTime = currentTime + cooldown;

        // self spell cooldown
        if (recTime != currentTime)
            AddCooldown(spellInfo.Id, recTime);
    }

    public void AddCooldown(int spellId, float cooldownEnd)
    {
        CooldownEntry cooldownEntry = new CooldownEntry(spellId, cooldownEnd);

        if (SpellCooldowns.ContainsKey(spellId))
            SpellCooldowns[spellId] = cooldownEntry;
        else
            SpellCooldowns.Add(spellId, cooldownEntry);
    }


    public bool HasGlobalCooldown()
    {
        return Time.time < GlobalCooldown;
    }
}
