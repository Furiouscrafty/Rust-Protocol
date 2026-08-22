using Unity.VisualScripting;
using UnityEngine;

public class EndGameScript : MonoBehaviour
{
    public GameObject player;

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject == player)
        {
            print("End Game");
            UnityEditor.EditorApplication.isPlaying = false;
        }
    }
}
