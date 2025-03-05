using System.Collections;
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

    [SerializeField]
    private bool unKickable;

    public bool UnKickable { get { return unKickable; } }
    public bool IsCombatant { get { return isCombatant; } }
    public bool Fed { get { return charstats.isfed; } set { charstats.isfed = value; } }
    public int CurHealth { get { return charstats.currentHealth; } set { charstats.currentHealth = value; } }
    private CharacterStats charstats;



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
    public void OnEnable() {
        Debug.Log("does this ever run");
        charstats  = new CharacterStats(Name, damage, health, health,isCombatant,false);


    }
    public CharacterStats GetCharStats() {
        return charstats;


    }

    public void AddHealth(int health)
    {
        charstats.currentHealth += health;
    }
    public void DecHealth(int health)
    {
        charstats.currentHealth -= health;
    }
    public Sprite Sprite;

    public Sprite walkingSprite;
    public Sprite GetWalkingSprite;

    public Sprite GetSprite() { return Sprite; }

    public override string ToString() {


        return $"{name}: hp:{charstats.currentHealth}/{health} dmg:{damage}";
    }




}
