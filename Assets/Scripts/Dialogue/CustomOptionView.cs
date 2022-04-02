using UnityEngine;
using UnityEngine.Events;
using TMPro;

public class CustomOptionView : MonoBehaviour
{

    [SerializeField] private TextMeshProUGUI textMesh;
    public UnityEvent onOptionChosen;

    public void Start()
    {
        if (CustomDialogueView.instance != null && !CustomDialogueView.instance.optionViews.Contains(this))
            CustomDialogueView.instance.optionViews.Add(this);
    }
    public void OnDestroy() => CustomDialogueView.instance.optionViews.Remove(this);

    public void UpdateText(string text) => textMesh.text = text;
    public void InvokeOption() => onOptionChosen.Invoke();

    public void Clear()
    {
        UpdateText("");
        onOptionChosen.RemoveAllListeners();
    }
}
