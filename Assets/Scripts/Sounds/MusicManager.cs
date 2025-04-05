using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class MusicManager : MonoBehaviour
{
    public static MusicManager instance;
    private AudioSource audioSource;
    public AudioClip bgm;
    [SerializeField] private Slider musicSlider;
    [SerializeField] private TextMeshProUGUI musicVolumeText;
    [SerializeField] private Animator musicAnimator;

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
        else
        {
            return;
        }

        audioSource = GetComponent<AudioSource>();
    }

    private void Start()
    {
        if (bgm != null)
        {
            PlayBGM(false, bgm);

        } else
        {
            PlayBGM(true, bgm);
        }
            musicSlider.onValueChanged.AddListener(delegate { SetVolume(musicSlider.value); });
            SetVolume(musicSlider.value = 2);
    }

    public static void SetVolume(float volume)
    {
        float normalizedVolume = volume / 10f;
        instance.audioSource.volume = normalizedVolume;
        instance.musicAnimator.SetFloat("Volume", normalizedVolume);

        if (instance.musicVolumeText != null)
        {
            instance.musicVolumeText.text = (volume * 10).ToString("0") + "%";
        }
    }

    public void PlayBGM(bool resetSong, AudioClip audioClip = null)
    {
        if (audioClip != null)
        {
            audioSource.clip = audioClip;
            audioSource.loop = true;
        }

        if (audioSource.clip != null)
        {
            if (resetSong)
            {
                audioSource.Stop();
            }

            audioSource.volume = 0;
            audioSource.Play();
            StartCoroutine(FadeInVolume(0.2f, 2f));
        }
    }

    public void PauseBGM()
    {
        audioSource.Pause();
    }

    private IEnumerator FadeInVolume(float targetVolume, float duration)
    {
        float startVolume = audioSource.volume;
        float time = 0;

        while (time < duration)
        {
            time += Time.deltaTime;
            audioSource.volume = Mathf.Lerp(startVolume, targetVolume, time / duration);
            yield return null;
        }

        audioSource.volume = targetVolume;
    }
}
