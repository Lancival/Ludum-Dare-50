using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Slide : MonoBehaviour
{
    public bool slideEnter;

    private void OnTriggerEnter2D(Collider2D other) {
        Rigidbody2D rb =  other.gameObject.GetComponent<Rigidbody2D>();
        if (slideEnter)
        {
            rb.freezeRotation = false;
        }
        else 
        {
            rb.transform.rotation = Quaternion.identity;
            rb.freezeRotation = true;
        }
    }         
}
