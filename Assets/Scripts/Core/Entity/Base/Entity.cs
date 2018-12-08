using System;
using System.Collections.Generic;
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

        [SerializeField] private List<EntityField> entityFieldList = new List<EntityField>();

        private readonly Dictionary<EntityFields, EntityField> entityFields = new Dictionary<EntityFields, EntityField>(new EntityFieldsEqualityComparer());
        protected WorldManager worldManager;

        public bool IsOwner => entity.isOwner;
        public bool IsController => entity.hasControl;
        public ulong NetworkId => entity.networkId.PackedValue;
        public BoltEntity BoltEntity => entity;

        public abstract EntityType TypeId { get; }
        public bool InWorld { get; private set; }

        public Unit AsUnit => this as Unit;
        public Player AsPlayer => this as Player;
        public GameEntity AsGameEntity => this as GameEntity;

        [UsedImplicitly]
        protected virtual void Awake()
        {
            foreach (var entityField in entityFieldList)
                entityFields.Add(entityField.Field, entityField);
        }

        public void TakenFromPool(WorldManager worldManager)
        {
            this.worldManager = worldManager;
        }

        public void ReturnedToPool()
        {
            worldManager = null;
        }

        #region Field getters/setters and modifiers

        public int GetIntValue(EntityFields field)
        {
            Assert.IsTrue(entityFields.ContainsKey(field), EntityFieldErrorMessage(field, false));
            return entityFields[field].Int;
        }
        public uint GetUintValue(EntityFields field)
        {
            Assert.IsTrue(entityFields.ContainsKey(field), EntityFieldErrorMessage(field, false));
            return entityFields[field].UInt;
        }
        public long GetLongValue(EntityFields field)
        {
            Assert.IsTrue(entityFields.ContainsKey(field), EntityFieldErrorMessage(field, false));
            return entityFields[field].Long;
        }
        public ulong GetULongValue(EntityFields field)
        {
            Assert.IsTrue(entityFields.ContainsKey(field), EntityFieldErrorMessage(field, false));
            return entityFields[field].ULong;
        }
        public float GetFloatValue(EntityFields field)
        {
            Assert.IsTrue(entityFields.ContainsKey(field), EntityFieldErrorMessage(field, false));
            return entityFields[field].Float;
        }
        public byte GetByteValue(EntityFields field, byte offset)
        {
            Assert.IsTrue(entityFields.ContainsKey(field), EntityFieldErrorMessage(field, false));
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
        public short GetShortValue(EntityFields field)
        {
            Assert.IsTrue(entityFields.ContainsKey(field), EntityFieldErrorMessage(field, false));

            return entityFields[field].Short;
        }
        public Guid GetGuidValue(EntityFields field)
        {
            Assert.IsTrue(entityFields.ContainsKey(field), EntityFieldErrorMessage(field, false));

            return Guid.Empty;
        }

        public void SetIntValue(EntityFields field, int value)
        {
            Assert.IsTrue(entityFields.ContainsKey(field), EntityFieldErrorMessage(field, true));

            entityFields[field].Int = value;
        }
        public void SetUintValue(EntityFields field, uint value)
        {
            Assert.IsTrue(entityFields.ContainsKey(field), EntityFieldErrorMessage(field, true));

            entityFields[field].UInt = value;
        }
        public void SetLongValue(EntityFields field, long value)
        {
            Assert.IsTrue(entityFields.ContainsKey(field), EntityFieldErrorMessage(field, true));

            entityFields[field].Long = value;
        }
        public void SetULongValue(EntityFields field, ulong value)
        {
            Assert.IsTrue(entityFields.ContainsKey(field), EntityFieldErrorMessage(field, true));

            entityFields[field].ULong = value;
        }
        public void SetFloatValue(EntityFields field, float value)
        {
            Assert.IsTrue(entityFields.ContainsKey(field), EntityFieldErrorMessage(field, true));

            entityFields[field].Float = value;
        }
        public void SetByteValue(EntityFields field, byte offset, byte value)
        {
            Assert.IsTrue(entityFields.ContainsKey(field), EntityFieldErrorMessage(field, true));
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
        public void SetShortValue(EntityFields field, byte offset, short value)
        {
            Assert.IsTrue(entityFields.ContainsKey(field), EntityFieldErrorMessage(field, true));
            Assert.IsFalse(offset > 1, "Trying to use wrong offset in WorldEntity.SetShortValue! Offset: " + offset);

            entityFields[field].Short = value;
        }

        public void SetGuidValue(EntityFields field, Guid value)
        {
            //Assert.IsTrue(entityFields.ContainsKey(field), EntityFieldErrorMessage(field, true));
        }
        public void SetStatFloatValue(EntityFields field, float value)
        {
            if (value < 0)
                value = 0.0f;

            SetFloatValue(field, value);
        }
        public void SetStatIntValue(EntityFields field, int value)
        {
            if (value < 0)
                value = 0;

            SetUintValue(field, (uint)value);
        }

        public void ApplyModUInt32Value(EntityFields field, int val, bool apply) { }
        public void ApplyModInt32Value(EntityFields field, int val, bool apply) { }
        public void ApplyModPositiveFloatValue(EntityFields field, float val, bool apply) { }
        public void ApplyModSignedFloatValue(EntityFields field, float val, bool apply) { }
        public void ApplyPercentModFloatValue(EntityFields field, float val, bool apply) { }

        public void SetFlag(EntityFields field, uint newFlag) { }
        public void RemoveFlag(EntityFields field, uint oldFlag) { }
        public void ToggleFlag(EntityFields field, uint flag) { }
        public bool HasFlag(EntityFields field, uint flag) { return false; }
        public void ApplyModFlag(EntityFields field, uint flag, bool apply) { }

        public void SetByteFlag(EntityFields field, byte offset, byte newFlag) { }
        public void RemoveByteFlag(EntityFields field, byte offset, byte newFlag) { }
        public void ToggleByteFlag(EntityFields field, byte offset, byte flag) { }
        public bool HasByteFlag(EntityFields field, byte offset, byte flag) { return false; }

        public void SetLongFlag(EntityFields field, long newFlag) { }
        public void RemoveLongFlag(EntityFields field, long oldFlag) { }
        public void ToggleLongFlag(EntityFields field, long flag) { }
        public bool HasLongFlag(EntityFields field, long flag) { return false; }
        public void ApplyLongModFlag(EntityFields field, long flag, bool apply) { }

        #endregion

        private string EntityFieldErrorMessage(EntityFields field, bool set)
        {
            var errorAction = set ? "to set value to" : "to get value from";
            return $"Attempted {errorAction} non-existing field: {field} for entity with type: {TypeId}";
        }
    }
}