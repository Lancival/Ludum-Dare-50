using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Platform : MonoBehaviour
{
    private Rigidbody2D rb;
    private EdgeCollider2D ec;

    private CustomOptionView option;
    
    void Awake()
    {
        ec = GetComponent<EdgeCollider2D>();
        rb = GetComponent<Rigidbody2D>();
        option = GetComponent<CustomOptionView>();
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        option.InvokeOption();        
    }
}
