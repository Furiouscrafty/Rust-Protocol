using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TouchDetector : MonoBehaviour
{
    public GameObject Player;
    public Boolean RemoveHealth;

    void OnTriggerEnter(Collider other)
    {

        if (RemoveHealth)
        {
            if (other.gameObject == Player)
            {
                Debug.Log("Player touched this object!");
                print("Health is being removed");

                PlayerMovement playerScript = Player.GetComponent<PlayerMovement>();

                if (playerScript != null)
                {
                    playerScript.Remove_Health(20);
                }
            }
        }
        else {
            if (other.gameObject == Player)
            {
                Debug.Log("Player touched this object!");
                print("Health is being removed");

                PlayerMovement playerScript = Player.GetComponent<PlayerMovement>();

                if (playerScript != null)
                {
                    playerScript.Add_Health(20);
                }
            }
        }
    }
}
