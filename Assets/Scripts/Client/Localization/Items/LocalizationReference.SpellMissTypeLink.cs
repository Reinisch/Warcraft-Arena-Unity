using System;
using Client.Localization;
using Core;
using JetBrains.Annotations;

namespace Client
{
    public partial class LocalizationReference
    {
        [Serializable]
        private class SpellMissTypeLink
        {
            [UsedImplicitly] public SpellMissType SpellMissType;
            [UsedImplicitly] public LocalizedString LocalizedString;
        }
    }
}