using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;
using UnityEngine.Audio;
using Yarn.Unity;

public class CustomDialogueView : DialogueViewBase
{

    [SerializeField] private TextMeshProUGUI textBox;
    [SerializeField] private AudioSource audioSource;
    public List<CustomOptionView> optionViews;

    private Queue<LocalizedLine> pendingLines;
    private Coroutine running = null;

    private static CustomDialogueView _instance = null;
    public static CustomDialogueView instance => _instance;

    void Awake()
    {
        if (_instance != null)
        {
            Debug.LogWarning("Another CustomDialogueView component already exists.");
            Destroy(this);
        }
        _instance = this;
        pendingLines = new Queue<LocalizedLine>();
        optionViews = new List<CustomOptionView>();
    }

    void OnDestroy()
    {
        if (_instance == this)
            _instance = null;
    }

    private void ClearOptions()
    {
        foreach (CustomOptionView option in optionViews)
        {
            option.onOptionChosen.RemoveAllListeners();
            option.UpdateText("");
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
        if (dialogueOptions.Length > optionViews.Count)
            Debug.LogError("Number of dialogue options exceeds number of CustomOptionViews!");

        for (int i = 0; i < dialogueOptions.Length && i < optionViews.Count; i++)
        {
            DialogueOption dialogueOption = dialogueOptions[i];
            CustomOptionView optionView = optionViews[i];

            optionView.onOptionChosen.AddListener(() =>
                {
                    // Interrupt currently playing line and clear pending lines
                    if (running != null)
                    {
                        StopCoroutine(running);
                        running = null;
                    }
                    pendingLines.Clear();

                    // Handle dialogue option and clear buttons
                    onOptionSelected(dialogueOption.DialogueOptionID);
                    ClearOptions();
                });
            optionView.UpdateText(dialogueOption.Line.TextWithoutCharacterName.Text);
        }
    }
}
