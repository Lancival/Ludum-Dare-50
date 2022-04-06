using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Respawn : MonoBehaviour
{
    private GameObject[] respawnPoints;

    private GameObject currentRespawnPoint;
    private Dictionary<GameObject, bool> dict;
    void Awake()
    {
        dict = new Dictionary<GameObject, bool>();
        respawnPoints = GameObject.FindGameObjectsWithTag("Checkpoint");
        if (respawnPoints == null)
        {
            Debug.LogWarning("No respawn checkpoints assigned");
        }

        // Register object in dictionary to see if we've encountered it before
        foreach (GameObject obj in respawnPoints)
        {
            dict[obj] = false;
        }
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.tag == "Respawn")
        {
            // Reset transform to the closest distance checkpoint
            this.gameObject.transform.position = currentRespawnPoint.transform.position;
        }
        if (other.tag == "Checkpoint")
        {   
            // if we haven't seen it before
            if (!dict[other.gameObject])
            {
                dict[other.gameObject] = true;
                currentRespawnPoint = other.gameObject;
            }
        }    
    }
}
