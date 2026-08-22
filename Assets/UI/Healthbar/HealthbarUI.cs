using UnityEngine;
using TMPro;

public class HealthTextUI : MonoBehaviour
{
    public GameObject player;
    public TMP_Text healthText;

    void Start()
    {
        PlayerMovement playerScript = player.GetComponent<PlayerMovement>();

        if (healthText == null)
            healthText = GetComponent<TMP_Text>();

        UpdateHealthText(playerScript);
    }

    void Update()
    {
        PlayerMovement playerScript = player.GetComponent<PlayerMovement>();
        UpdateHealthText(playerScript);
    }

    void UpdateHealthText(PlayerMovement HP)
    {
        healthText.text = $"Health: {Mathf.CeilToInt(HP.CurrentHealth)} / {Mathf.CeilToInt(HP.InitialHealth)}";
    }
}
