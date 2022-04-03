using System.Collections;
using UnityEngine;
using TMPro;
using Yarn.Unity;

[DisallowMultipleComponent]

public class ArtHandler : MonoBehaviour
{

    private static ArtHandler instance = null;

    void Awake()
    {
        if (instance != null)
        {
            Debug.LogWarning("Another ArtHandler already exists.");
            Destroy(this);
        }
        instance = this;
    }

    void OnDestroy()
    {
        if (instance == this)
        {
            instance = null;
        }
    }

    private IEnumerator ShowArt(string artName)
    {
        yield return CustomDialogueView.WaitUntilNotRunning();
        GameObject art = GameObject.Find(artName);
        if (art != null)
            StartCoroutine(SpriteFadeIn(art.GetComponent<SpriteRenderer>(), 3f));
        yield break;
    }

    [YarnCommand("art")]
    private static IEnumerator FindArt(string artName)
    {
        if (instance != null)
            yield return instance.ShowArt(artName);
        yield break;
    }

    public static IEnumerator SpriteFadeIn(SpriteRenderer renderer, float duration)
    {
        Color color = renderer.color;
        color.a = 0f;
        float start = Time.time;
        float elapsed = Time.time - start;
        while (elapsed < duration)
        {
            color.a = Mathf.Lerp(0f, 1f, elapsed / duration);
            renderer.color = color;
            elapsed = Time.time - start;
            yield return null;
        }
        color.a = 1f;
        renderer.color = color;
        yield break;
    }
}
