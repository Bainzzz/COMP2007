using UnityEngine;
using UnityEngine.AI;

public class AIWander : MonoBehaviour
{
    private NavMeshAgent agent;
    [SerializeField] private float wanderRadius = 200f; //radius to pick random points
    [SerializeField] private float minWanderInterval = 20f; //min time before new destination
    [SerializeField] private float maxWanderInterval = 40f; //max time before new destination
    [SerializeField] private float minDistanceToTarget = 1f; //distance to consider destination reached
    private float timer;
    private float wanderInterval;

    void Start()
    {
        agent = GetComponent<NavMeshAgent>();
        agent.stoppingDistance = minDistanceToTarget * 0.5f; //makes it approach nicer 
        SetNewDestination(); //start with an initial destination
    }

    #region update loctaion
    void Update()
    {
        timer += Time.deltaTime;

        if (agent.remainingDistance <= minDistanceToTarget || timer >= wanderInterval)
        {
            SetNewDestination(); //if timer expires or reach target then pick new target 
        }
    }

    private void SetNewDestination() //set new destination and reset timer
    {
        Vector3 newPos = RandomNavSphere(transform.position, wanderRadius, -1);
        agent.SetDestination(newPos);
        timer = 0;
        wanderInterval = Random.Range(minWanderInterval, maxWanderInterval); 
    }
    #endregion

    #region find point on navmesh 
    private Vector3 RandomNavSphere(Vector3 origin, float distance, int layermask) //find a random point on the NavMesh within the specified radius
    {
        Vector3 randomDirection = Random.insideUnitSphere * distance;
        randomDirection += origin;

        NavMeshHit navHit;
        NavMesh.SamplePosition(randomDirection, out navHit, distance, layermask);

        return navHit.position;
    }
    #endregion
}
