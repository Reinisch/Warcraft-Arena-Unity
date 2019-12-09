using System;
using JetBrains.Annotations;
using UnityEngine;

namespace Client
{
    public partial class LocalizationReference
    {
        [Serializable]
        private class KeyCodeLink
        {
            [UsedImplicitly] public KeyCode KeyCode;
            [UsedImplicitly] public string String;
        }
    }
}