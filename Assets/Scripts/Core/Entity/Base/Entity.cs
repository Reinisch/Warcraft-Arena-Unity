using System.Collections.Generic;
using System.Diagnostics;
using Bolt;
using JetBrains.Annotations;
using UdpKit;
using UnityEngine;
using UnityEngine.Assertions;

namespace Core
{
    public abstract class Entity : EntityBehaviour
    {
        public abstract class CreateInfo : IProtocolToken
        {
            public abstract void Read(UdpPacket packet);

            public abstract void Write(UdpPacket packet);
        }

        [SerializeField, UsedImplicitly] private List<EntityField> entityFieldList = new List<EntityField>();

        private readonly Dictionary<EntityFields, EntityField> entityFields = new Dictionary<EntityFields, EntityField>(new EntityFieldsEqualityComparer());

        public bool IsOwner => entity.isOwner;
        public bool IsController => entity.hasControl;
        public ulong NetworkId => entity.networkId.PackedValue;
        public BoltEntity BoltEntity => entity;

        internal WorldManager WorldManager { get; private set; }

        public abstract EntityType EntityType { get; }
        public abstract bool AutoScoped { get; }
        public bool IsValid { get; private set; }

        [UsedImplicitly]
        internal virtual void Awake()
        {
            foreach (var entityField in entityFieldList)
                entityFields.Add(entityField.Field, entityField);
        }

        public override void Attached()
        {
            base.Attached();

            IsValid = true;
        }

        public override void Detached()
        {
            IsValid = false;

            base.Detached();
        }

        internal void TakenFromPool(WorldManager worldManager)
        {
            WorldManager = worldManager;
        }

        internal void ReturnedToPool()
        {
            WorldManager = null;
        }

        #region Field Accessors

        public int GetIntValue(EntityFields field)
        {
            ValidateField(field, -1);
            return entityFields[field].Int;
        }

        public uint GetUintValue(EntityFields field)
        {
            ValidateField(field, -1);
            return entityFields[field].UInt;
        }

        public long GetLongValue(EntityFields field)
        {
            ValidateField(field, -1);
            return entityFields[field].Long;
        }

        public ulong GetULongValue(EntityFields field)
        {
            ValidateField(field, -1);
            return entityFields[field].ULong;
        }

        public float GetFloatValue(EntityFields field)
        {
            ValidateField(field, -1);
            return entityFields[field].Float;
        }

        public short GetShortValue(EntityFields field)
        {
            ValidateField(field, -1);
            return entityFields[field].Short;
        }

        public bool HasFlag(EntityFields field, uint flag)
        {
            return false;
        }

        public byte GetByteValue(EntityFields field, byte offset)
        {
            ValidateField(field, -1);
            Assert.IsTrue(offset < 4, "Trying to use wrong offset in WorldEntity.GetByteValue! Offset: " + offset);

            if (offset == 0)
            {
                return entityFields[field].Byte0;
            }

            if (offset == 1)
            {
                return entityFields[field].Byte1;
            }

            if (offset == 2)
            {
                return entityFields[field].Byte2;
            }

            return entityFields[field].Byte3;
        }

        internal void SetIntValue(EntityFields field, int value)
        {
            ValidateField(field, 1);
            entityFields[field].Int = value;
        }

        internal void SetUintValue(EntityFields field, uint value)
        {
            ValidateField(field, 1);
            entityFields[field].UInt = value;
        }

        internal void SetLongValue(EntityFields field, long value)
        {
            ValidateField(field, 1);
            entityFields[field].Long = value;
        }

        internal void SetULongValue(EntityFields field, ulong value)
        {
            ValidateField(field, 1);
            entityFields[field].ULong = value;
        }

        internal void SetFloatValue(EntityFields field, float value)
        {
            ValidateField(field, 1);
            entityFields[field].Float = value;
        }

        internal void SetByteValue(EntityFields field, byte offset, byte value)
        {
            ValidateField(field, 1);
            Assert.IsTrue(offset < 4, "Trying to use wrong offset in WorldEntity.SetByteValue! Offset: " + offset);

            if (offset == 0)
            {
                entityFields[field].Byte0 = value;
            }
            else if (offset == 1)
            {
                entityFields[field].Byte1 = value;
            }
            else if (offset == 2)
            {
                entityFields[field].Byte2 = value;
            }
            else
            {
                entityFields[field].Byte3 = value;
            }
        }

        internal void SetShortValue(EntityFields field, byte offset, short value)
        {
            ValidateField(field, 1);
            Assert.IsFalse(offset > 1, "Trying to use wrong offset in WorldEntity.SetShortValue! Offset: " + offset);

            entityFields[field].Short = value;
        }

        internal void SetStatFloatValue(EntityFields field, float value)
        {
            if (value < 0)
                value = 0.0f;

            SetFloatValue(field, value);
        }

        internal void SetStatIntValue(EntityFields field, int value)
        {
            if (value < 0)
                value = 0;

            SetUintValue(field, (uint) value);
        }

        internal void SetFlag(EntityFields field, uint newFlag)
        {
        }

        internal void RemoveFlag(EntityFields field, uint oldFlag)
        {
        }

        internal void ToggleFlag(EntityFields field, uint flag)
        {
        }
        
        internal void ApplyModFlag(EntityFields field, uint flag, bool apply)
        {
        }

        #endregion

        public virtual void Accept(IVisitor visitor)
        {
            visitor.Visit(this);
        }

        [Conditional("UNITY_ASSERTIONS")]
        private void ValidateField(EntityFields field, int set)
        {
            string message = $"Attempted {set:\"to set value to\",\"to get value from\"} non-existing field: {field} for entity with type: {EntityType}";

            Assert.IsTrue(entityFields.ContainsKey(field), message);
        }
    }
}