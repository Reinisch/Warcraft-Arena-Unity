using JetBrains.Annotations;
using UnityEngine;
using UnityEngine.AI;

namespace Common
{
    public class AgentMoveTo : MonoBehaviour
    {
        [SerializeField, UsedImplicitly] private Transform goal;

        private NavMeshAgent agent;

        [UsedImplicitly]
        private void Start()
        {
            agent = GetComponent<NavMeshAgent>();
            agent.destination = goal?.position ?? agent.transform.position;
        }

        [UsedImplicitly]
        private void Update()
        {
            if (agent.enabled && agent.isOnNavMesh && goal != null)
                agent.destination = goal.position;
        }
    }
}