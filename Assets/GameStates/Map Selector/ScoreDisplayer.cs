using UnityEngine;
using TMPro;

public class ScoreDisplayer : MonoBehaviour
{
    public RoundSettings roundSettings;
    public TMP_Text ScoreText;
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

            if (roundSettings != null && ScoreText != null)
            {
                ScoreText.text = $"High Score: {roundSettings.highscore}";
            }

    }
}
