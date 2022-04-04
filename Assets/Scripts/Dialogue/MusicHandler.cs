using System.Collections;
using UnityEngine;
using UnityEngine.Audio;
using Yarn.Unity;

[DisallowMultipleComponent]
[RequireComponent(typeof(AudioSource))]

public class MusicHandler : MonoBehaviour
{
    private static MusicHandler instance = null;
    
    private AudioSource[] sources;
    private int playing = -1;

    [SerializeField] private AudioClip[] clips;
    [SerializeField] private float duration = 1.0f;

    void Awake()
    {
        if (instance != null)
        {
            Debug.LogWarning("Another MusicHandler already exists.");
            Destroy(this);
            return;
        }
        instance = this;
        sources = GetComponents<AudioSource>();
    }

    void OnDestroy()
    {
        if (instance == this)
        {
            instance = null;
        }
    }

    private void PlayMusic(string music)
    {
        int index = 0;
        switch (music)
        {
            case "songBakery":
                break;
            case "songHorror":
                index = 1;
                break;
            case "songVN":
                index = 2;
                break;
        }

        AudioClip clip = clips[index];
        if (clip != null)
        {
            if (playing == -1)
            {
                sources[0].clip = clip;
                StartCoroutine(AudioUtility.PlayAndFadeInAudioSource(sources[0], 1f, duration));
                playing = 0;
            }
            else if (playing == 0)
            {
                sources[1].clip = clip;
                StartCoroutine(AudioUtility.CrossfadeAudioSourcesWithPlayAndStop(sources[1], 1f, sources[0], duration));
                playing = 1;
            }
            else
            {
                sources[0].clip = clip;
                StartCoroutine(AudioUtility.CrossfadeAudioSourcesWithPlayAndStop(sources[0], 1f, sources[1], duration));
                playing = 0;
            }
        }
    }

    [YarnCommand("music")]
    public static IEnumerator ChangeMusic(string music)
    {
        yield return CustomDialogueView.WaitUntilNotRunning();
        if (instance != null)
            instance.PlayMusic(music);
        yield break;
    }
}
