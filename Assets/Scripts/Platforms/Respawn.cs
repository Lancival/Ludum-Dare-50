using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Respawn : MonoBehaviour
{
    private GameObject[] respawnPoints;
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
            float closestDistance = float.MaxValue;
            Vector3 closestPosition = new Vector3(0,0,0);
            foreach (GameObject obj in respawnPoints)
            {
                if (dict[obj])
                {
                    float distance = Vector3.Distance(obj.transform.position, this.gameObject.transform.position);
                    if (distance < closestDistance)
                    {
                        closestDistance = distance;
                        closestPosition = obj.transform.position;
                    }
                }
            }

            // Reset transform to the closest distance checkpoint
            this.gameObject.transform.position = closestPosition;
        }
        if (other.tag == "Checkpoint")
        {
            dict[other.gameObject] = true;
        }    
    }
}
