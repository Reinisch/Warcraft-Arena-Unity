using System;
using Client.Localization;
using Core;
using JetBrains.Annotations;

namespace Client
{
    public partial class LocalizationReference
    {
        [Serializable]
        private class SpellCastResultLink
        {
            [UsedImplicitly] public SpellCastResult SpellCastResult;
            [UsedImplicitly] public LocalizedString LocalizedString;
        }
    }
}