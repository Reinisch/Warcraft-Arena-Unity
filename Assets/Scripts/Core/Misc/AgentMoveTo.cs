using UnityEngine;

namespace Core
{
    public class AgentMoveTo : MonoBehaviour
    {
        public Transform goal;
        UnityEngine.AI.NavMeshAgent agent;

        void Start()
        {
            agent = GetComponent<UnityEngine.AI.NavMeshAgent>();
            agent.destination = goal.position;
        }

        private void Update()
        {
            agent.destination = goal.position;
        }
    }
}