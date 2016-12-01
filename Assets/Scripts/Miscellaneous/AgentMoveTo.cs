// MoveTo.cs
using UnityEngine;
using System.Collections;

public class AgentMoveTo : MonoBehaviour
{
    public Transform goal;
    NavMeshAgent agent;

    void Start()
    {
        agent = GetComponent<NavMeshAgent>();
        agent.destination = goal.position;
    }

    private void Update()
    {
        agent.destination = goal.position;
    }
}