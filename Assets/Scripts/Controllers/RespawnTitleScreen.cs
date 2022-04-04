using UnityEngine;

public class RespawnTitleScreen : MonoBehaviour
{

    private Vector3 initialPosition;

    void Awake()
    {
        initialPosition = transform.position;
    }

    void OnTriggerEnter2D(Collider2D collider)
    {
        if (collider.gameObject.name == "Respawn")
        {
            transform.position = initialPosition;
        }
    }
}
