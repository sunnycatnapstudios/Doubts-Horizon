using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.Serialization;

public class DoubleAudioSource : MonoBehaviour {
    public AudioSource source1;
    public AudioSource source2;

    private bool _curSource = true; // True if cur = 1 | False if cur = 2

    private Coroutine _curSourceFadeRoutine = null;
    private Coroutine _newSourceFadeRoutine = null;

    private void Awake() {
        if (source1 == null || source2 == null) {
            throw new NullReferenceException();
        }
    }

    // Set the mixer group of both sources to the one specified
    public void SetMixerGroup(AudioMixerGroup group) {
        source1.outputAudioMixerGroup = group;
        source2.outputAudioMixerGroup = group;
    }

    // Just play a sound from the current source
    public void JustPlaySound(AudioClip clip) {
        if (_curSource) {
            source1.PlayOneShot(clip);
        } else {
            source2.PlayOneShot(clip);
        }
    }

    // Stop both sources
    public void JustStopSound() {
        source1.Stop();
        source2.Stop();
    }

    //gradually shifts the sound coming from our audio sources to this clip:
    // maxVolume should be in 0-to-1 range
    public void CrossFade(AudioClip clipToPlay, float fadingTime, float maxVolume = 1, float delayBeforeCrossFade = 0) {
        //var fadeRoutine = StartCoroutine(Fade(clipToPlay, maxVolume, fadingTime, delayBeforeCrossFade));
        StartCoroutine(Fade(clipToPlay, maxVolume, fadingTime, delayBeforeCrossFade));
    }

    // gradually fade into silence.
    public void CrossFadeToZero(float fadingTime, float delayBeforeCrossFade = 0) {
        StartCoroutine(Fade(null, 0, fadingTime, delayBeforeCrossFade));
    }

    private IEnumerator Fade(AudioClip playMe, float maxVolume, float fadingTime, float delayBeforeCrossFade = 0) {
        if (delayBeforeCrossFade > 0) {
            yield return new WaitForSecondsRealtime(delayBeforeCrossFade);
        }

        AudioSource curActiveSource, newActiveSource;
        if (_curSource) // Active source is source 1
        {
            //_source1 is currently playing the most recent AudioClip
            curActiveSource = source1;
            //so launch on _source2
            newActiveSource = source2;
        } else {
            //otherwise, _source2 is currently active
            curActiveSource = source2;
            //so play on _source1
            newActiveSource = source1;
        }

        //perform the switching
        newActiveSource.clip = playMe;
        newActiveSource.volume = 0;
        newActiveSource.Play();

        if (_curSourceFadeRoutine != null) {
            StopCoroutine(_curSourceFadeRoutine);
        }

        if (_newSourceFadeRoutine != null) {
            StopCoroutine(_newSourceFadeRoutine);
        }

        _curSourceFadeRoutine = StartCoroutine(fadeSource(curActiveSource, curActiveSource.volume, 0, fadingTime));
        _newSourceFadeRoutine =
            StartCoroutine(fadeSource(newActiveSource, newActiveSource.volume, maxVolume, fadingTime));

        _curSource = !_curSource; // Flip active source
    }

    // Perform the actual fade coroutine
    private IEnumerator fadeSource(AudioSource sourceToFade, float startVolume, float endVolume, float duration) {
        float startTime = Time.unscaledTime;

        do {
            float elapsed = Time.unscaledTime - startTime;

            if (duration <= 0) {
                duration = 0.1f; // For Lerp function
            }

            sourceToFade.volume = Mathf.Clamp01(Mathf.Lerp(startVolume, endVolume, elapsed / duration));

            // If "close enough" to target goal, stop the fade
            if (Math.Abs(sourceToFade.volume - endVolume) <= 0.1) {
                sourceToFade.volume = endVolume;
                if (endVolume == 0) {
                    sourceToFade.Stop();
                }

                break;
            }

            yield return null;
        } while (Time.unscaledTime < startTime + duration);
    }

    // Return the clip in the active source
    public AudioClip CurrentClip {
        get {
            if (_curSource) {
                return source1.clip;
            } else {
                return source2.clip;
            }
        }
    }

    // Return the clip in the inactive source
    public AudioClip OtherClip {
        get {
            if (!_curSource) {
                return source1.clip;
            } else {
                return source2.clip;
            }
        }
    }

    //returns false if BOTH sources are not playing and there are no sounds are staged to be played.
    //also returns false if one of the sources is not yet initialized
    public bool IsPlaying {
        get {
            if (source1 == null || source2 == null) {
                return false;
            }

            //otherwise, both sources are initialized. See if any is playing:
            return source1.isPlaying || source2.isPlaying;
        }
    }
}
