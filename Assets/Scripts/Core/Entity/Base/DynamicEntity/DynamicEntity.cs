using System;
using UnityEngine;

namespace Core
{
    public enum DynamicEntityType
    {
        Portal,
        AreaSpell,
        FarsightFocus
    }

    public class DynamicEntity : WorldEntity, IGridEntity<DynamicEntity>
    {
        public override EntityType EntityType => EntityType.DynamicEntity;
        public override bool AutoScoped => true;

        public GridReference<DynamicEntity> GridRef { get; private set; }

        protected Aura Aura { get; set; }
        protected Aura RemovedAura { get; set; }
        protected Unit Caster { get; set; }

        protected int Duration { get; set; }
        protected uint SpellVisualId { get; set; }
        protected bool IsViewpoint { get; set; }

        public bool IsInGrid() { throw new NotImplementedException(); }
        public void AddToGrid(GridReferenceManager<DynamicEntity> refManager) { throw new NotImplementedException(); }
        public void RemoveFromGrid() { throw new NotImplementedException(); }

        public bool CreateDynamicObject(Guid guidlow, Unit caster, SpellInfo spell, Vector3 pos, float radius, DynamicEntityType type, uint spellXSpellVisualId) { return false; }
        public override void DoUpdate(int timeDelta) { }
        public void Remove() { }
        public void SetDuration(int newDuration) { }
        public int GetDuration() { return 0; }
        public void Delay(int delaytime) { }
        public void SetAura(Aura aura) { }
        public void RemoveAura() { }
        public void SetCasterViewpoint() { }
        public void RemoveCasterViewpoint() { }
        public Unit GetCaster() { return Caster; }
        public void BindToCaster() { }
        public void UnbindFromCaster() { }
        public uint GetSpellId() { return GetUintValue(EntityFields.DynamicSpellid); }
        public float GetRadius() { return GetFloatValue(EntityFields.DynamicRadius); }
    }
}