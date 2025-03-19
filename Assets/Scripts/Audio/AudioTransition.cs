using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;

public class AudioTransition : MonoBehaviour {
    public float delayBeforeStart; // How long to wait before starting cross-fade
    public float crossfadeDuration; // Duration of cross-fade
    public float crossVolume = 1; // Should just be left as 1

    // Choose this to cross mute the audio. false by default
    [SerializeField] public bool crossMuteMusic;
    [SerializeField] public bool crossMuteAmbience;

    // next clips to cross-fade. Make sure to disable the mute option beforehand
    [Serializable]
    private struct AudioClips {
        public AudioClip nextMusicClip;
        public AudioClip nextAmbientClip;
    }

    [SerializeField] private AudioClips audioClips;

    public bool enableColliderTransition = true; // Toggle if script automatically trigger on collider

    // Call in other scripts to trigger
    public void TriggerAudioTransition() {
        if (crossVolume > 1 || crossVolume < 0) {
            Debug.LogWarning("AudioTransition: Crossing volume is out of range of [0,1].");
            return;
        }

        if (crossfadeDuration < 0 || delayBeforeStart < 0) {
            Debug.LogWarning("AudioTransition: Crossing duration/Delay before start cannot be less than 0.");
            return;
        }

        if (audioClips.nextMusicClip != null || crossMuteMusic) {
            if (crossMuteMusic) {
                Debug.Log("Test111");
                AudioManager.Instance.CrossFadeMusicToZero(crossfadeDuration, delayBeforeStart);
            } else {
                Debug.Log("Fading");
                AudioManager.Instance.CrossFadeMusicSound(audioClips.nextMusicClip, crossfadeDuration, crossVolume,
                    delayBeforeStart);
            }
        }

        if (audioClips.nextAmbientClip != null || crossMuteAmbience) {
            if (crossMuteAmbience) {
                AudioManager.Instance.CrossFadeAmbienceToZero(crossfadeDuration, delayBeforeStart);
            } else {
                AudioManager.Instance.CrossFadeAmbienceSound(audioClips.nextAmbientClip, crossfadeDuration, crossVolume,
                    delayBeforeStart);
            }
        }
    }

    // If attached to an object with a box collider, automatically handle on collision triggers
    // Trigger transition when leaving the collider
    public void OnTriggerExit2D(Collider2D other) {
        if (other.CompareTag("Player") && enableColliderTransition) {
            TriggerAudioTransition();
        }
    }
}
