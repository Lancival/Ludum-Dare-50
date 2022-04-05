using System.Collections;
using UnityEngine;
using UnityEngine.UI;
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
        StartCoroutine(SpriteFadeIn(GameObject.Find(artName)?.GetComponent<Image>(), 3f));
        yield break;
    }

    // Helper function associated with <<art>> command
    [YarnCommand("art")]
    private static IEnumerator FindArt(string artName)
    {
        yield return instance?.ShowArt(artName);
        yield break;
    }

    // Helper function which fades in and out an image over duration seconds
    public static IEnumerator SpriteFadeIn(Image image, float duration)
    {
        if (image != null)
        {
            Color color = image.color;
            color.a = 0f;
            float start = Time.time;
            float elapsed = Time.time - start;
            while (elapsed < duration)
            {
                color.a = Mathf.Lerp(0f, 1f, elapsed / duration);
                image.color = color;
                elapsed = Time.time - start;
                yield return null;
            }
            color.a = 1f;
            image.color = color;
            yield break;
        }
    }
}
