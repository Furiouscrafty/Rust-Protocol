using UnityEngine;
using TMPro;

public class PauseController : MonoBehaviour
{
    // ===== INPUT CONSTANTS =====
    private const KeyCode PAUSE_KEY = KeyCode.Escape;
    private const KeyCode UNPAUSE_KEY = KeyCode.Escape;

    [Header("UI")]
    public GameObject pauseMenuUI;
    public GameObject hudUI;

    [Header("Player")]
    public PlayerMovement playerMovement;
    public GameObject Inventory;

    private bool isPaused = false;

    [Header("Score")]
    public TMP_Text ScoreText;
    public RoundSettings HighScore;

    void Start()
    {
        ResumeGame(); // Ensure correct startup state
    }

    void Update()
    {
        ScoreText.text = $"High Score: {HighScore.highscore}";
        if (!isPaused && Input.GetKeyDown(PAUSE_KEY))
        {
            PauseGame();
        }
        else if (isPaused && Input.GetKeyDown(UNPAUSE_KEY))
        {
            ResumeGame();
        }
    }

    public void PauseGame()
    {
        isPaused = true;
        Inventory.SetActive(false);

        // Stop time
        Time.timeScale = 0f;

        // Disable player control
        if (playerMovement != null)
            playerMovement.canMove = false;

        // UI
        if (pauseMenuUI != null)
            pauseMenuUI.SetActive(true);

        if (hudUI != null)
            hudUI.SetActive(false);

        // Free mouse
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
    }

    public void ResumeGame()
    {
        isPaused = false;
        Inventory.SetActive(true);

        // Resume time
        Time.timeScale = 1f;

        // Enable player control
        if (playerMovement != null)
            playerMovement.canMove = true;

        // UI
        if (pauseMenuUI != null)
            pauseMenuUI.SetActive(false);

        if (hudUI != null)
            hudUI.SetActive(true);

        // Lock mouse
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    // For UI buttons
    public void ResumeButton()
    {
        ResumeGame();
    }

    public bool IsPaused()
    {
        return isPaused;
    }
}
