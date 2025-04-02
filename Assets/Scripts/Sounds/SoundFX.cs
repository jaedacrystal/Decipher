using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class SoundFX : MonoBehaviour
{
    public static SoundFX instance { get; private set; }

    [SerializeField] private Slider volumeSlider;
    [SerializeField] private TextMeshProUGUI volumeText;
    [SerializeField] private SoundEffectGroup[] soundEffectGroups;

    private Dictionary<string, List<AudioClip>> soundDictionary;
    private AudioSource audioSource;
    private AudioSource randomPitchAudioSource;

    public static float minPitch = 0.9f;
    public static float maxPitch = 1.2f;

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;

            InitializeDictionary();

            AudioSource[] audioSources = GetComponents<AudioSource>();
            if (audioSources.Length >= 2)
            {
                randomPitchAudioSource = audioSources[0];
                audioSource = audioSources[1];
            }
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private void Start()
    {
        if (volumeSlider != null)
        {
            volumeSlider.onValueChanged.AddListener(SetVolume);
            volumeText.text = (volumeSlider.value * 10).ToString("0") + "%";
        }

        SetVolume(volumeSlider.value);
    }

    private void InitializeDictionary()
    {
        soundDictionary = new Dictionary<string, List<AudioClip>>();

        foreach (SoundEffectGroup group in soundEffectGroups)
        {
            soundDictionary[group.name] = group.audioClips;
        }
    }

    public static void Play(string soundName)
    {
        if (instance.soundDictionary.TryGetValue(soundName, out List<AudioClip> clips) && clips.Count > 0)
        {
            SoundEffectGroup group = Array.Find(instance.soundEffectGroups, g => g.name == soundName);
            AudioClip clip = clips[UnityEngine.Random.Range(0, clips.Count)];

            if (group.randomPitch)
            {
                instance.randomPitchAudioSource.pitch = UnityEngine.Random.Range(minPitch, maxPitch);
                instance.randomPitchAudioSource.PlayOneShot(clip);
            }
            else
            {
                instance.audioSource.PlayOneShot(clip);
            }
        }
    }

    public void SetVolume(float volume)
    {
        float normalizedVolume = volume / 10f;
        audioSource.volume = normalizedVolume;
        randomPitchAudioSource.volume = normalizedVolume;
        volumeText.text = (volume * 10).ToString("0") + "%";
    }

}

[System.Serializable]
public struct SoundEffectGroup
{
    public string name;
    public List<AudioClip> audioClips;
    public bool randomPitch;
}