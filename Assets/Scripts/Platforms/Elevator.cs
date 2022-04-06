using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Elevator : MonoBehaviour
{
    public Transform startPosition;
    public Transform endPosition;
    public float moveSpeed = 10f;

    private bool startElevator = false;

    void Update()
    {
        if (startElevator)
            MoveToEndPoint();
        else
        {
            MoveToStartPoint();
        }
    }

    private void MoveToEndPoint()
    {
        this.transform.position = Vector2.MoveTowards(this.transform.position, endPosition.position, Time.deltaTime * moveSpeed);
    }

     private void MoveToStartPoint()
    {
        this.transform.position = Vector2.MoveTowards(this.transform.position, startPosition.position, Time.deltaTime * moveSpeed);
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        startElevator = true;
        Debug.Log("Starting elevator!");    
    }

    void OnTriggerExit2D(Collider2D other)
    {
        startElevator = false;
        Debug.Log("Elevator moving down!");
    }
}
