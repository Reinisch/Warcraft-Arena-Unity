using System;
using JetBrains.Annotations;
using UnityEngine;

namespace Core
{
    [Serializable, UsedImplicitly]
    public class SpellProcsPerMinuteModifier
    {
        [SerializeField, UsedImplicitly] private SpellProcsPerMinuteModType modType;
        [SerializeField, UsedImplicitly] private int parameter;
        [SerializeField, UsedImplicitly] private float modValue;

        public SpellProcsPerMinuteModType Type => modType;
        public int Parameter => parameter;
        public float Value => modValue;
    }
}