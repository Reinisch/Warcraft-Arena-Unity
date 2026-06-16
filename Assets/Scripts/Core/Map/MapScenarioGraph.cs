using Unity.Behavior;
using UnityEngine;
using Zenject;

namespace Core
{
    public sealed class MapScenarioGraph
    {
        private readonly MapScenarioGraphSettings settings;
        private readonly DiContainer container;

        private BehaviorGraphAgent agent;

        public BlackboardReference BlackboardReference => agent.BlackboardReference;

        public MapScenarioGraph(MapScenarioGraphSettings settings, DiContainer container)
        {
            this.settings = settings;
            this.container = container;
        }

        public void Initialize(Transform parent)
        {
            agent = container.InstantiatePrefab(settings.GraphPrefab, parent)
                .GetComponent<BehaviorGraphAgent>();

            agent.Init();
        }

        public void Start() => agent.Start();

        public void End() => agent.End();

        public void Dispose()
        {
            if (agent == null)
                return;

            agent.End();
            
            Object.Destroy(agent.gameObject);
            agent = null;
        }
    }
}
