using System;
using Client.Localization;
using Core;
using JetBrains.Annotations;

namespace Client
{
    public partial class LocalizationReference
    {
        [Serializable]
        private class PowerTypeCostLink
        {
            [UsedImplicitly] public SpellPowerType PowerType;
            [UsedImplicitly] public LocalizedString LocalizedRawString;
            [UsedImplicitly] public LocalizedString LocalizedPercentageString;
        }
    }
}