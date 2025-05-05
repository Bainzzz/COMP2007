using UnityEngine;
using UnityEngine.AI;
using UnityEngine.SceneManagement;

public class AIMonster : MonoBehaviour
{
    private NavMeshAgent agent;
    private Transform player;
    private Transform playerCapsule;
    private enum State { Wandering, Chasing }
    private State currentState;

    //wandering setting 
    [SerializeField] private float wanderRadius = 200f; //radius to pick random wander points
    [SerializeField] private float minWanderInterval = 20f; //min time before new destination
    [SerializeField] private float maxWanderInterval = 40f; //max time before new destination
    [SerializeField] private float minDistanceToTarget = 1f; //distance to consider destination reached
    //chasing settings
    [SerializeField] private float detectionRange = 10f; //range to detect player
    [SerializeField] private float chaseSpeed = 5f; //speed when chasing
    [SerializeField] private float wanderSpeed = 3f; //speed when wandering
    [SerializeField] private float losePlayerTime = 5f; //time before giving up chase
    [SerializeField] private float attackDistance = 1f; //distance to cacth player 
    [SerializeField] private LayerMask detectionLayerMask; //layers to include in raycast 
    [SerializeField] private AudioClip spotPlayerClip; // Sound to play when spotting player
    [SerializeField] private GameOverUI gameOverUI; // Reference to GameOverUI script
    private AudioSource audioSource; // AudioSource for spotting sound

    private float timer; //tracks time since last wander destination
    private float wanderInterval; //time until next wander destination
    private float losePlayerTimer; //time since player was last seen

    void Start()
    {
        agent = GetComponent<NavMeshAgent>();
        agent.stoppingDistance = minDistanceToTarget * 0.5f;
        player = GameObject.FindGameObjectWithTag("Player")?.transform;
        if (player != null)
        {
            playerCapsule = player.Find("Capsule");
        }
        currentState = State.Wandering;         //initialize to wandering state
        agent.speed = wanderSpeed;  //set initial speed for wandering
        SetNewDestination();
        // Initialize AudioSource for spotting sound
        audioSource = gameObject.AddComponent<AudioSource>();
        audioSource.spatialBlend = 1f; // 3D audio
        audioSource.playOnAwake = false;
        audioSource.loop = false;
    }

    #region State Management
    void Update()     //called every frame to manage Monster's behavior
    {
        switch (currentState)
        {
            case State.Wandering:                //handle wandering movement
                HandleWandering();
                if (CanSeePlayer())     //check if player is visible, switch to chasing state
                {
                    currentState = State.Chasing;
                    agent.speed = chaseSpeed;
                    losePlayerTimer = 0f;
                    // Play spotting sound
                    if (spotPlayerClip != null && audioSource != null)
                    {
                        audioSource.PlayOneShot(spotPlayerClip);
                    }
                }
                break;

            case State.Chasing:         //handle chasing movement
                HandleChasing();
                if (!CanSeePlayer())     //check if player is still visible
                {
                    losePlayerTimer += Time.deltaTime;  //increment timer since player was last seen, if player is lost for too long, return to wandering
                    if (losePlayerTimer >= losePlayerTime)
                    {
                        currentState = State.Wandering;
                        agent.speed = wanderSpeed;
                        SetNewDestination();
                    }
                }
                else
                {
                    //reset timer if player is still visible
                    losePlayerTimer = 0f;
                }
                break;
        }
    }
    #endregion

    #region Wandering
    private void HandleWandering()
    {
        timer += Time.deltaTime;   //increment timer for wander interval
        if (agent.remainingDistance <= minDistanceToTarget || timer >= wanderInterval)     //if timer expires or Monster reaches target, pick new destination
        {
            SetNewDestination();
        }
    }

    private void SetNewDestination()  //sets a new random destination and resets timer
    {
        Vector3 newPos = RandomNavSphere(transform.position, wanderRadius, -1);   //find a random point on the NavMesh
        agent.SetDestination(newPos);
        timer = 0;
        wanderInterval = Random.Range(minWanderInterval, maxWanderInterval);    //set a new random interval
    }
    #endregion

    #region Chasing
    private void HandleChasing()
    {
        agent.SetDestination(playerCapsule != null ? playerCapsule.position : player.position);   //set destination to Capsule's position, or player's if Capsule is null

        //check if Monster is close enough to hit player
        if (playerCapsule != null)
        {
            float distanceToPlayer = Vector3.Distance(transform.position, playerCapsule.position);
            if (distanceToPlayer <= attackDistance)
            {
                LoseGame();
            }
        }
    }
    #endregion

    #region Player Detection
    private bool CanSeePlayer()    //checks if the player's capsule is within range and line of sight
    {
        if (player == null || playerCapsule == null) return false;  //return false if player or capsule is not found

        Vector3 directionToPlayer = playerCapsule.position - transform.position;  // calculate direction and distance to Capsule
        float distanceToPlayer = directionToPlayer.magnitude;
        if (distanceToPlayer > detectionRange) //return false if Capsule is too far
        {
            return false;
        }

        RaycastHit hit;  //perform raycast to check line of sight
        if (Physics.Raycast(transform.position, directionToPlayer.normalized, out hit, detectionRange, detectionLayerMask))
        {
            if (hit.transform == playerCapsule || hit.transform == player || hit.transform.IsChildOf(player))      //return true if hit is Capsule, player, or a child of player
            {
                return true;
            }
        }
        return false;
    }
    #endregion

    #region Lose Condition
    // Handles the lose condition when Monster hits player
    private void LoseGame()
    {
        // Show the Game Over UI
        if (gameOverUI != null)
        {
            gameOverUI.ShowGameOver();
        }
    }
    #endregion

    private Vector3 RandomNavSphere(Vector3 origin, float distance, int layermask)
    {
        Vector3 randomDirection = Random.insideUnitSphere * distance;
        randomDirection += origin;

        NavMeshHit navHit;
        NavMesh.SamplePosition(randomDirection, out navHit, distance, layermask);

        return navHit.position;
    }
}