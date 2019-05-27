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
            agent.destination = goal.position;
        }

        [UsedImplicitly]
        private void Update()
        {
            agent.destination = goal.position;
        }
    }
}