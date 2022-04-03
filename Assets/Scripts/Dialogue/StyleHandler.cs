using System.Collections;
using UnityEngine;
using TMPro;
using Yarn.Unity;

[DisallowMultipleComponent]

public class StyleHandler : MonoBehaviour
{
    private static StyleHandler instance = null;

    [SerializeField] private TMP_FontAsset[] fonts;
    [SerializeField] private Sprite[] sprites;

    void Awake()
    {
        if (instance != null)
        {
            Debug.LogWarning("Another StyleHandler already exists.");
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

    private void SwitchStyles(string style)
    {
        Debug.Log(string.Format("Switching styles to {0}!", style));

        int index = 0;
        switch (style)
        {
            case "cuteBakery":
                break;
            case "horrorScary":
                index = 1;
                break;
            case "visualNovel":
                index = 2;
                break;
        }

        TMP_FontAsset font = fonts[index];
        Sprite sprite = sprites[index];

        if (font != null)
        {
            CustomDialogueView.instance.dialogueBox.font = font;
            foreach (CustomOptionView view in CustomDialogueView.instance.optionViews)
                view.textMesh.font = font;
        }
        if (sprite != null)
        {
            foreach (CustomOptionView view in CustomDialogueView.instance.optionViews)
                view.SwitchSprite(sprite);
        }
    }

    [YarnCommand("style")]
    public static IEnumerator ChangeStyle(string style)
    {
        yield return CustomDialogueView.WaitUntilNotRunning();
        if (instance != null)
            instance.SwitchStyles(style);
        yield break;
    }
}
