using System.Collections;
using UnityEngine;
using UnityEngine.Events;

[DisallowMultipleComponent]
[RequireComponent(typeof(SpriteRenderer))]
[RequireComponent(typeof(EdgeCollider2D))]

public class PlatformButton : MonoBehaviour
{

    public UnityEvent onFilled;

    private SpriteRenderer spriteRenderer;
    private MaterialPropertyBlock block;

    private float _fill;
    private float fill
    {
        get => _fill;
        set
        {
            _fill = value;
            block.SetFloat("_Fill", _fill);
            spriteRenderer.SetPropertyBlock(block);
            if (_fill >= 1f)
                onFilled.Invoke();
        }
    }

    // Number of seconds required to fill the bar.
    private static readonly float duration = 1f;

    private Coroutine coroutine = null;

    void Awake()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();

        block = new MaterialPropertyBlock();
        spriteRenderer.GetPropertyBlock(block);
        fill = 0f;
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (coroutine != null)
            StopCoroutine(coroutine);
        coroutine = StartCoroutine(FillUpCoroutine());
    }

    void OnTriggerExit2D(Collider2D other)
    {
        if (coroutine != null)
            StopCoroutine(coroutine);
        coroutine = StartCoroutine(DrainCoroutine());
    }

    private IEnumerator FillUpCoroutine()
    {
        if (fill < 1f)
        {
            float startTime = Time.time;
            float startFill = fill;
            do
            {
                fill = Mathf.Lerp(0f, 1f, (Time.time - startTime) / duration + startFill);
                if (fill >= 1f)
                    break;
                yield return null;
            }
            while (fill <= 1f);
        }
        fill = 1f;
        yield break;
    }

    private IEnumerator DrainCoroutine()
    {
        if (fill > 0f)
        {
            float startTime = Time.time;
            float oneMinusStartFill = 1f - fill;
            do
            {
                fill = Mathf.Lerp(1f, 0f, (Time.time - startTime) / duration + oneMinusStartFill);
                if (fill <= 0f)
                    break;
                yield return null;
            }
            while (fill > 0f);
        }
        fill = 0f;
        yield break;
    }
}
