using System;
using System.Collections.Generic;
using Common;
using Core;
using JetBrains.Annotations;
using UnityEngine;

namespace Client
{
    [Serializable]
    public class ClassTypeActionBarSettingsDictionary : SerializedDictionary<ClassTypeActionBarSettingsDictionary.Entry, ClassType, List<ActionBarSettings>>
    {
        [Serializable]
        public class Entry : ISerializedKeyValue<ClassType, List<ActionBarSettings>>
        {
            [SerializeField, UsedImplicitly] private ClassType key;
            [SerializeField, UsedImplicitly] private List<ActionBarSettings> value;

            public ClassType Key => key;
            public List<ActionBarSettings> Value => value;
        }
    }
}
