using UnityEngine;
using System.Collections;

public static class SpellHelper
{
    public static int MaxSpellEffects { get { return 32; } }

    public static SpellCastTimes InstantCastTime { get { return WarcraftDatabase.SpellCastTimes[1]; } }

    public static SpellChargeCategory ZeroChargeCategory { get { return WarcraftDatabase.SpellChargeCategories[1]; } }

    public static SpellRange SelfCastRange { get { return WarcraftDatabase.SpellRanges[1]; } }

    public static SpellPowerCost BasicManaCost { get { return WarcraftDatabase.SpellPowerCosts[1]; } }
}