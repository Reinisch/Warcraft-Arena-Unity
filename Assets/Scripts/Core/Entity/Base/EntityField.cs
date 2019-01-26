using System;
using System.Runtime.InteropServices;
using JetBrains.Annotations;
using UnityEngine;

namespace Core
{
    [Serializable]
    public class EntityField
    {
        [SerializeField, UsedImplicitly] public EntityFieldValue FieldValue;

        [SerializeField, UsedImplicitly] private EntityFields field;
        [SerializeField, UsedImplicitly] private FieldTypes fieldType;

        public EntityFields Field => field;
        public FieldTypes FieldType => fieldType;

#if UNITY_EDITOR
        public dynamic Value
        {
            get
            {
                switch (fieldType)
                {
                    case FieldTypes.Int:
                        return FieldValue.Int;
                    case FieldTypes.Uint:
                        return FieldValue.UInt;
                    case FieldTypes.Ulong:
                        return FieldValue.ULong;
                    case FieldTypes.Long:
                        return FieldValue.Long;
                    case FieldTypes.Float:
                        return FieldValue.Float;
                    case FieldTypes.Double:
                        return FieldValue.Double;
                    case FieldTypes.Short:
                        return FieldValue.Short;
                    case FieldTypes.UShort:
                        return FieldValue.UShort;
                    default:
                        goto case FieldTypes.Int;
                }
            }
        }
#endif

        public EntityField(EntityFields field, FieldTypes fieldType)
        {
            this.field = field;
            this.fieldType = fieldType;
        }
    }

    [Serializable, StructLayout(LayoutKind.Explicit)]
    public struct EntityFieldValue
    {
        [FieldOffset(0)] public long Long;
        [FieldOffset(0), NonSerialized] public ulong ULong;
        [FieldOffset(0), NonSerialized] public float Float;
        [FieldOffset(0), NonSerialized] public double Double;
        [FieldOffset(0), NonSerialized] public int Int;
        [FieldOffset(0), NonSerialized] public uint UInt;
        [FieldOffset(0), NonSerialized] public short Short;
        [FieldOffset(0), NonSerialized] public ushort UShort;

        public static explicit operator EntityFieldValue(long value)
        {
            return new EntityFieldValue { Long = value };
        }

        public static explicit operator EntityFieldValue(ulong value)
        {
            return new EntityFieldValue { ULong = value };
        }

        public static explicit operator EntityFieldValue(float value)
        {
            return new EntityFieldValue { Float = value };
        }

        public static explicit operator EntityFieldValue(double value)
        {
            return new EntityFieldValue { Double = value };
        }

        public static explicit operator EntityFieldValue(uint value)
        {
            return new EntityFieldValue { UInt = value };
        }

        public static explicit operator EntityFieldValue(int value)
        {
            return new EntityFieldValue { Int = value };
        }

        public static explicit operator EntityFieldValue(short value)
        {
            return new EntityFieldValue { Short = value };
        }

        public static explicit operator EntityFieldValue(ushort value)
        {
            return new EntityFieldValue { UShort = value };
        }
    }
}
