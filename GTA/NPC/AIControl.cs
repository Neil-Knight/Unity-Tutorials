using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class AIControl : MonoBehaviour
{
    public GameObject[] goalLocations;
    public Animator animator;
    public NavMeshAgent agent;

    // Start is called before the first frame update
    void Start()
    {
        agent = this.GetComponent<NavMeshAgent>();
        animator = this.GetComponent<Animator>();
        goalLocations = GameObject.FindGameObjectsWithTag("NPC Goal");
        AssignSteeringSpeed();
        agent.SetDestination(goalLocations[Random.Range(0, goalLocations.Length)].transform.position);
        animator.SetFloat("walkingOffset", Random.Range(0, 1));
        animator.SetTrigger("isWalking");
    }

    public void AssignSteeringSpeed()
    {
        float speedMultiplier = Random.Range(1, 1.5f);
        animator.SetFloat("speedMultiplier", speedMultiplier);
        agent.speed *= speedMultiplier;
    }

    // Update is called once per frame
    void Update()
    {
        if (agent.remainingDistance < 1) // Close to the destination
            agent.SetDestination(goalLocations[Random.Range(0, goalLocations.Length)].transform.position);
    }
}
