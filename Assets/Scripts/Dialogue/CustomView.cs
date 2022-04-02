using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;
using UnityEngine.Audio;
using Yarn.Unity;

public class CustomView : DialogueViewBase
{

    [SerializeField] private TextMeshProUGUI textBox;
    [SerializeField] private AudioSource audioSource;
    [SerializeField] private Button[] buttons;

    private Queue<LocalizedLine> pendingLines;
    private Coroutine running = null;

    private static CustomView instance = null;

    void Awake()
    {
        if (instance != null)
        {
            Debug.LogWarning("Another CustomView component already exists.");
            Destroy(this);
        }
        instance = this;
        pendingLines = new Queue<LocalizedLine>();
    }

    void OnDestroy()
    {
        if (instance == this)
            instance = null;
    }

    private void ClearButtons()
    {

        Debug.Log("Clearing buttons!");

        foreach (Button button in buttons)
        {
            button.onClick.RemoveAllListeners();
            button.transform.GetChild(0).GetComponent<TextMeshProUGUI>().text = "";
        }
    }

    // Run through the queued lines, outputting them to text and audio
    private IEnumerator RunPendingLines()
    {
        while (pendingLines.Count > 0)
        {
            LocalizedLine dialogueLine = pendingLines.Dequeue();
            textBox.text = dialogueLine.TextWithoutCharacterName.Text;
            if (dialogueLine is AudioLocalizedLine)
            {
                AudioLocalizedLine audioLine = (AudioLocalizedLine) dialogueLine;
                AudioClip clip = audioLine.AudioClip;
                if (clip != null)
                {
                    audioSource.PlayOneShot(clip);
                    yield return new WaitForSeconds(clip.length);
                }
                else
                {
                    Debug.LogWarning("No AudioClip was provided for this line.");
                    yield return new WaitForSeconds(3f);
                }
            }
        }
        running = null;
        yield break;
    }

    // Add lines to the queue and immediately continue until options are encountered. Start playing lines if they aren't playing already.
    /// <inheritdoc/>
    public override void RunLine(LocalizedLine dialogueLine, Action onDialogueLineFinished)
    {
        pendingLines.Enqueue(dialogueLine);
        if (running == null)
            running = StartCoroutine(RunPendingLines());
        onDialogueLineFinished?.Invoke();
    }

    // Handle Options
    /// <inheritdoc/>
    public override void RunOptions(DialogueOption[] dialogueOptions, Action<int> onOptionSelected)
    {
        if (dialogueOptions.Length > buttons.Length)
            Debug.LogError("Not enough buttons to show options!");

        for (int i = 0; i < dialogueOptions.Length && i < buttons.Length; i++)
        {
            DialogueOption option = dialogueOptions[i];
            Button button = buttons[i];

            button.onClick.AddListener(() =>
                {
                    // Interrupt currently playing line and clear pending lines
                    if (running != null)
                    {
                        StopCoroutine(running);
                        running = null;
                    }
                    pendingLines.Clear();

                    // Handle dialogue option and clear buttons
                    onOptionSelected(option.DialogueOptionID);
                    ClearButtons();
                });
            button.transform.GetChild(0).GetComponent<TextMeshProUGUI>().text = option.Line.TextWithoutCharacterName.Text;
        }
    }
}
