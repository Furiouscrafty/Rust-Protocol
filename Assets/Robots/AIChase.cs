using UnityEngine;
using UnityEngine.AI;

public class AIChase : MonoBehaviour
{
    [Header("Player Detection")]
    public Transform player;
    public GameObject PLAY;
    public float chaseRange = 10f; //Distance at which the robot will move towards the player
    public float shootRange = 5f; //Distance whereby the robot will shoot the player
    public float stoppingDistance = 3f; // Distance where AI stops moving but keeps shooting

    [Header("Patrol Settings")]
    public float patrolRadius = 15f;
    public float patrolWaitTime = 3f;
    public float walkSpeed = 3.5f;
    public float maxPatrolTravelTime = 4f;

    [Header("Components")]
    public NavMeshAgent agent;
    public LineRenderer laserLine;

    [Header("Laser Settings")]
    public float laserWidth = 0.1f;
    public Color laserColor = Color.red;
    public float laserRange = 50f;
    public LayerMask laserHitLayers;
    public float laserDamage;

    [Header("Laser Timing")]
    public float laserChargeTime = 3f;
    public float laserFireDuration = 5f;

    private enum AIState { Patrolling, Chasing, Shooting, ShootingStationary } // The current actions of the AI
    private AIState currentState = AIState.Patrolling;

    private Vector3 patrolDestination;
    private float patrolTimer;
    private float patrolTravelTimer;
    private bool hasPatrolDestination;

    private float nextStateCheck;
    private const float STATE_CHECK_INTERVAL = 0.1f;

    // Laser timers
    private float laserChargeTimer;
    private float laserFireTimer;
    private bool laserCharged;

    void Start()
    {
        agent.speed = walkSpeed;
        SetupLaser();
    }

    void Update()
    {
        if (Time.time >= nextStateCheck)
        {
            nextStateCheck = Time.time + STATE_CHECK_INTERVAL;
            UpdateAIState();
        }

        ExecuteCurrentState();
    }

    void UpdateAIState()
    {
        float distance = Vector3.Distance(transform.position, player.position);

        // Three-tier distance system
        if (distance <= stoppingDistance)
        {
            // Close enough - stop moving and shoot
            currentState = AIState.ShootingStationary;
        }
        else if (distance <= shootRange)
        {
            // Within shoot range but not at stopping distance - chase while shooting
            currentState = AIState.Shooting;
        }
        else if (distance <= chaseRange)
        {
            // Within chase range but too far to shoot - just chase
            currentState = AIState.Chasing;
        }
        else
        {
            // Out of range - patrol
            currentState = AIState.Patrolling;
        }
    }

    void ExecuteCurrentState()
    {
        switch (currentState)
        {
            case AIState.ShootingStationary: // Stationary shooting
                ShootLaserStationary();
                break;

            case AIState.Shooting: // Chase while shooting
                ShootLaserWhileChasing();
                break;

            case AIState.Chasing:
                ResetLaser();
                ChasePlayer();
                break;

            case AIState.Patrolling:
                ResetLaser();
                Patrol();
                break;
        }
    }

    // Shoot while stationary (original shooting behavior)
    void ShootLaserStationary()
    {
        agent.isStopped = true;

        // CHARGING
        if (!laserCharged)
        {
            laserLine.enabled = false;
            laserChargeTimer += Time.deltaTime;

            if (laserChargeTimer >= laserChargeTime)
            {
                laserCharged = true;
                laserChargeTimer = 0f;
                laserFireTimer = 0f;
            }

            FacePlayer();
            return;
        }

        // FIRING
        FireLaser();

        // STOP AFTER DURATION
        if (laserFireTimer >= laserFireDuration)
        {
            ResetLaser();
        }
    }

    // Shoot while chasing
    void ShootLaserWhileChasing()
    {
        agent.isStopped = false;
        agent.SetDestination(player.position);

        // CHARGING
        if (!laserCharged)
        {
            laserLine.enabled = false;
            laserChargeTimer += Time.deltaTime;

            if (laserChargeTimer >= laserChargeTime)
            {
                laserCharged = true;
                laserChargeTimer = 0f;
                laserFireTimer = 0f;
            }

            FacePlayer();
            return;
        }

        // FIRING
        FireLaser();

        // STOP AFTER DURATION
        if (laserFireTimer >= laserFireDuration)
        {
            ResetLaser();
        }
    }

    // Consolidated laser firing logic
    void FireLaser()
    {
        laserLine.enabled = true;
        laserFireTimer += Time.deltaTime;

        Vector3 start = transform.position;
        Vector3 direction = (player.position - start).normalized;

        laserLine.SetPosition(0, start);

        if (Physics.Raycast(start, direction, out RaycastHit hit, laserRange, laserHitLayers))
        {
            laserLine.SetPosition(1, hit.point);

            if (hit.transform == player)
            {
                print("Player is hit");
                PlayerMovement pm = PLAY.GetComponent<PlayerMovement>();
                pm.Remove_Health(laserDamage);
            }
        }
        else
        {
            laserLine.SetPosition(1, start + direction * laserRange);
        }

        FacePlayer();
    }

    void ResetLaser()
    {
        laserCharged = false;
        laserChargeTimer = 0f;
        laserFireTimer = 0f;

        if (laserLine)
            laserLine.enabled = false;
    }

    // CHASE
    void ChasePlayer()
    {
        agent.isStopped = false;
        agent.SetDestination(player.position);
        FacePlayer();

        hasPatrolDestination = false;
        patrolTimer = 0f;
        patrolTravelTimer = 0f;
    }

    // PATROL
    void Patrol()
    {
        if (!hasPatrolDestination)
        {
            patrolTimer += Time.deltaTime;

            if (patrolTimer >= patrolWaitTime)
                StartNewPatrol();
            else
                agent.isStopped = true;

            return;
        }

        agent.isStopped = false;
        agent.SetDestination(patrolDestination);
        patrolTravelTimer += Time.deltaTime;

        if (HasReachedDestination())
        {
            hasPatrolDestination = false;
            patrolTimer = 0f;
            patrolTravelTimer = 0f;
            agent.isStopped = true;
        }
        else if (patrolTravelTimer >= maxPatrolTravelTime)
        {
            ChooseNewPatrolPoint();
            patrolTravelTimer = 0f;
            agent.SetDestination(patrolDestination);
        }
    }

    void StartNewPatrol()
    {
        ChooseNewPatrolPoint();
        patrolTimer = 0f;
        patrolTravelTimer = 0f;
        hasPatrolDestination = true;

        agent.isStopped = false;
        agent.SetDestination(patrolDestination);
    }

    void ChooseNewPatrolPoint()
    {
        for (int i = 0; i < 10; i++)
        {
            Vector3 random = Random.insideUnitSphere * patrolRadius;
            random += transform.position;
            random.y = transform.position.y;

            if (NavMesh.SamplePosition(random, out NavMeshHit hit, patrolRadius, NavMesh.AllAreas))
            {
                patrolDestination = hit.position;
                return;
            }
        }

        patrolDestination = transform.position;
    }

    bool HasReachedDestination()
    {
        return !agent.pathPending &&
               agent.remainingDistance <= agent.stoppingDistance &&
               (!agent.hasPath || agent.velocity.sqrMagnitude < 0.1f);
    }

    // UTILITIES
    void FacePlayer()
    {
        Vector3 dir = player.position - transform.position;
        dir.y = 0f;

        if (dir != Vector3.zero)
            transform.rotation = Quaternion.LookRotation(dir);
    }

    void SetupLaser()
    {
        if (!laserLine)
            laserLine = gameObject.AddComponent<LineRenderer>();

        laserLine.startWidth = laserWidth;
        laserLine.endWidth = laserWidth;
        laserLine.material = new Material(Shader.Find("Sprites/Default"));
        laserLine.startColor = laserColor;
        laserLine.endColor = laserColor;
        laserLine.enabled = false;
    }

    void OnDrawGizmosSelected()
    {
        if (player == null) return;

        // CHASE RANGE (Yellow)
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, chaseRange);

        // SHOOT RANGE (Red)
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, shootRange);

        // STOPPING DISTANCE (Blue)
        Gizmos.color = Color.blue;
        Gizmos.DrawWireSphere(transform.position, stoppingDistance);

        // PATROL RADIUS (Green)
        Gizmos.color = Color.green;
        Gizmos.DrawWireSphere(transform.position, patrolRadius);
    }

}