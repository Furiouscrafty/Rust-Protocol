using UnityEngine;

public class SimpleMenuToggle : MonoBehaviour
{
    public GameObject pauseMenu;
    public GameObject settingsMenu;


    public void OpenSettings()
    {
        pauseMenu.SetActive(false);
        settingsMenu.SetActive(true);
    }

    public void BackToPause()
    {
        settingsMenu.SetActive(false);
        pauseMenu.SetActive(true);
    }
}

