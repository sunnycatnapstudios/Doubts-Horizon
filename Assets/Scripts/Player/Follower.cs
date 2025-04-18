﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Follower : MonoBehaviour {
    public int order, partyIndex;
    private Player Player;
    private _PartyManager _partyManager;
    public float followSpeed;
    public Vector3 currentPos, newPos;
    public SpriteRenderer spriteState;
    public Animator partyAnim;
    private PartyManager partyManager;

    void Awake() {
        Player = GameObject.FindGameObjectWithTag("Player").GetComponent<Player>();
    }
    void Start() {
        if (order != 0) { transform.position = Player.transform.position; }

        spriteState = GetComponent<SpriteRenderer>();
        partyAnim = GetComponent<Animator>();
        //_partyManager = GameStatsManager.Instance._partyManager;
        partyManager = GameStatsManager.Instance.partyManager;

    }

    // Update is called once per frame
    void Update() {
        float refX = transform.position.x, refY = transform.position.y;

        // Handles Party Movement and Placement
        // partyIndex = Mathf.Abs(partyManager.partyCount-order);
        partyIndex = Mathf.Abs(partyManager.partyCount - order);//minus one because player now in count i belieev

        if (partyIndex >= 0 && partyIndex < Player.moveHist.Count) { newPos = Player.moveHist[partyIndex]; } else { newPos = transform.position; }

        followSpeed = Player.moveSpeed;
        if (order != 0) {
            if (Vector3.Distance(newPos, transform.position) > 20) {
                transform.position = newPos;
            } else {
                transform.position = Vector3.MoveTowards(transform.position, newPos, followSpeed * Time.deltaTime);
            }
        }

        //Debug.Log(partyAnim.name);
        if(partyAnim.name == "LockSmith Bass") {


            if (Mathf.Abs(transform.position.x - refX) > Mathf.Abs(transform.position.y - refY)&& transform.position.x - refX > 0) {
               
                partyAnim.Play("PartyRight");
            } else if (transform.position.y - refY > 0) {
                partyAnim.Play("PartyUp");
            } else if (transform.position.y - refY < 0) {
                partyAnim.Play("PartyDown");
            } else if (Mathf.Abs(transform.position.x - refX) > Mathf.Abs(transform.position.y - refY) && transform.position.x - refX < 0){
                partyAnim.Play("PartyLeft");

            }
            return;
        }
        if (transform.position.x - refX > 0) { spriteState.flipX = true; }


        else if (transform.position.x - refX < 0) { spriteState.flipX = false; }

        if (Mathf.Abs(transform.position.x - refX) > Mathf.Abs(transform.position.y - refY)) {
            partyAnim.Play("PartyLeft");
        } else if (transform.position.y - refY > 0) {
            partyAnim.Play("PartyUp");
        } else if (transform.position.y - refY < 0) {
            partyAnim.Play("PartyDown");
        }
    }
}
