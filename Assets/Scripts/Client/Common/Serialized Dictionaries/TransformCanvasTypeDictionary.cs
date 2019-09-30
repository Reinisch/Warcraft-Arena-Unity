using System;
using Common;
using JetBrains.Annotations;
using UnityEngine;

namespace Client
{
    [Serializable]
    public class TransformCanvasTypeDictionary : SerializedDictionary<TransformCanvasTypeDictionary.Entry, InterfaceCanvasType, RectTransform>
    {
        [Serializable]
        public class Entry : ISerializedKeyValue<InterfaceCanvasType, RectTransform>
        {
            [SerializeField, UsedImplicitly] private InterfaceCanvasType canvasType;
            [SerializeField, UsedImplicitly] private RectTransform rectTransform;

            public InterfaceCanvasType Key => canvasType;
            public RectTransform Value => rectTransform;
        }
    }
}