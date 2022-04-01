using System.Collections;
using System.Collections.Generic;
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
        else
        {
            Settings.MasterVolume.onChange += UpdateMasterVolume;
            Settings.MusicVolume.onChange += UpdateMusicVolume;
            Settings.SfxVolume.onChange += UpdateSfxVolume;
        }
    }

    void Start()
    {
        UpdateMasterVolume(Settings.MasterVolume.Value);
        UpdateMusicVolume(Settings.MusicVolume.Value);
        UpdateSfxVolume(Settings.SfxVolume.Value);
    }

    void OnDestroy()
    {
        if (instance == this)
        {
            instance = null;
            Settings.MasterVolume.onChange -= UpdateMasterVolume;
            Settings.MusicVolume.onChange -= UpdateMusicVolume;
            Settings.SfxVolume.onChange -= UpdateSfxVolume;
        }
    }

    private void UpdateMasterVolume(float volume) => mixer.SetFloat("Master Volume", AudioUtility.VolumeToDecibels(volume));
    private void UpdateMusicVolume(float volume) => mixer.SetFloat("Music Volume", AudioUtility.VolumeToDecibels(volume));
    private void UpdateSfxVolume(float volume) => mixer.SetFloat("Sfx Volume", AudioUtility.VolumeToDecibels(volume));
}
