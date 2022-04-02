using UnityEngine;
using UnityEngine.Events;
using TMPro;

public class CustomOptionView : MonoBehaviour
{

    [SerializeField] private TextMeshProUGUI textMesh;
    public UnityEvent onOptionChosen;

    public void UpdateText(string text) => textMesh.text = text;
    public void InvokeOption() => onOptionChosen.Invoke();

    public void Clear()
    {
        UpdateText("");
        onOptionChosen.RemoveAllListeners();
    }
}
