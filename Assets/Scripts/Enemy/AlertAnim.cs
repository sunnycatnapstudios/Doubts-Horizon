using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AlertAnim : MonoBehaviour {
    public Enemy Enemy;
    [HideInInspector] public Animator Alert;
    public int playCount, animPlayCount;


    [Serializable]
    public struct AudioClips {
        public AudioClip sfxSpotted;
    }

    [SerializeField] private AudioClips audioClips;

    // Start is called before the first frame update
    void Start() {
        Alert = GetComponent<Animator>();
    }

    // Update is called once per frame
    void Update() {
        if (Enemy.attack) {
            // Alert.Play("Alert");
            Alert.ResetTrigger("AlertNone");
            Alert.ResetTrigger("AlertLost");
            Alert.SetTrigger("AlertActive");
            if (playCount <= 0) {
                AudioManager.Instance.PlaySound(audioClips.sfxSpotted);
                playCount += 1;
            }
        } else if (Enemy.searching) {
            // if (animPlayCount == 0) {Alert.Play("AlertLost"); animPlayCount+
            Alert.ResetTrigger("AlertActive");
            Alert.ResetTrigger("AlertNone");
            Alert.SetTrigger("AlertLost");
        } else {
            // Alert.Play("AlertNone");
            Alert.ResetTrigger("AlertLost");
            Alert.ResetTrigger("AlertActive");
            Alert.SetTrigger("AlertNone");
            playCount = 0;
            animPlayCount = 0;
        }
    }
}
