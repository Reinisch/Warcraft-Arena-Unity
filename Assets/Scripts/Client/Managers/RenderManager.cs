using System.Collections.Generic;
using Core;
using JetBrains.Annotations;
using UnityEngine;
using UnityEngine.Assertions;

namespace Client
{
    public class RenderManager : SingletonGameObject<RenderManager>
    {
        [SerializeField, UsedImplicitly] private Sprite defaultSpellIcon;

        private WorldManager worldManager;

        public Sprite DefaultSpellIcon => defaultSpellIcon;

        private Dictionary<Unit, UnitRenderer> UnitRenderers { get; } = new Dictionary<Unit, UnitRenderer>();

        public void Initialize(WorldManager worldManager)
        {
            this.worldManager = worldManager;

            worldManager.UnitManager.EventEntityAttached += OnEventEntityAttached;
            worldManager.UnitManager.EventEntityDetach += OnEventEntityDetach;

            SpellManager.Instance.EventSpellCast += OnSpellCast;
            SpellManager.Instance.EventSpellDamageDone += OnSpellDamageDone;          
        }

        public void Deinitialize()
        {
            SpellManager.Instance.EventSpellDamageDone -= OnSpellDamageDone;
            SpellManager.Instance.EventSpellCast -= OnSpellCast;

            worldManager.UnitManager.EventEntityAttached -= OnEventEntityAttached;
            worldManager.UnitManager.EventEntityDetach -= OnEventEntityDetach;

            foreach (var unitRendererRecord in UnitRenderers)
                unitRendererRecord.Value.Deinitialize();

            UnitRenderers.Clear();
        }

        public void DoUpdate(int deltaTime)
        {
            foreach (var unitEntry in UnitRenderers)
                unitEntry.Value.DoUpdate(deltaTime);
        }

        private void OnSpellDamageDone(Unit caster, Unit target, int damage, bool isCrit)
        {
            if (!UnitRenderers.ContainsKey(target))
                return;

            if (caster.IsController)
            {
                GameObject damageEvent = Instantiate(Resources.Load("Prefabs/UI/DamageEvent")) as GameObject;
                Assert.IsNotNull(damageEvent, "damageEvent != null");
                // damageEvent.GetComponent<UnitDamageUIEvent>().Initialize(damage, UnitRenderers[target], isCrit, ArenaManager.PlayerInterface);
            }
        }

        private void OnSpellCast(Unit target, SpellInfo spellInfo)
        {
            GameObject spellRenderer = spellInfo.VisualSettings.FindEffect(SpellVisualEntry.UsageType.Cast);

            if (spellRenderer != null)
                Instantiate(spellRenderer, target.Position, Quaternion.identity);
        }

        private void OnEventEntityAttached(WorldEntity worldEntity)
        {
            var unitEntity = worldEntity as Unit;
            if (unitEntity == null)
                return;

            unitEntity.GetComponentInChildren<UnitRenderer>().Initialize(unitEntity);
            UnitRenderers.Add(unitEntity, unitEntity.GetComponentInChildren<UnitRenderer>());
        }

        private void OnEventEntityDetach(WorldEntity worldEntity)
        {
            var unitEntity = worldEntity as Unit;
            if (unitEntity == null)
                return;

            if(UnitRenderers.ContainsKey(unitEntity))
            {
                UnitRenderers[unitEntity].Deinitialize();
                UnitRenderers.Remove(unitEntity);
            }
        }
    }
}