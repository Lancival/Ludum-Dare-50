using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;
using Yarn.Unity;

[RequireComponent(typeof(AudioSource))]

public class IconManager : MonoBehaviour
{

    private List<GameObject> rings;
    private List<string> names;
    private GameObject julia;

    [SerializeField] private AudioClip discordJoin;
    [SerializeField] private AudioClip discordLeave;
    private AudioSource audioSource;
    private bool joined = false;

    private static IconManager _instance = null;
    public static IconManager instance => _instance;

    void Awake()
    {
        if (_instance != null)
        {
            Destroy(this);
            return;
        }
        _instance = this;

        rings = new List<GameObject>();
        names = new List<string>();
        foreach (Transform icon in transform)
        {
            GameObject ring = icon.GetChild(0).gameObject;
            rings.Add(ring);
            ring.SetActive(false);
            names.Add(icon.gameObject.name);
        }

        julia = transform.GetChild(6).gameObject;

        audioSource = GetComponent<AudioSource>();
    }

    public void Deactivate()
    {
        foreach(GameObject ring in rings)
            ring.SetActive(false);
    }

    public void Speak(string speakerName)
    {
        Deactivate();

        if (speakerName == "Everyone")
        {
            foreach (GameObject ring in rings)
                ring.SetActive(true);
        }
        else
        {
            rings[names.IndexOf(speakerName)].SetActive(true);
        }
    }

    [YarnCommand("Julia")]
    public static void ToggleJulia()
    {
        _instance.julia.SetActive(!_instance.julia.activeSelf);
        if (_instance.joined)
            _instance.audioSource.PlayOneShot(_instance.discordLeave);
        else
            _instance.audioSource.PlayOneShot(_instance.discordJoin);
        _instance.joined = !_instance.joined;
    }
}
