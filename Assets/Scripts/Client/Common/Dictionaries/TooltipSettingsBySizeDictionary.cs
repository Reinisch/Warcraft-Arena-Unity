using System;
using Common;
using JetBrains.Annotations;
using UnityEngine;

namespace Client
{
    [Serializable]
    public class TooltipSettingsBySizeDictionary : SerializedDictionary<TooltipSettingsBySizeDictionary.Entry, TooltipSize, TooltipSizeSettings>
    {
        [Serializable]
        public class Entry : ISerializedKeyValue<TooltipSize, TooltipSizeSettings>
        {
            [SerializeField, UsedImplicitly] private TooltipSize tooltipSize;
            [SerializeField, UsedImplicitly] private TooltipSizeSettings tooltipSettings;

            public TooltipSize Key => tooltipSize;
            public TooltipSizeSettings Value => tooltipSettings;
        }
    }
}
