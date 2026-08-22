using TMPro;
using UnityEngine;

public class ScoreManager : MonoBehaviour
{
    public GameObject player; // Where the score variable is
    public TMP_Text ScoreText; // Where the score is displayed
    public RoundSettings score; // Allows script to access the score

    void Start()
    {
        PlayerMovement playerScript = player.GetComponent<PlayerMovement>();

        if (ScoreText == null)
            ScoreText = GetComponent<TMP_Text>();

        UpdateScoreText(playerScript);
    }

    // Update is called once per frame
    void Update()
    {
        PlayerMovement playerScript = player.GetComponent<PlayerMovement>();
        score.score = playerScript.Score; // Makes the Score attribute in roundSettings equal to the score variable in the playerScript
        UpdateScoreText(playerScript);
    }

    void UpdateScoreText(PlayerMovement Score_) // Updates the Score UI
    {
        ScoreText.text = $"Score: {Score_.Score}";
    }
}
