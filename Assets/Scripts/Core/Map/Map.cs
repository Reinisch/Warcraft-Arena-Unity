using System;
using System.Collections.Generic;
using Unity.Behavior;
using UnityEngine;
using Common;
using Zenject;

namespace Core
{
    public class Map
    {
        private MapGrid mapGrid;
        private readonly DiContainer container;
        private readonly MapController mapController;
        private readonly Dictionary<ulong, WorldEntity> worldEntitiesById = new();
        private readonly Collider[] raycastResults = new Collider[300];

        private MapScenarioGraph scenarioGraph;

        [Inject]
        internal World World { get; private set; }

        [Inject]
        public MapSettings Settings { get; private set; }

        public string Name => Settings.Definition.MapName;
        /// <summary>The scenario this map is running. Null on a client map loaded scene-only (no server logic).</summary>
        public ScenarioDefinition Scenario { get; private set; }
        public BlackboardReference ScenarioBlackboard => scenarioGraph.BlackboardReference;
        public float VisibilityRange => Settings.Definition.MaxVisibilityRange;

        internal Map(MapSettings mapSettings, DiContainer container, MapController mapController)
        {
            Settings = mapSettings;
            this.container = container;
            this.mapController = mapController;
            mapGrid = new MapGrid(this);
        }

        public void StartScenario()
        {
            scenarioGraph.Start();
        }

        public void StopScenario()
        {
            scenarioGraph?.End();
        }

        public void SetScenario(ScenarioDefinition scenario)
        {
            StopScenario();
            scenarioGraph?.Dispose();

            Scenario = scenario;
            scenarioGraph = CreateScenarioGraph(scenario);
            StartScenario();
        }

        private MapScenarioGraph CreateScenarioGraph(ScenarioDefinition scenario)
        {
            var graph = new MapScenarioGraph(scenario.ScenarioSettings, container);
            graph.Initialize(Settings.transform);
            return graph;
        }

        /// <summary>
        /// Rebuilds the spatial grid after the scene root has been moved into its world-layout slot,
        /// so cell bounds are computed from the map's final position rather than its authored position.
        /// </summary>
        internal void RelocateGrid()
        {
            mapGrid.Dispose();
            mapGrid = new MapGrid(this);

            foreach (WorldEntity entity in worldEntitiesById.Values)
                mapGrid.AddEntity(entity);
        }

        internal void Dispose()
        {
            StopScenario();
            scenarioGraph?.Dispose();
            mapGrid.Dispose();
        }

        internal void DoUpdate(int deltaTime)
        {
            // The spatial grid (cell relocation, proximity visibility) is server-authoritative logic. On a
            // client the units are puppets driven by replication, and running the relocator there would
            // teleport out-of-grid units to the spawn point — so only the server ticks the grid.
            if (World.HasServerLogic)
                mapGrid.DoUpdate(deltaTime);
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