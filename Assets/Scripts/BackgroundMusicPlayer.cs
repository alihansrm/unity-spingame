using UnityEngine;

[RequireComponent(typeof(AudioSource))]
public sealed class BackgroundMusicPlayer : MonoBehaviour
{
    [SerializeField] private AudioSource musicSource;
    [SerializeField] private AudioClip musicClip;
    [SerializeField, Range(0f, 1f)] private float musicVolume = 0.35f;
    [SerializeField, Min(0f)] private float fadeInSeconds = 2f;
    [SerializeField, Min(0f)] private float fadeOutSeconds = 3f;

    private void OnValidate()
    {
        musicSource = GetComponent<AudioSource>();
        fadeInSeconds = Mathf.Max(0f, fadeInSeconds);
        fadeOutSeconds = Mathf.Max(0f, fadeOutSeconds);
    }

    private void Awake()
    {
        musicSource = GetComponent<AudioSource>();
        musicSource.playOnAwake = false;
        musicSource.loop = true;
        musicSource.spatialBlend = 0f;
        musicSource.dopplerLevel = 0f;
        musicSource.volume = 0f;
        musicSource.clip = musicClip;
    }

    private void OnEnable()
    {
        if (musicClip == null)
        {
            Debug.LogWarning("Arka plan müziği atanmamış.", this);
            return;
        }

        musicSource.volume = 0f;
        musicSource.Play();
    }

    private void Update()
    {
        if (!musicSource.isPlaying || musicClip == null)
            return;

        // Use the audio playback position: fading follows the music even if game time is paused.
        float position = (float)musicSource.timeSamples / musicClip.frequency;
        float duration = (float)musicClip.samples / musicClip.frequency;
        float fadeIn = Mathf.Min(fadeInSeconds, duration * 0.5f);
        float fadeOut = Mathf.Min(fadeOutSeconds, duration * 0.5f);
        float startGain = fadeIn > 0f ? Mathf.SmoothStep(0f, 1f, position / fadeIn) : 1f;
        float endGain = fadeOut > 0f ? Mathf.SmoothStep(0f, 1f, (duration - position) / fadeOut) : 1f;
        musicSource.volume = musicVolume * startGain * endGain;
    }

    private void OnDisable()
    {
        musicSource.Stop();
        musicSource.volume = 0f;
    }
}
