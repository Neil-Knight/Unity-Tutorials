using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

[RequireComponent(typeof(NavMeshAgent))]
public class AIControl : MonoBehaviour
{
    public Path waypointNetwork;
    public int currentIndex = 0;
    public bool hasPath = false;
    public bool pathPending = false;
    public bool pathStale = false;
    public bool pathState = false;
    public NavMeshPathStatus pathStatus = NavMeshPathStatus.PathInvalid;

    private NavMeshAgent agent;

    private void Start()
    {
        agent = GetComponent<NavMeshAgent>();
        if (waypointNetwork == null)
            return;

        SetNextDestination(false);
    }

    void SetNextDestination(bool increment)
    {
        if (!waypointNetwork)
            return;

        int incrementStep = increment ? 1 : 0;
        Transform nextWaypointTransform;

        int nextWaypoint = (currentIndex + incrementStep >= waypointNetwork.nodes.Count) ? 0 : currentIndex + incrementStep;
        nextWaypointTransform = waypointNetwork.nodes[nextWaypoint];

        if (nextWaypointTransform != null)
        {
            currentIndex = nextWaypoint;
            agent.destination = nextWaypointTransform.position;
            return;
        }

        currentIndex = nextWaypoint;
    }

    private void Update()
    {
        hasPath = agent.hasPath;
        pathPending = agent.pathPending;
        pathStale = agent.isPathStale;
        pathStatus = agent.pathStatus;

        if ((agent.remainingDistance <= agent.stoppingDistance && !pathPending) || pathStatus == NavMeshPathStatus.PathInvalid)
        {
            SetNextDestination(true);
        }
        else if (agent.isPathStale)
            SetNextDestination(false);
    }
}
