public struct SpellValue
{
    public int[] EffectBasePoints;
    public int MaxAffectedTargets;
    public float RadiusMod;
    public int AuraStackAmount;

    public SpellValue(TrinitySpellInfo spellInfo)
    {
        EffectBasePoints = new int[SpellHelper.MaxSpellEffects];

        for (int i = 0; i < spellInfo.SpellEffectInfos.Count; i++)
            if (spellInfo.SpellEffectInfos[i] != null)
                EffectBasePoints[spellInfo.SpellEffectInfos[i].EffectIndex] = spellInfo.SpellEffectInfos[i].BasePoints;

        MaxAffectedTargets = spellInfo.MaxAffectedTargets;
        RadiusMod = 1.0f;
        AuraStackAmount = 1;
    }
};