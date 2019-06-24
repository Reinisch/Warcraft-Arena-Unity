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
        private readonly Dictionary<ulong, WorldEntity> worldEntitiesById = new Dictionary<ulong, WorldEntity>();
        private readonly Collider[] raycastResults = new Collider[200];

        internal WorldManager WorldManager { get; private set; }

        public MapSettings Settings { get; private set; }

        internal void Initialize(WorldManager worldManager, Scene mapScene)
        {
            WorldManager = worldManager;

            foreach (var rootObject in mapScene.GetRootGameObjects())
            {
                Settings = rootObject.GetComponentInChildren<MapSettings>();

                if (Settings != null)
                    break;
            }

            Assert.IsNotNull(Settings, $"Map settings are missing in map: {mapScene.name}");
            mapGrid.Initialize(this);

            if(worldManager.HasServerLogic)
                foreach (var scenarioAction in Settings.ScenarioActions)
                    scenarioAction.Initialize(this);
        }

        internal void Deinitialize()
        {
            if (WorldManager.HasServerLogic)
                foreach (var scenarioAction in Settings.ScenarioActions)
                    scenarioAction.DeInitialize();

            mapGrid.Deinitialize();

            Settings = null;
            WorldManager = null;
        }

        internal void DoUpdate(int deltaTime)
        {
            mapGrid.DoUpdate(deltaTime);

            if (WorldManager.HasServerLogic)
                foreach (var scenarioAction in Settings.ScenarioActions)
                    scenarioAction.DoUpdate(deltaTime);
        }

        internal void AddWorldEntity(WorldEntity entity)
        {
            worldEntitiesById.Add(entity.Id, entity);
            mapGrid.AddEntity(entity);
        }

        internal void RemoveWorldEntity(WorldEntity entity)
        {
            worldEntitiesById.Remove(entity.Id);
            mapGrid.RemoveEntity(entity);
        }

        public void VisitInRadius(WorldEntity referer, float radius, IUnitVisitor unitVisitor)
        {
            mapGrid.VisitInRadius(referer, radius, unitVisitor);
        }

        public void SearchAreaTargets(List<Unit> targets, float radius, Vector3 center, Unit referer, SpellTargetChecks checkType)
        {
            int hitCount = Physics.OverlapSphereNonAlloc(center, radius, raycastResults, PhysicsReference.Mask.Characters);
            Assert.IsFalse(hitCount == raycastResults.Length, "Raycast results reached maximum!");
            for (int i = 0; i < hitCount; i++)
            {
                var targetUnit = WorldManager.UnitManager.Find(raycastResults[i]);
                if (targetUnit == null || targetUnit.Map != this)
                    continue;

                switch (checkType)
                {
                    case SpellTargetChecks.Ally:
                        if (referer.IsHostileTo(targetUnit))
                            continue;
                        break;
                    case SpellTargetChecks.Enemy:
                        if (!referer.IsHostileTo(targetUnit))
                            continue;
                        break;
                }

                targets.Add(targetUnit);
            }
        }

        public TEntity FindMapEntity<TEntity>(ulong networkId) where TEntity : Entity
        {
            return worldEntitiesById.LookupEntry(networkId) as TEntity;
        }

        public TEntity FindMapEntity<TEntity>(ulong networkId, Predicate<TEntity> predicate) where TEntity : WorldEntity
        {
            TEntity targetEntity = FindMapEntity<TEntity>(networkId);
            return targetEntity != null && predicate(targetEntity) ? targetEntity : null;
        }
    }
}