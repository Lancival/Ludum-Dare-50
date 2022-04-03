using UnityEngine;

[DisallowMultipleComponent]
[RequireComponent(typeof(SpriteRenderer))]

public class DiscordShaderController : MonoBehaviour
{

    private SpriteRenderer spriteRenderer;
    private MaterialPropertyBlock block;

    [SerializeField] private bool _visible = false;
    public bool RingVisible
    {
        get => _visible;
        set
        {
            _visible = value;
            block.SetFloat("_Visible", _visible ? 1f : 0f);
            spriteRenderer.SetPropertyBlock(block);
        }
    }

    void Awake()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
        block = new MaterialPropertyBlock();
        spriteRenderer.GetPropertyBlock(block);
        block.SetFloat("_Visible", _visible ? 1f : 0f);
        spriteRenderer.SetPropertyBlock(block);
    }

}
