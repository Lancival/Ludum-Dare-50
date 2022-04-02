using System;
using UnityEngine;
using TMPro;
using UnityEngine.UI;
using UnityEngine.Audio;
using Yarn.Unity;

public class CustomView : DialogueViewBase
{

    [SerializeField] private TextMeshProUGUI textBox;
    [SerializeField] private AudioSource audioSource;

    private static CustomView instance = null;

    void Awake()
    {
        if (instance != null)
        {
            Debug.LogWarning("Another CustomView component already exists.");
            Destroy(this);
        }
        instance = this;
    }

    void OnDestroy()
    {
        if (instance == this)
            instance = null;
    }

    // Output line to TextMeshPro text component instantaneously
    /// <inheritdoc/>
    public override void RunLine(LocalizedLine dialogueLine, Action onDialogueLineFinished)
    {
        textBox.text = dialogueLine.TextWithoutCharacterName.Text;
        if (dialogueLine is AudioLocalizedLine)
        {
            AudioLocalizedLine audioLine = (AudioLocalizedLine) dialogueLine;
            AudioClip clip = audioLine.AudioClip;
            if (clip != null)
                audioSource.PlayOneShot(clip);
            else
                Debug.LogWarning("No AudioClip was provided for this line.");
        }
    }

    // Wait for user to advance dialogue, then interrupt the current line to end it
    /// <inheritdoc/>
    public override void UserRequestedViewAdvancement()
    {
        audioSource.Stop();
        requestInterrupt?.Invoke();
    }

    // End the current line
    /// <inheritdoc/>
    public override void InterruptLine(LocalizedLine dialogueLine, Action onInterruptLineFinished)
    {
        onInterruptLineFinished?.Invoke();
    }
}
