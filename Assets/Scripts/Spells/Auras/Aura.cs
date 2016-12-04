using UnityEngine;
using System.Collections.Generic;

public class Aura
{
    protected SpellInfo m_spellInfo;
    protected int castId;
    protected int casterId;

    protected int spellXSpellVisualId;
    protected double applyTime;
    protected GameObject owner;

    protected int maxDuration;                                // Max aura duration
    protected int duration;                                   // Current time
    protected int m_timeCla;                                  // Timer for power per sec calcultion

    protected int casterLevel;                                // Aura level (store caster level for correct show level dep amount)
    protected int m_procCharges;                              // Aura charges (0 for infinite)
    protected int m_stackAmount;                              // Aura stack amount

    protected Dictionary<int, AuraApplication> applications = new Dictionary<int, AuraApplication>();

    protected bool isRemoved;
    protected bool isSingleTarget;  // true if it's a single target spell and registered at caster - can change at spell steal for example
    protected bool isUsingCharges;


    private List<AuraApplication> removedApplications = new List<AuraApplication>();
    private List<AuraEffect> auraEffects = new List<AuraEffect>();

    //private List<SpellProjectileEffect> spellEffectInfos;
}
