using System;
using Client.Localization;
using Core;

namespace Client
{
    public partial class LocalizationReference
    {
        [Serializable]
        private class PowerTypeCostLink
        {
            public SpellPowerType PowerType;
            public LocalizedString LocalizedRawString;
            public LocalizedString LocalizedPercentageString;
        }
    }
}