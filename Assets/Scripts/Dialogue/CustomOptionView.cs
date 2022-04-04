using UnityEngine;
using UnityEngine.Events;
using TMPro;

public class CustomOptionView : MonoBehaviour
{

    [Tooltip("TextMeshPro Text component that should show the text of the dialogue option.")]
        public TextMeshProUGUI textMesh;
    [Tooltip("Whether this CustomOptionView should register itself with the CustomDialogueView at the start of the scene.")]
        [SerializeField] private bool registerOnStart = true;

    [Header("Events")]
        [Tooltip("Event which is invoked when this option is chosen.")]
            public UnityEvent onOptionChosen;
        [Tooltip("Event which is invoked when this option is initialized.")]
            public UnityEvent onInitialize;


    public void Awake()
    {
        if (textMesh == null)
            Debug.Log(string.Format("{0} is missing a TextMeshPro text component!", gameObject.name));
        if (registerOnStart)
            Register();
    }
    public void OnDestroy() => UnRegister();

    public void Register()
    {
        if (CustomDialogueView.instance != null && !CustomDialogueView.instance.optionViews.Contains(this))
            CustomDialogueView.instance.optionViews.Add(this);
    }

    public void UnRegister()
    {
        if (CustomDialogueView.instance != null && CustomDialogueView.instance.optionViews.Contains(this))
            CustomDialogueView.instance.optionViews.Remove(this);
    }

    public void UpdateText(string text) => textMesh.text = text;
    public void InvokeOption() => onOptionChosen.Invoke();

    public void Clear()
    {
        UpdateText("");
        onOptionChosen.RemoveAllListeners();
    }

    public void SwitchSprite(Sprite sprite)
    {
        Debug.Log("Switching sprites!");
    }
}
