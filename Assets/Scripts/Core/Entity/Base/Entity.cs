using System.Collections.Generic;
using System.Diagnostics;
using Bolt;
using JetBrains.Annotations;
using UdpKit;
using UnityEngine;
using Common;

namespace Core
{
    public abstract class Entity : EntityBehaviour
    {
        public abstract class CreateInfo : IProtocolToken
        {
            public abstract void Read(UdpPacket packet);

            public abstract void Write(UdpPacket packet);
        }

        [SerializeField, UsedImplicitly] private List<EntityField> defaultFieldValues = new List<EntityField>();

        private readonly Dictionary<EntityFields, EntityFieldValue> entityFields = new Dictionary<EntityFields, EntityFieldValue>(new StatUtils.EntityFieldsComparer());
        private readonly bool validateFieldAccess = false;

        internal WorldManager WorldManager { get; private set; }

        public BoltEntity BoltEntity => entity;
        public bool IsOwner => entity.isOwner;
        public bool IsController => entity.hasControl;
        public ulong NetworkId => entity.networkId.PackedValue;

        public bool IsValid { get; private set; }
        internal abstract bool AutoScoped { get; }
        public abstract EntityType EntityType { get; }

        [UsedImplicitly]
        private void Awake()
        {
            foreach (var entityField in StatUtils.GetEntityFields(EntityType))
                entityFields[entityField] = default;
        }

        public override void Attached()
        {
            base.Attached();

            foreach (var entityField in defaultFieldValues)
                entityFields[entityField.Field] = entityField.FieldValue;

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

        internal void SetIntValue(EntityFields field, int value)
        {
            ValidateField(field, 1);
            entityFields[field] = (EntityFieldValue)value;
        }

        internal void SetUintValue(EntityFields field, uint value)
        {
            ValidateField(field, 1);
            entityFields[field] = (EntityFieldValue)value;
        }

        internal void SetLongValue(EntityFields field, long value)
        {
            ValidateField(field, 1);
            entityFields[field] = (EntityFieldValue)value;
        }

        internal void SetULongValue(EntityFields field, ulong value)
        {
            ValidateField(field, 1);
            entityFields[field] = (EntityFieldValue)value;
        }

        internal void SetFloatValue(EntityFields field, float value)
        {
            ValidateField(field, 1);
            entityFields[field] = (EntityFieldValue)value;
        }

        internal void SetShortValue(EntityFields field, byte offset, short value)
        {
            ValidateField(field, 1);
            Assert.IsFalse(offset > 1, "Trying to use wrong offset in WorldEntity.SetShortValue! Offset: " + offset);

            entityFields[field] = (EntityFieldValue)value;
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

        [Conditional(Assert.AssertionDefine)]
        private void ValidateField(EntityFields field, int set)
        {
            if (!validateFieldAccess)
                return;

            string message = $"Attempted {set:'to set value to';'to get value from'} non-existing field: {field} for entity with type: {EntityType}";
            Assert.IsTrue(entityFields.ContainsKey(field), message);
        }
    }
}