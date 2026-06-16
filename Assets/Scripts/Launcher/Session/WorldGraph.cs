using Client;
using Unity.Behavior;
using UnityEngine;
using Zenject;

namespace Assets.Scripts.Workflow
{
    /// <summary>
    /// Runs an application-level Unity Behavior graph at the World Context that drives the top-level logic:
    /// For client just shows BattleScreen or LobbyScreen
    /// </summary>
    public sealed class WorldGraph : MonoBehaviour
    {
        [SerializeField] private BehaviorGraphAgent graphPrefab;
        [Inject] private DiContainer container;

        private BehaviorGraphAgent agent;

        private void Start()
        {
            if (graphPrefab == null)
                return;

            agent = container.InstantiatePrefab(graphPrefab, transform).GetComponent<BehaviorGraphAgent>();
            agent.Init();
            agent.Start();
        }

        private void OnDestroy()
        {
            if (agent == null)
                return;

            agent.End();
            agent = null;
        }
    }
}
