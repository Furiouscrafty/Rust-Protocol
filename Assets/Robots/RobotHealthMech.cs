using UnityEngine;

public class RobotHealthMech : MonoBehaviour
{
    [Header("Health")]
    public float maxHealth = 100f;
    private float currentHealth;

    [Header("Score")]
    public float pointsUponDeath = 100f;

    private PlayerMovement player;
    private bool isDead = false;

    void Awake()
    {
        currentHealth = maxHealth;

        // Auto-find player (works for spawned enemies)
        GameObject playerObj = GameObject.FindGameObjectWithTag("Player");
        if (playerObj != null)
        {
            player = playerObj.GetComponent<PlayerMovement>();
        }
    }

    public void RemoveHealth(float damage)
    {
        if (isDead) return;

        currentHealth -= damage;

        if (currentHealth <= 0f)
        {
            Die();
        }
    }

    void Die()
    {
        isDead = true;

        if (player != null)
        {
            player.Add_Score(pointsUponDeath);
        }

        Destroy(gameObject);
    }
}
