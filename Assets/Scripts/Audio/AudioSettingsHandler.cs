using UnityEngine;
using UnityEngine.Audio;

[DisallowMultipleComponent]

public class AudioSettingsHandler : MonoBehaviour
{

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
        Settings.MasterVolume.onChange += UpdateMasterVolume;
        Settings.MusicVolume.onChange += UpdateMusicVolume;
        Settings.SfxVolume.onChange += UpdateSfxVolume;
        Settings.VoiceVolume.onChange += UpdateVoiceVolume;
    }

    void Start()
    {
        UpdateMasterVolume(Settings.MasterVolume.Value);
        UpdateMusicVolume(Settings.MusicVolume.Value);
        UpdateSfxVolume(Settings.SfxVolume.Value);
        UpdateVoiceVolume(Settings.VoiceVolume.Value);
    }

    void OnDestroy()
    {
        if (instance == this)
        {
            instance = null;
            Settings.MasterVolume.onChange -= UpdateMasterVolume;
            Settings.MusicVolume.onChange -= UpdateMusicVolume;
            Settings.SfxVolume.onChange -= UpdateSfxVolume;
            Settings.VoiceVolume.onChange -= UpdateVoiceVolume;
        }
    }

    private void UpdateMasterVolume(float volume) => mixer.SetFloat("Master Volume", AudioUtility.VolumeToDecibels(volume));
    private void UpdateMusicVolume(float volume) => mixer.SetFloat("Music Volume", AudioUtility.VolumeToDecibels(volume));
    private void UpdateSfxVolume(float volume) => mixer.SetFloat("Sfx Volume", AudioUtility.VolumeToDecibels(volume));
    private void UpdateVoiceVolume(float volume) => mixer.SetFloat("Voice Volume", AudioUtility.VolumeToDecibels(volume));
}
