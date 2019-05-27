using System.Collections.Generic;
using Client.Spells;
using Common;
using Core;
using JetBrains.Annotations;
using UnityEngine;
using UnityEngine.Assertions;

namespace Client
{
    public class RenderManager : SingletonGameObject<RenderManager>
    {
        [SerializeField, UsedImplicitly] private Sprite defaultSpellIcon;
        [SerializeField, UsedImplicitly] private List<SpellVisualSettings> spellVisualSettings;

        private WorldManager worldManager;
        private readonly Dictionary<int, SpellVisualSettings> spellVisualSettingsById = new Dictionary<int, SpellVisualSettings>();
        private readonly Dictionary<Unit, UnitRenderer> unitRenderers = new Dictionary<Unit, UnitRenderer>();

        public Sprite DefaultSpellIcon => defaultSpellIcon;
        public IReadOnlyDictionary<int, SpellVisualSettings> SpellVisualSettingsById => spellVisualSettingsById;

        public new void Initialize()
        {
            base.Initialize();

            foreach (SpellVisualSettings spellVisualSetting in spellVisualSettings)
                spellVisualSettingsById.Add(spellVisualSetting.SpellInfo.Id, spellVisualSetting);
        }

        public new void Deinitialize()
        {
            spellVisualSettingsById.Clear();

            base.Deinitialize();
        }

        public void InitializeWorld(WorldManager worldManager)
        {
            this.worldManager = worldManager;

            worldManager.UnitManager.EventEntityAttached += OnEventEntityAttached;
            worldManager.UnitManager.EventEntityDetach += OnEventEntityDetach;

            SpellManager.Instance.EventSpellCast += OnSpellCast;
            SpellManager.Instance.EventSpellDamageDone += OnSpellDamageDone;
        }

        public void DeinitializeWorld()
        {
            SpellManager.Instance.EventSpellDamageDone -= OnSpellDamageDone;
            SpellManager.Instance.EventSpellCast -= OnSpellCast;

            worldManager.UnitManager.EventEntityAttached -= OnEventEntityAttached;
            worldManager.UnitManager.EventEntityDetach -= OnEventEntityDetach;

            foreach (var unitRendererRecord in unitRenderers)
                unitRendererRecord.Value.Deinitialize();

            unitRenderers.Clear();
        }

        public void DoUpdate(int deltaTime)
        {
            foreach (var unitEntry in unitRenderers)
                unitEntry.Value.DoUpdate(deltaTime);
        }

        private void OnSpellDamageDone(Unit caster, Unit target, int damage, bool isCrit)
        {
            if (!unitRenderers.ContainsKey(target))
                return;

            if (caster.IsController)
            {
                GameObject damageEvent = Instantiate(Resources.Load("Prefabs/UI/DamageEvent")) as GameObject;
                Assert.IsNotNull(damageEvent, "damageEvent != null");
                // damageEvent.GetComponent<UnitDamageUIEvent>().Initialize(damage, unitRenderers[target], isCrit, ArenaManager.PlayerInterface);
            }
        }

        private void OnSpellCast(Unit target, SpellInfo spellInfo)
        {
            if (SpellVisualSettingsById.TryGetValue(spellInfo.Id, out SpellVisualSettings spellVisuals))
            {
                spellVisuals.FindEffect(SpellVisualEffect.UsageType.Cast)?.PlayEffect(target.Position, target.Rotation, null);
            }
        }

        private void OnEventEntityAttached(WorldEntity worldEntity)
        {
            var unitEntity = worldEntity as Unit;
            if (unitEntity == null)
                return;

            unitEntity.GetComponentInChildren<UnitRenderer>().Initialize(unitEntity);
            unitRenderers.Add(unitEntity, unitEntity.GetComponentInChildren<UnitRenderer>());
        }

        private void OnEventEntityDetach(WorldEntity worldEntity)
        {
            var unitEntity = worldEntity as Unit;
            if (unitEntity == null)
                return;

            if(unitRenderers.ContainsKey(unitEntity))
            {
                unitRenderers[unitEntity].Deinitialize();
                unitRenderers.Remove(unitEntity);
            }
        }
    }
}