using UnityEngine;

public class Controler : MonoBehaviour
{
    public RoundSettings RoundSettings;
    public GameObject Aigis1;
    public GameObject Aigis2;
    public GameObject Crawler;

    private void Start()
    {
        Aigis1.SetActive(false);
        Aigis2.SetActive(false);
        Crawler.SetActive(false);
    }

    void Update()
    {
        if (RoundSettings.CurrentRound >= 5)
        {
            Aigis1.SetActive(true);
            Aigis2.SetActive(true);
            Crawler.SetActive(true);
        }
    }
}
