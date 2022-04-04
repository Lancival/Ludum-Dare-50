using UnityEngine;
using UnityEngine.Audio;
using static AudioUtility;
using static Settings;

[DisallowMultipleComponent]
public class AudioSettingsHandler : MonoBehaviour
{

    [Tooltip("The AudioMixer whose groups this handler should modify.")]
        [SerializeField] private AudioMixer mixer;
    private static AudioSettingsHandler instance = null;

    void Awake()
    {
        if (mixer == null)
        {
            Debug.LogWarning("AudioSettingsHandler missing reference to AudioMixer.");
            Destroy(this);
        }
        else if (instance != null)
        {
            Debug.LogWarning("Another instance of AudioSettingsHandler already exists.");
            Destroy(this);
        }

        instance = this;

        // Subscribe to audio change events
        MasterVolume.onChange += UpdateMasterVolume;
        MusicVolume.onChange += UpdateMusicVolume;
        SfxVolume.onChange += UpdateSfxVolume;
        VoiceVolume.onChange += UpdateVoiceVolume;
    }

    // Update AudioMixerGroups at the start of the scene
    void Start()
    {
        foreach (Setting<float> audioSetting in Settings.AudioSettings)
            UpdateVolume(audioSetting.name, audioSetting.Value);
    }

    // If this handler wasn't destroyed immediately, unsubscribe from settings change events
    void OnDestroy()
    {
        if (instance == this)
        {
            instance = null;
            MasterVolume.onChange -= UpdateMasterVolume;
            MusicVolume.onChange -= UpdateMusicVolume;
            SfxVolume.onChange -= UpdateSfxVolume;
            VoiceVolume.onChange -= UpdateVoiceVolume;
        }
    }

    // Helper functions to update AudioMixerGroup volumes
    private void UpdateVolume(string name, float volume) => mixer.SetFloat(name, VolumeToDecibels(volume));
    private void UpdateMasterVolume(float volume) => mixer.SetFloat("Master Volume", VolumeToDecibels(volume));
    private void UpdateMusicVolume(float volume) => mixer.SetFloat("Music Volume", VolumeToDecibels(volume));
    private void UpdateSfxVolume(float volume) => mixer.SetFloat("Sfx Volume", VolumeToDecibels(volume));
    private void UpdateVoiceVolume(float volume) => mixer.SetFloat("Voice Volume", VolumeToDecibels(volume));
}
