using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.Audio;

// DEPRECATED: No longer using resource file for retrieving audio files
[CreateAssetMenu(fileName = "AudioClipDatabase", menuName = "AudioClipDatabase")]
public class AudioClipDatabase : ScriptableObject {
    private static AudioClipDatabase _database;

    public static AudioClipDatabase Instance {
        get { return _database; }
    }

    public AudioClip[] audioClips;

    public void PopulateDatabase() {
        if (_database == null) {
            _database = this;
        } else if (_database != this) {
            Destroy(this);
        }

        DontDestroyOnLoad(this);

        audioClips = Resources.LoadAll<AudioClip>("Audio");
        Debug.Log("Audio Clip Database Found:" + audioClips.Length + " Sound Files.");
    }

    public AudioClip GetAudioClip(String name) {
        foreach (AudioClip clip in audioClips) {
            if (clip.name == name) {
                return clip;
            }
        }

        return null;
    }
}
