﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TestOnScenePlayAudio : MonoBehaviour
{
    // Start is called before the first frame update
    IEnumerator Start()
    {
        // For now just play the forest ambient
        AudioManager.Instance.CrossFadeAmbienceSound("Ambient_Forest", 3f);
        //AudioManager.Instance.CrossFadeAmbienceToZero(3f, 5f);
        //yield return DualAudioTest();
        yield break;
    }
    
    // IEnumerator DualAudioTest()
    // {
    //     //yield return new WaitForSeconds(5);
    //
    //     AudioManager.Instance.CrossFadeAmbienceToZero(3f);
    //     AudioManager.Instance.CrossFadeMusicSound("Music_JustSynth", 10, 1f, 2f);
    //     yield break;
    // }
}
