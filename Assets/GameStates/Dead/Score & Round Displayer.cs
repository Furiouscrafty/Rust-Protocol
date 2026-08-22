using TMPro;
using UnityEngine;

public class ScoreRoundDisplayer : MonoBehaviour
{
    public RoundSettings roundSettings;
    public TMP_Text ScoreText;
    public TMP_Text RoundText;
    void Start()
    {
        if (ScoreText == null)
            ScoreText = GetComponent<TMP_Text>();
        UpdateScoreText();
    }

    // Update is called once per frame
    void Update()
    {
        UpdateScoreText();
    }

    void UpdateScoreText()
    {

        if (roundSettings != null && ScoreText != null && RoundText != null)
        {
            ScoreText.text = $"Score: {roundSettings.score}";
            RoundText.text = $"Rounds Lasted: {roundSettings.CurrentRound}";
        }

    }
}
