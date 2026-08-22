using UnityEngine;
using TMPro;


public class SimpleVendingMachine : MonoBehaviour
{
    public GameObject player;
    public float interactRange = 3f;

    public TMP_Text promptText;
    public KeyCode interactKey = KeyCode.E;

    public int healAmount = 20;
    public int cost = 100;
    public RoundSettings roundSettings;
    public bool healthflag; // Flag for health

    private Transform playerTransform;

    void Start()
    {
        if (player == null)
            player = GameObject.FindGameObjectWithTag("Player");

        if (player != null)
            playerTransform = player.transform;

        if (promptText != null)
            promptText.gameObject.SetActive(false);
    }

    void Update()
    {
        PlayerMovement pm = player.GetComponent<PlayerMovement>(); // Copied from TryBuyHealth
        // Health validation for buying health
        if (pm != null)
            if (pm.CurrentHealth < pm.InitialHealth)
            { healthflag = true; }
            else
            {
                healthflag = false;
            }

        if (playerTransform == null) return;

        float distance = Vector3.Distance(transform.position, playerTransform.position);
        bool inRange = distance <= interactRange;

        // Show or hide prompt
        if (promptText != null)
            promptText.gameObject.SetActive(inRange);

        if (inRange)
        {

            // Update text
            if (roundSettings.money >= cost && healthflag)
                promptText.text = $"Press E to Buy Health (${cost})";
            else
                promptText.text = $"Not Enough Money (${cost})";

            // Interaction
            if (Input.GetKeyDown(interactKey) && healthflag)
                TryBuyHealth();
        }
    }

    void TryBuyHealth()
    {
        if (roundSettings.money < cost)
        {
            Debug.Log("Not enough money");
            return;
        }

        roundSettings.money -= cost;

        PlayerMovement pm = player.GetComponent<PlayerMovement>();
        if (pm != null)
        {
            pm.Add_Health(healAmount);
            Debug.Log($"Healed {healAmount}. Money left: {roundSettings.money}");
        }
    }
}
