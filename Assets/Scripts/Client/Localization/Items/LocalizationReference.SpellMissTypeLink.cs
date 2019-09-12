using System;
using Client.Localization;
using Core;

namespace Client
{
    public partial class LocalizationReference
    {
        [Serializable]
        private class SpellMissTypeLink
        {
            public SpellMissType SpellMissType;
            public LocalizedString LocalizedString;
        }
    }
}