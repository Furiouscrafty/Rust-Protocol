using TMPro;
using UnityEngine;

public class Money : MonoBehaviour
{
    [Header("Settings")]
    public RoundSettings roundSettings;

    public TMP_Text ScoreText;

    void Start()
    {
        if (ScoreText == null)
            ScoreText = GetComponent<TMP_Text>();
        UpdateMoneyText();
    }

    void Update()
    {
        UpdateMoneyText();
    }

    void UpdateMoneyText()
    {
        if (roundSettings != null && ScoreText != null)
        {
            ScoreText.text = $"Money: {roundSettings.money}";
        }
    }

    public int Get_Money()
    {
        if (roundSettings != null)
            return roundSettings.money;
        return 0;
    }

    public void Set_Money(int x)
    {
        if (roundSettings != null)
            roundSettings.money += x;
    }
}
