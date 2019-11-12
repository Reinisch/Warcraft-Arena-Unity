using System;
using Common;
using JetBrains.Annotations;
using UnityEngine;

namespace Client
{
    [Serializable]
    public class SoundEntryCharacterSoundDictionary : SerializedDictionary<SoundEntryCharacterSoundDictionary.Entry, UnitSounds, SoundEntry>
    {
        [Serializable]
        public class Entry : ISerializedKeyValue<UnitSounds, SoundEntry>
        {
            [SerializeField, UsedImplicitly] private UnitSounds soundType;
            [SerializeField, UsedImplicitly] private SoundEntry soundEntry;

            public UnitSounds Key => soundType;
            public SoundEntry Value => soundEntry;
        }
    }
}