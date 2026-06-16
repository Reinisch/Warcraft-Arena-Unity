using System;
using Common;
using JetBrains.Annotations;
using UnityEngine;

namespace Client
{
    [Serializable]
    public class TooltipSettingsByAlignmentDictionary : SerializedDictionary<TooltipSettingsByAlignmentDictionary.Entry, TooltipAlignment, TooltipAlignmentSettings>
    {
        [Serializable]
        public class Entry : ISerializedKeyValue<TooltipAlignment, TooltipAlignmentSettings>
        {
            [SerializeField, UsedImplicitly] private TooltipAlignment tooltipAlignment;
            [SerializeField, UsedImplicitly] private TooltipAlignmentSettings tooltipSettings;

            public TooltipAlignment Key => tooltipAlignment;
            public TooltipAlignmentSettings Value => tooltipSettings;
        }
    }
}
