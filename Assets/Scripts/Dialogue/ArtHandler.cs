using System.Collections;
using UnityEngine;
using Yarn.Unity;

[DisallowMultipleComponent]

public class ArtHandler : MonoBehaviour
{
    private static ArtHandler instance = null;

    // Check if another ArtHandler already exists
    void Awake()
    {
        if (instance != null)
        {
            Debug.LogWarning("Another ArtHandler already exists.");
            Destroy(this);
            return;
        }
        instance = this;
    }

    // Reset instance if the current instance is destroyed
    void OnDestroy()
    {
        if (instance == this)
            instance = null;
    }

    // Find and fade in the sprite named artName
    private IEnumerator ShowArt(string artName)
    {
        yield return CustomDialogueView.WaitUntilNotRunning();
        GameObject art = GameObject.Find(artName);
        art.SetActive(true);
        StartCoroutine(SpriteFadeIn(art.GetComponent<SpriteRenderer>(), 3f));
        yield break;
    }

    // Helper function associated with <<art>> command
    [YarnCommand("art")]
    private static IEnumerator FindArt(string artName)
    {
        yield return instance?.ShowArt(artName);
        yield break;
    }

    // Helper function which fades in and out a SpriteRenderer over duration seconds
    public static IEnumerator SpriteFadeIn(SpriteRenderer renderer, float duration)
    {
        if (renderer != null)
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
}
