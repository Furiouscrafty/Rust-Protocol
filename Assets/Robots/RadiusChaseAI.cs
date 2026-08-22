using UnityEngine;
using UnityEngine.AI;

public class RadiusChaseAI : MonoBehaviour
{
    [Header("References")]
    public NavMeshAgent agent;
    public Transform player;

    [Header("Radius Settings")]
    public float detectionRadius = 15f;
    public float stopRadius = 3f;

    void Update()
    {
        if (player == null || agent == null)
            return;

        float distanceToPlayer = Vector3.Distance(transform.position, player.position);

        // Player is close enough to stop
        if (distanceToPlayer <= stopRadius)
        {
            print("Inside");
            agent.isStopped = true;
        }
        // Player is within detection radius -> move towards player
        else if (distanceToPlayer <= detectionRadius)
        {
            print("Inside");
            agent.isStopped = false;
            agent.SetDestination(player.position);
        }
        // Player is outside both radii -> stop
        else
        {
            agent.isStopped = true;
        }
    }

    // Optional: Visualize radii in editor
    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, detectionRadius);

        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, stopRadius);
    }
}
