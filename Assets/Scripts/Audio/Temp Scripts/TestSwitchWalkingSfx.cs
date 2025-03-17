using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// TODO replace with actual tile based method later
// Temporary used to swap player walking sfx between level transition
public class TestSwitchWalkingSfx : MonoBehaviour {
    public Player player;
    public AudioClip audioClip;

    public void SwitchPlayerWalkingSfx() {
        player.walkAudi.clip = audioClip;
    }
}
