﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
[CreateAssetMenu]

public class Survivor : ScriptableObject
{
    [SerializeField]
    private string name;

    public string Name { get { return name; } }
    [SerializeField]
    private  int damage;

    public int Damage { get { return damage; } }
    [SerializeField]
    private int health;

    public int Health { get { return health; } }
    [SerializeField]
    private bool isCombatant;
    public bool IsCombatant { get { return isCombatant; } }

    private int curHealth;
    public int CurHealth { get { return curHealth; } set { curHealth = value; } }

    public void Start() {
        curHealth = health;
    }


    [SerializeField]
    public RuntimeAnimatorController Animcontroller;

    public string GetName()
    {
        return name;
    }
    public int GetHealth()
    {
        return health;
    }
    



   

    public void AddHealth(int health)
    {
        curHealth += health;
    }
    public void DecHealth(int health)
    {
        curHealth -= health;
    }
    public Sprite Sprite;

    public Sprite walkingSprite;
    public Sprite GetWalkingSprite;

    public Sprite GetSprite() { return Sprite; }

    public override string ToString() {


        return $"{name}: hp:{curHealth}/{health} dmg:{damage}";
    }




}
