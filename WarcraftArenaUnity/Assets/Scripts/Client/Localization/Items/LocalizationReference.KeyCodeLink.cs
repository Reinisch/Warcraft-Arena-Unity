using System;
using UnityEngine;

namespace Client
{
    public partial class LocalizationReference
    {
        [Serializable]
        private class KeyCodeLink
        {
            public KeyCode KeyCode;
            public string String;
        }
    }
}