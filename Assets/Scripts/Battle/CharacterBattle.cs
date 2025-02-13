﻿using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharacterBattle : MonoBehaviour, IComparable<CharacterBattle>
{
    // TODO Build all types of characters off one base prefab.
    // private Character characterBase
    //private Player characterBase;           // Reference to a [player] object
    //private Enemy enemyCharacterBase;           // Reference to a [enemy] object
    
    private State state;                    // State of current entity
    
    private Vector3 slideTargetPositon;     // position to slide to 
    private Action onSlideComplete;         // Callback on slide complete

    private bool isPlayerTeam;              // true - player team | false - enemy team
    public bool IsPlayerTeam { get => isPlayerTeam;}
    
    private GameObject selectionCircleGameObject;   // The selctionCircle for a character
    private SpriteRenderer spriteRenderer;              // TODO the sprite renderer. Should reference a portrait for turn order view
    private HealthBar healthBarController;         // The healthbar script for a character
    
    private UnitStats unitStats;        // Stats system for a character
    
    private enum State
    {
        Idle,
        Sliding,
        Busy,
    }
    private void Awake()
    {
        //characterBase = GetComponent<Player>();
        selectionCircleGameObject = transform.Find("SelectionCircle").gameObject;
        spriteRenderer = GetComponent<SpriteRenderer>();
        healthBarController = transform.Find("HealthBar").GetComponent<HealthBar>();

        HideSelectionCircle();
        state = State.Idle;
    }

    private void Update()
    {
        switch (state)
        {
            case State.Idle:
                break;
            case State.Sliding:
                float slideSpeed = 4f;
                transform.position += (slideTargetPositon - GetPosition()) * (slideSpeed * Time.deltaTime);

                float reachedDistance = 1f;
                if (Vector3.Distance(GetPosition(), slideTargetPositon) < reachedDistance)
                {
                    // Arrived at slide target position
                    transform.position = slideTargetPositon;
                    onSlideComplete();
                }
                break;
            case State.Busy:
                break;
        }
    }
    
    // Called first in BattleHandler to setup this character
    public void Setup(bool isPlayerTeam)
    {
        this.isPlayerTeam = isPlayerTeam;
        if (this.isPlayerTeam)
        {
            // TODO if player, orientate characters and play their idle animation
            //characterBase.anim.Play("Walk Down");
        } 
        else
        {
            // TODO if enemy, orientate characters and play their idle animation
            //characterBase.anim.Play("Walk Left");
        }
        unitStats = GetComponent<UnitStats>();
        healthBarController.SetBarSize(unitStats.GetHealthPercentage());    // Set to current health
    }
    
    // Return the current position of this character
    public Vector3 GetPosition()
    {
        return transform.position;
    }

    public void TakeDamage(int damageAmount)
    {
        unitStats.TakeDamage(damageAmount);
        Debug.Log("Hit! Current Health = " + unitStats.GetHealth());
        healthBarController.SetBarSize(unitStats.GetHealthPercentage());
        
        if (IsDead())
        {
            // TODO play death animation if this character dead
            //characterBase.die
        }
    }
    
    // Attack the targeted character
    public void Attack(CharacterBattle targetCharacterBattle, Action onAttackComplete)
    {
        Vector3 slideTargetPositon = targetCharacterBattle.GetPosition() +  (GetPosition() - targetCharacterBattle.GetPosition()).normalized * 2f;
        Vector3 startingPosition = GetPosition();
        
        SlideToPosition(slideTargetPositon, onSlideComplete: () =>
        {
            state = State.Busy;
            // After Slide complete, play attack animation
            Vector3 attackDir = targetCharacterBattle.GetPosition() - GetPosition().normalized;     // Direction vector toward enemy
            //characterBase.anim.Play("Walk Up");     // TODO Attack animation, wait for anim complete
            //characterBase.anim.Play("Walk Left");   // TODO create a dedicated animation player to handle logic during, at, and after an animation
            
            targetCharacterBattle.TakeDamage(unitStats.GetAttack());      // Deal damage to opponent
            
            // Attack Complete, slide back
            SlideToPosition(startingPosition, onSlideComplete: () =>
            {
                // TODO set back to idle animation
                state = State.Idle;
                onAttackComplete();
            });
        });
        
    }

    // Uses list of targets to auto target an opponent. 
    // TODO can replace with modular AI script
    public int AutoPickTarget(List<CharacterBattle> targets)
    {
        int targetIndex = 0;
        // For now just target the front player member
        foreach (CharacterBattle target in targets)
        {
            if (!target.IsDead())
            {
                return targetIndex;
            }
            targetIndex++;
        }

        return -1;  // Should never reach here
    }
    
    public bool IsDead()
    {
        return unitStats.IsDead();
    }

    private void SlideToPosition(Vector3 slideTargetPositon, Action onSlideComplete)
    {
        this.slideTargetPositon = slideTargetPositon;
        this.onSlideComplete = onSlideComplete;
        state = State.Sliding;
        if (slideTargetPositon.x > 0)
        {
            // TODO play slide right animation
        }
        else
        {
            // TODO play slide left animation
        }
    }

    // TODO return the portrait of the character. For now just return the default sprite
    public Sprite GetPortraitSprite()
    {
        return spriteRenderer.sprite;
    }
    
    // Show and hide characters selection circle
    public void HideSelectionCircle()
    {
        selectionCircleGameObject.SetActive(false);
    }

    public void ShowSelectionCircle()
    {
        selectionCircleGameObject.SetActive(true);
    }

    // Use to sort characters by their speed
    public int CompareTo(CharacterBattle other)
    {
        return other.unitStats.GetSpeed().CompareTo(unitStats.GetSpeed());
    }
}
