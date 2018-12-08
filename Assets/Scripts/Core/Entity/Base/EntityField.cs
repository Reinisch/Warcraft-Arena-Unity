using System;
using System.Runtime.InteropServices;
using JetBrains.Annotations;
using UnityEngine;

namespace Core
{
    [Serializable, StructLayout(LayoutKind.Explicit)]
    public class EntityField
    {
        [FieldOffset(0)] public long Long;
        [FieldOffset(0), NonSerialized] public ulong ULong;
        [FieldOffset(0), NonSerialized] public float Float;
        [FieldOffset(0), NonSerialized] public double Double;
        [FieldOffset(0), NonSerialized] public int Int;
        [FieldOffset(0), NonSerialized] public uint UInt;
        [FieldOffset(0), NonSerialized] public short Short;
        [FieldOffset(0), NonSerialized] public ushort UShort;
        [FieldOffset(0), NonSerialized] public byte Byte0;
        [FieldOffset(1), NonSerialized] public byte Byte1;
        [FieldOffset(2), NonSerialized] public byte Byte2;
        [FieldOffset(3), NonSerialized] public byte Byte3;
        [SerializeField, UsedImplicitly, FieldOffset(8)] private EntityFields field;
        [SerializeField, UsedImplicitly, FieldOffset(12)] private FieldTypes fieldType;

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
                        return Int;
                    case FieldTypes.Uint:
                        return UInt;
                    case FieldTypes.Ulong:
                        return ULong;
                    case FieldTypes.Long:
                        return Long;
                    case FieldTypes.Float:
                        return Float;
                    case FieldTypes.Double:
                        return Double;
                    case FieldTypes.Short:
                        return Short;
                    case FieldTypes.UShort:
                        return UShort;
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
}
