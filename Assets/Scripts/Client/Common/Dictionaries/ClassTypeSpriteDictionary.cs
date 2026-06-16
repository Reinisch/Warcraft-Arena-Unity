using System;
using Common;
using Core;
using JetBrains.Annotations;
using UnityEngine;

namespace Client
{
    [Serializable]
    public class ClassTypeSpriteDictionary : SerializedDictionary<ClassTypeSpriteDictionary.ClassIconSprite, ClassType, Sprite>
    {
        [Serializable]
        public class ClassIconSprite : ISerializedKeyValue<ClassType, Sprite>
        {
            [SerializeField, UsedImplicitly] private ClassType classType;
            [SerializeField, UsedImplicitly] private Sprite sprite;

            public ClassType Key => classType;
            public Sprite Value => sprite;
        }
    }
}
