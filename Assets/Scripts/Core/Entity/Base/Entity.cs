using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Assertions;

namespace Core
{
    public abstract class Entity : MonoBehaviour
    {
        private Dictionary<EntityFields, bool> ChangesMask { get; set; }
        private Dictionary<EntityFields, dynamic> UpdateFields { get; set; }

        protected bool EntityUpdated { get; set; }
        protected UpdateFlags UpdateFlag { get; set; }
        protected FieldFlags FieldNotifyFlags { get; set; }

        public bool InWorld { get; protected set; }
        public EntityType TypeId { get; protected set; }
        public EntityTypeMask TypeMask { get; protected set; }
        public Guid Guid { get { return GetGuidValue(EntityFields.Guid); } protected set { SetGuidValue(EntityFields.Guid, value); } }
        public uint Entry { get { return GetUintValue(EntityFields.Entry); } set { SetUintValue(EntityFields.Entry, value); } }
        public virtual float Scale { get { return GetFloatValue(EntityFields.ScaleX); } set { SetFloatValue(EntityFields.ScaleX, value); } }

        public Unit AsUnit => this as Unit;
        public Player AsPlayer => this as Player;
        public GameEntity AsGameEntity => this as GameEntity;

        public virtual void SendUpdateToPlayer(Player player) { }
        public virtual void DestroyForPlayer(Player target) { }

        #region Field getters/setters and modifiers

        public int GetIntValue(EntityFields field)
        {
            Assert.IsTrue(UpdateFields.ContainsKey(field), EntityFieldErrorMessage(field, false));
            return UpdateFields[field];
        }
        public uint GetUintValue(EntityFields field)
        {
            Assert.IsTrue(UpdateFields.ContainsKey(field), EntityFieldErrorMessage(field, false));
            return UpdateFields[field];
        }
        public long GetLongValue(EntityFields field)
        {
            Assert.IsTrue(UpdateFields.ContainsKey(field), EntityFieldErrorMessage(field, false));
            return UpdateFields[field];
        }
        public float GetFloatValue(EntityFields field)
        {
            Assert.IsTrue(UpdateFields.ContainsKey(field), EntityFieldErrorMessage(field, false));
            return UpdateFields[field];
        }
        public byte GetByteValue(EntityFields field, byte offset)
        {
            Assert.IsTrue(UpdateFields.ContainsKey(field), EntityFieldErrorMessage(field, false));
            Assert.IsTrue(offset < 4, "Trying to use wrong offset in WorldEntity.GetByteValue! Offset: " + offset);

            return BitConverter.GetBytes((uint)UpdateFields[field])[offset];
        }
        public short GetShortValue(EntityFields field, byte offset)
        {
            Assert.IsTrue(UpdateFields.ContainsKey(field), EntityFieldErrorMessage(field, false));
            Assert.IsTrue(offset < 2, "Trying to use wrong offset in WorldEntity.GetShortValue! Offset: " + offset);

            return BitConverter.ToInt16(BitConverter.GetBytes((uint)UpdateFields[field]), offset);
        }
        public Guid GetGuidValue(EntityFields field)
        {
            //Assert.IsTrue(UpdateFields.ContainsKey(field), EntityFieldErrorMessage(field, false));
            return UpdateFields[field];
        }

        public void SetIntValue(EntityFields field, int value)
        {
            Assert.IsTrue(UpdateFields.ContainsKey(field), EntityFieldErrorMessage(field, true));

            if (UpdateFields[field] != value)
            {
                UpdateFields[field] = value;
                ChangesMask[field] = true;

                AddToEntityUpdateIfNeeded();
            }
        }
        public void SetUintValue(EntityFields field, uint value)
        {
            Assert.IsTrue(UpdateFields.ContainsKey(field), EntityFieldErrorMessage(field, true));

            if (UpdateFields[field] != value)
            {
                UpdateFields[field] = value;
                ChangesMask[field] = true;

                AddToEntityUpdateIfNeeded();
            }
        }
        public void SetLongValue(EntityFields field, long value)
        {
            Assert.IsTrue(UpdateFields.ContainsKey(field), EntityFieldErrorMessage(field, true));

            if (UpdateFields[field] != value)
            {
                UpdateFields[field] = value;
                ChangesMask[field] = true;

                AddToEntityUpdateIfNeeded();
            }
        }
        public void SetFloatValue(EntityFields field, float value)
        {
            Assert.IsTrue(UpdateFields.ContainsKey(field), EntityFieldErrorMessage(field, true));

            if (!Mathf.Approximately((float)UpdateFields[field], value))
            {
                UpdateFields[field] = value;
                ChangesMask[field] = true;

                AddToEntityUpdateIfNeeded();
            }
        }
        public void SetByteValue(EntityFields field, byte offset, byte value)
        {
            Assert.IsTrue(UpdateFields.ContainsKey(field), EntityFieldErrorMessage(field, true));
            Assert.IsFalse(offset > 3, "Trying to use wrong offset in WorldEntity.SetByteValue! Offset: " + offset);

            if ((byte)(UpdateFields[field] >> (offset * 8)) != value)
            {
                UpdateFields[field] &= ~((uint)0xFF << (offset * 8));
                UpdateFields[field] |= (uint)value << (offset * 8);
                ChangesMask[field] = true;

                AddToEntityUpdateIfNeeded();
            }
        }
        public void SetShortValue(EntityFields field, byte offset, short value)
        {
            Assert.IsTrue(UpdateFields.ContainsKey(field), EntityFieldErrorMessage(field, true));
            Assert.IsFalse(offset > 1, "Trying to use wrong offset in WorldEntity.SetShortValue! Offset: " + offset);

            if ((byte)(UpdateFields[field] >> (offset * 16)) != value)
            {
                UpdateFields[field] &= ~((uint)0xFFFF << (offset * 16));
                UpdateFields[field] |= (uint)value << (offset * 16);
                ChangesMask[field] = true;

                AddToEntityUpdateIfNeeded();
            }
        }
        public void SetGuidValue(EntityFields field, Guid value)
        {
            //Assert.IsTrue(UpdateFields.ContainsKey(field), EntityFieldErrorMessage(field, true));

            if (UpdateFields[field] != value)
            {
                UpdateFields[field] = value;
                ChangesMask[field] = true;

                AddToEntityUpdateIfNeeded();
            }
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

        public bool AddGuidValue(EntityFields field, Guid value)
        {
            Assert.IsTrue(UpdateFields.ContainsKey(field), EntityFieldErrorMessage(field, true));

            if (value != Guid.Empty && UpdateFields[field] == Guid.Empty)
            {
                UpdateFields[field] = value;
                ChangesMask[field] = true;

                AddToEntityUpdateIfNeeded();
                return true;
            }

            return false;
        }
        public bool RemoveGuidValue(EntityFields field, Guid value)
        {
            Assert.IsTrue(UpdateFields.ContainsKey(field), EntityFieldErrorMessage(field, true));

            if (value != Guid.Empty && UpdateFields[field] == value)
            {
                UpdateFields[field] = Guid.Empty;
                ChangesMask[field] = true;

                AddToEntityUpdateIfNeeded();
                return true;
            }

            return false;
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

        #region Creation and update methods

        public virtual void Initialize(bool isWorldEntity, Guid guid)
        {
            Create();

            SetGuidValue(EntityFields.Guid, guid);
            TypeId = EntityType.Entity;
            TypeMask = EntityTypeMask.Object;
            UpdateFlag = UpdateFlags.None;
            FieldNotifyFlags = FieldFlags.Dynamic;
        }

        public virtual void Deinitialize()
        {
            InWorld = false;
            EntityUpdated = false;
        }

        private void Create()
        {
            if (UpdateFields != null)
                return;

            ChangesMask = new Dictionary<EntityFields, bool>(new EntityFieldsEqualityComparer());
            UpdateFields = new Dictionary<EntityFields, dynamic>(new EntityFieldsEqualityComparer());

            foreach (var field in EntityFieldHelper.GetEntityFields(TypeId))
            {
                switch (field.GetFieldType())
                {
                    case FieldTypes.Int:
                    case FieldTypes.Uint:
                    case FieldTypes.Long:
                        UpdateFields.Add(field, 0);
                        break;
                    case FieldTypes.Float:
                        UpdateFields.Add(field, 0.0f);
                        break;
                    case FieldTypes.Guid:
                        UpdateFields.Add(field, Guid.Empty);
                        break;
                    default:
                        throw new NotImplementedException("EntityField: " + field + " has type: " + field.GetFieldType() + " that is not implemented yet!");
                }

                ChangesMask.Add(field, false);
            }

            SetIntValue(EntityFields.Type, (int)TypeMask);
        }
        
        public virtual void AddToWorld()
        {
            if (InWorld)
                return;

            Assert.IsTrue(UpdateFields.Count != 0, "Adding non-initialized object in the world! Type: " + TypeId);

            InWorld = true;

            ClearUpdateMask(false);
        }

        public virtual void RemoveFromWorld()
        {
            if (!InWorld)
                return;

            InWorld = false;
            ClearUpdateMask(true);
        }

        protected void AddToEntityUpdateIfNeeded()
        {
            if (InWorld && !EntityUpdated)
            {
                AddToEntityUpdate();
                EntityUpdated = true;
            }
        }

        protected abstract void AddToEntityUpdate();

        protected abstract void RemoveFromEntityUpdate();

        private void ClearUpdateMask(bool remove)
        {
            if (EntityUpdated)
            {
                if (remove)
                    RemoveFromEntityUpdate();
                EntityUpdated = false;
            }
        }

        #endregion

        private string EntityFieldErrorMessage(EntityFields field, bool set)
        {
            var errorAction = set ? "to set value to" : "to get value from";
            return $"Attempted {errorAction} non-existing field: {field} for entity with type: {TypeId} and mask: {TypeMask}";
        }
    }
}