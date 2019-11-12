using System;
using System.Collections.Generic;
using JetBrains.Annotations;
using UnityEngine;

namespace Client
{
    [Serializable]
    public class ActionBarData
    {
        [SerializeField, UsedImplicitly] private List<ActionButtonData> buttons = new List<ActionButtonData>();

        public List<ActionButtonData> Buttons => buttons;
    }
}
