using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using Common;
using Object = UnityEngine.Object;
using Core.Scenario;

namespace Core
{
    public sealed class Map
    {
        private readonly MapGrid mapGrid;
        private readonly Dictionary<ulong, WorldEntity> worldEntitiesById = new Dictionary<ulong, WorldEntity>();
        private readonly Collider[] raycastResults = new Collider[300];

        private MapScenario scenario;

        internal World World { get; }

        public MapSettings Settings { get; }
        public float VisibilityRange => Settings.Definition.MaxVisibilityRange;

        internal Map(World world, int scenarioId, Scene mapScene)
        {
            World = world;

            foreach (var rootObject in mapScene.GetRootGameObjects())
            {
                Settings = rootObject.GetComponentInChildren<MapSettings>();

                if (Settings != null)
                    break;
            }

            Assert.IsNotNull(Settings, $"Map settings are missing in map: {mapScene.name}");
            mapGrid = new MapGrid(this);

            if (World.HasServerLogic)
            {
                scenario = Settings.CreateScenario(scenarioId, Settings.transform);
                scenario.Initialize(this);
            }
        }

        internal void Dispose()
        {
            if (World.HasServerLogic)
            {
                scenario.DeInitialize();
                Object.Destroy(scenario);
                scenario = null;
            }

            mapGrid.Dispose();
        }

        internal void DoUpdate(int deltaTime)
        {
            mapGrid.DoUpdate(deltaTime);

            if (World.HasServerLogic)
            {
                scenario.DoUpdate(deltaTime);
            }
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
                if (!World.UnitManager.TryFind(raycastResults[i], out Unit targetUnit) || targetUnit.Map != this)
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

        public void UpdateVisibilityFor(Player player) => mapGrid.UpdateVisibility(player, true);

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