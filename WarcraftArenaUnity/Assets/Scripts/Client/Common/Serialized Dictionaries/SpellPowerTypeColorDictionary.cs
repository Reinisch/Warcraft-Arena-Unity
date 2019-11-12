using System;
using Common;
using Core;
using JetBrains.Annotations;
using UnityEngine;

namespace Client
{
    [Serializable]
    public class SpellPowerTypeColorDictionary : SerializedDictionary<SpellPowerTypeColorDictionary.Entry, SpellPowerType, Color>
    {
        [Serializable]
        public class Entry : ISerializedKeyValue<SpellPowerType, Color>
        {
            [SerializeField, UsedImplicitly] private SpellPowerType key;
            [SerializeField, UsedImplicitly] private Color value;

            public SpellPowerType Key => key;
            public Color Value => value;
        }
    }
}
