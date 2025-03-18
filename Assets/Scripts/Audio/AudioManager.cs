using UnityEngine;
using UnityEngine.Audio;

public class AudioManager : MonoBehaviour {
    private static AudioManager _audioManager;

    public static AudioManager Instance {
        get { return _audioManager; }
    }

    [Header("Audio Sources")] // Audio Source game objects within the scene
    public AudioSource audioSource; // Source for sound effects (sfx), including the UI

    public AudioSource uiAudioSource; // Source for the UI sfx. Separate from general sound source
    public DoubleAudioSource ambienceDoubleSource; // Double source for ambient sounds
    public DoubleAudioSource musicDoubleSource; // Double source for music sounds

    [Header("Audio Mixers")] // Unity mixers and groups. Used for controlling volumes
    public AudioMixer mixer;

    public AudioMixerGroup soundGroup;
    public AudioMixerGroup uiGroup;
    public AudioMixerGroup ambientGroup;
    public AudioMixerGroup musicGroup;

    private void Awake() {
        if (_audioManager == null) {
            _audioManager = this;
        } else if (_audioManager != this) {
            Destroy(gameObject);
        }

        DontDestroyOnLoad(gameObject);

        // Set the groups to control by mixer later
        audioSource.outputAudioMixerGroup = soundGroup;
        uiAudioSource.outputAudioMixerGroup = uiGroup;
        ambienceDoubleSource.SetMixerGroup(ambientGroup);
        musicDoubleSource.SetMixerGroup(musicGroup);
    }

    public void RestartToDefault() {
        // Reset every source to default state
        SetMasterVolume(0);
        JustStopAmbienceSound();
        JustStopMusicSound();

        audioSource.Stop();
        uiAudioSource.Stop();
    }

    // TODO add more options later
    // Set the master volume of all sounds
    public void SetMasterVolume(float volume) {
        mixer.SetFloat("masterVolume", volume);
    }
    // TODO add more options later

    // Play sound from an audio source. Defaults to audioSource if no alternative source is provided
    public void PlaySound(AudioClip clip, AudioSource source) {
        if (!source.outputAudioMixerGroup) {
            // Default assign soundmixergroup if none set
            source.outputAudioMixerGroup = soundGroup;
        }

        source.PlayOneShot(clip); // Play the audio source
    }

    public void PlaySound(AudioClip clip) {
        PlaySound(clip, audioSource);
    }

    // Play UI sounds from the UI source
    public void PlayUiSound(AudioClip clip) {
        audioSource.PlayOneShot(clip);
    }

    // Cross-fade the ambient sounds
    public void CrossFadeAmbienceSound(AudioClip clip, float fadeTime, float maxVolume = 1,
        float delayBeforeCrossFade = 0) {
        ambienceDoubleSource.CrossFade(clip, fadeTime, maxVolume, delayBeforeCrossFade);
    }

    // Fade the ambient source to silence.
    public void CrossFadeAmbienceToZero(float fadeTime, float delayBeforeCrossFade = 0) {
        ambienceDoubleSource.CrossFadeToZero(fadeTime, delayBeforeCrossFade);
    }

    // Just play ambient sound from ambient source (shouldn't be necessary)
    public void JustPlayAmbienceSound(AudioClip clip) {
        ambienceDoubleSource.JustPlaySound(clip);
    }

    // Just stop playing ambient sounds (shouldn't be necessary)
    public void JustStopAmbienceSound() {
        ambienceDoubleSource.JustStopSound();
    }

    // Cross-fade the music sounds
    public void CrossFadeMusicSound(AudioClip clip, float fadeTime, float maxVolume = 1,
        float delayBeforeCrossFade = 0) {
        musicDoubleSource.CrossFade(clip, fadeTime, maxVolume, delayBeforeCrossFade);
    }

    // Fade the music source to silence.
    public void CrossFadeMusicToZero(float fadeTime, float delayBeforeCrossFade = 0) {
        musicDoubleSource.CrossFadeToZero(fadeTime, delayBeforeCrossFade);
    }

    // Just play music sound from music source (shouldn't be necessary)
    public void JustPlayMusicSound(AudioClip clip) {
        musicDoubleSource.JustPlaySound(clip);
    }

    // Just stop music sounds (shouldn't be necessary)
    public void JustStopMusicSound() {
        musicDoubleSource.JustStopSound();
    }

    // For fetching the current/other clip in the sources
    public AudioClip AmbienceCurrentClip {
        get { return ambienceDoubleSource.CurrentClip; }
    }

    public AudioClip AmbienceOtherClip {
        get { return ambienceDoubleSource.OtherClip; }
    }

    public AudioClip MusicCurrentClip {
        get { return musicDoubleSource.CurrentClip; }
    }

    public AudioClip MusicOtherClip {
        get { return musicDoubleSource.OtherClip; }
    }
}
