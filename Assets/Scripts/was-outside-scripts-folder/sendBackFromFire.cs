﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class sendBackFromFire : MonoBehaviour
{
    GameObject player;
    PartyManager manager;
    // Start is called before the first frame update
    void Start() { 
        player = GameObject.FindGameObjectWithTag("Player");
        manager = player.GetComponent<PartyManager>();
    
        
    }

    private void OnTriggerEnter2D(Collider2D other) {
        GameObject[] followers = GameObject.FindGameObjectsWithTag("Followers");
        //followers = GameObject.FindGameObjectWithTag("Feed1").GetComponent<fireplace>().getFollowers();

        foreach (GameObject follower in followers) {
            follower.GetComponent<SpriteRenderer>().enabled = true;

        }
        List<Survivor> iterator = new List<Survivor>(manager.currentPartyMembers);
        foreach (Survivor survivor in iterator) {
            if (survivor.Fed) {

                survivor.Fed = false;

            } else {
                manager.RemoveFromParty(survivor);
                Debug.Log($"Kicked {survivor.GetName()} from party");

            }



        }

        
    }
}
