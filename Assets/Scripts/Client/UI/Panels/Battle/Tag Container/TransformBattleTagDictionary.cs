using System;
using Common;
using JetBrains.Annotations;
using UnityEngine;

namespace Client
{
    [Serializable]
    public class TransformBattleTagDictionary : SerializedDictionary<TransformBattleTagDictionary.Entry, BattleHudTagType, RectTransform>
    {
        [Serializable]
        public class Entry : ISerializedKeyValue<BattleHudTagType, RectTransform>
        {
            [SerializeField, UsedImplicitly] private BattleHudTagType tag;
            [SerializeField, UsedImplicitly] private RectTransform rectTransform;

            public BattleHudTagType Key => tag;
            public RectTransform Value => rectTransform;
        }
    }
}