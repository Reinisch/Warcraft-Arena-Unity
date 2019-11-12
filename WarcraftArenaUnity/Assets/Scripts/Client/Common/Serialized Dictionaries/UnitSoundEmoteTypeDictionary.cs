using System;
using Common;
using Core;
using JetBrains.Annotations;
using UnityEngine;

namespace Client
{
    [Serializable]
    public class UnitSoundEmoteTypeDictionary : SerializedDictionary<UnitSoundEmoteTypeDictionary.Entry, EmoteType, UnitSounds>
    {
        [Serializable]
        public class Entry : ISerializedKeyValue<EmoteType, UnitSounds>
        {
            [SerializeField, UsedImplicitly] private EmoteType emoteType;
            [SerializeField, UsedImplicitly] private UnitSounds soundType;

            public EmoteType Key => emoteType;
            public UnitSounds Value => soundType;
        }
    }
}
