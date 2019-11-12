using System;
using Client.Localization;
using Core;
using JetBrains.Annotations;

namespace Client
{
    public partial class LocalizationReference
    {
        [Serializable]
        private class ClientConnectFailReasonLink
        {
            [UsedImplicitly] public ClientConnectFailReason FailReason;
            [UsedImplicitly] public LocalizedString LocalizedString;
        }
    }
}