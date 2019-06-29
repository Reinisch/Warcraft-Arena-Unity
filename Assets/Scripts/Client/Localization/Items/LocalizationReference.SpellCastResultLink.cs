using System;
using Client.Localization;
using Core;

namespace Client
{
    public partial class LocalizationReference
    {
        [Serializable]
        private class SpellCastResultLink
        {
            public SpellCastResult SpellCastResult;
            public LocalizedString LocalizedString;
        }
    }
}