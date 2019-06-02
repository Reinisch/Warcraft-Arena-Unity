using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using Common;

namespace Core
{
    public class Map
    {
        private readonly WorldGrid mapGrid = new WorldGrid();
        private readonly List<WorldEntity> worldEntities = new List<WorldEntity>();

        private WorldManager WorldManager { get; set; }
        private MapDefinition Definition { get; }

        public MapSettings Settings { get; private set; }

        internal Map(int id, Map parent = null)
        {
            Definition = BalanceManager.MapsById[id];
        }

        internal void Initialize(WorldManager worldManager, Scene mapScene)
        {
            WorldManager = worldManager;

            foreach (var rootObject in mapScene.GetRootGameObjects())
            {
                Settings = rootObject.GetComponentInChildren<MapSettings>();

                if (Settings != null)
                    break;
            }

            Assert.IsNotNull(Settings, $"Map settings are missing in map: {Definition.MapName} Id: {Definition.Id}");
            mapGrid.Initialize(this);

            foreach (var scenarioAction in Settings.ScenarioActions)
                scenarioAction.Initialize(this);
        }

        internal void Deinitialize()
        {
            foreach (var scenarioAction in Settings.ScenarioActions)
                scenarioAction.DeInitialize();

            mapGrid.Deinitialize();

            Settings = null;
            WorldManager = null;
        }

        internal void DoUpdate(int deltaTime)
        {
            mapGrid.DoUpdate(deltaTime);

            foreach (var scenarioAction in Settings.ScenarioActions)
                scenarioAction.DoUpdate(deltaTime);
        }

        internal void DelayedUpdate(int deltaTime)
        {

        }

        internal void AddWorldEntity(WorldEntity entity)
        {
            worldEntities.Add(entity);
            mapGrid.AddEntity(entity);
        }

        internal void RemoveWorldEntity(WorldEntity entity)
        {
            worldEntities.Remove(entity);
            mapGrid.RemoveEntity(entity);
        }

        public void SearchAreaTargets(List<Unit> targets, float radius, Vector3 center, Unit referer, TargetChecks checkType)
        {
            Collider[] hitColliders = Physics.OverlapSphere(center, radius, PhysicsManager.Mask.Characters);
            foreach (var hitCollider in hitColliders)
            {
                var targetUnit = WorldManager.UnitManager.Find(hitCollider);
                if (targetUnit == null || targetUnit.Map != this)
                    continue;

                switch (checkType)
                {
                    case TargetChecks.Ally:
                        if (referer.IsHostileTo(targetUnit))
                            continue;
                        break;
                    case TargetChecks.Enemy:
                        if (!referer.IsHostileTo(targetUnit))
                            continue;
                        break;
                }

                targets.Add(targetUnit);
            }
        }

        public TEntity FindMapEntity<TEntity>(ulong networkId) where TEntity : Entity
        {
            return worldEntities.Find(entity => entity.NetworkId == networkId) as TEntity;
        }

        public TEntity FindMapEntity<TEntity>(Predicate<TEntity> predicate) where TEntity : WorldEntity
        {
            return worldEntities.Find(entity => { TEntity target = entity as TEntity; return target != null && predicate(target); }) as TEntity;
        }
    }
}