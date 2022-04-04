using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Platform : MonoBehaviour
{
    private Rigidbody2D rb;
    private EdgeCollider2D ec;

    private AudioSource audioSource;
    [SerializeField] private AudioClip ac;
    private CustomOptionView option;

    bool hasPlayed = false;
    
    void Awake()
    {
        ec = GetComponent<EdgeCollider2D>();
        rb = GetComponent<Rigidbody2D>();
        option = GetComponent<CustomOptionView>();
        audioSource = GetComponent<AudioSource>();
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        option.InvokeOption();        
    }

    public void PlayDiscordNotificationSound()
    {
        if (!hasPlayed)
        {
            audioSource.PlayOneShot(ac, 0.4f);
            hasPlayed = !hasPlayed;
        }
    }
}
