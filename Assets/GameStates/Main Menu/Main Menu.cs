using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.SceneManagement;

public class MainMenu : MonoBehaviour
{
    public int SceneToLoad;
    public void PlayGame()
    { SceneManager.LoadSceneAsync(SceneToLoad); }
}
