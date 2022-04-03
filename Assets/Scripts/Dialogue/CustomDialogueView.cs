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

    [Header("Audio")]
        [Tooltip("AudioSource that should play the voiceovers.")]
        [SerializeField] private AudioSource audioSource;

    [Header("Text")]
        [Tooltip("TextMeshPro text component that should print the dialogue line.")]
            [SerializeField] private TextMeshProUGUI dialogueBox;
        [Tooltip("TextMeshPro text component that should print the character name.")]
            [SerializeField] private TextMeshProUGUI nameBox;
        [Tooltip("FadeCanvasGroup component that should fade in and out.")]
            [SerializeField] private FadeCanvasGroup fade;
    
    [Header("Dialogue Options")]
        [Tooltip("List of CustomOptionViews that will display the dialogue options.")]
        public List<CustomOptionView> optionViews = new List<CustomOptionView>();

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

        Settings.Subtitles.onChange += SubtitleVisibilityChange;
    }

    void OnDestroy()
    {
        if (_instance == this)
        {
            _instance = null;
            Settings.Subtitles.onChange -= SubtitleVisibilityChange;
        }
    }

    private void SubtitleVisibilityChange(bool subtitle)
    {
        if (running != null)
        {
            if (subtitle)
                fade.FadeIn();
            else
                fade.FadeOut();
        }
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

            // Update text boxes
            dialogueBox.text = dialogueLine.TextWithoutCharacterName.Text;
            nameBox.text = dialogueLine.CharacterName;

            // Play audio
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
        fade.FadeOut();
        yield break;
    }

    // Add lines to the queue and immediately continue until options are encountered. Start playing lines if they aren't playing already.
    /// <inheritdoc/>
    public override void RunLine(LocalizedLine dialogueLine, Action onDialogueLineFinished)
    {
        pendingLines.Enqueue(dialogueLine);
        if (running == null)
        {
            if (Settings.Subtitles.Value)
                fade.FadeIn();
            running = StartCoroutine(RunPendingLines());
        }
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
