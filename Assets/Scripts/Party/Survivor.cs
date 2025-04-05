using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu]
public class Survivor : ScriptableObject {
    [SerializeField] private string name;

    public string Name {
        get { return name; }
    }

    [SerializeField] private int damage;

    public int Damage {
        get { return charstats.attack; }
    }

    [SerializeField] private int health;

    public int Health {
        get { return health; }
    }
    public List<string> deathDialogue;

    public List<string> starvedDialogue;

    [SerializeField] private bool isCombatant;

    [SerializeField] public InventoryDialogues inventoryDialogues;

    [SerializeField] public Sprite OverworldSprite;

    [SerializeField] private bool unKickable;
    private bool Hungry = false;

    public bool UnKickable {
        get { return unKickable; }
    }

    public bool IsCombatant {
        get { return isCombatant; }
    }

    public bool Fed {
        get { return charstats.isfed; }
        set { charstats.isfed = value; }
    }

    public int CurHealth {
        get { return charstats.currentHealth; }
        set { charstats.currentHealth = value; }
    }

    public int currentHealth {
        get { return charstats.currentHealth; }
        set { charstats.currentHealth = value; }
    }

    public int maxHealth {
        get { return charstats.maxHealth; }
        set { charstats.maxHealth = value; }
    }

    public string enteringFireDialogue;

    private CharacterStats charstats;


    [SerializeField] public RuntimeAnimatorController Animcontroller;

    public string GetName() {
        return name;
    }

    public int GetHealth() {
        return health;
    }

    public void OnEnable() {
        charstats = new CharacterStats(Name, damage, health, health, isCombatant, false);
    }

    public CharacterStats GetCharStats() {
        return charstats;
    }

    public void AddHealth(int health) {
        charstats.currentHealth += health;
        if (charstats.currentHealth > maxHealth) charstats.currentHealth = maxHealth;
    }

    public void DecHealth(int health) {
        charstats.currentHealth -= health;
    }
    public void AddDamage(int Damage) {
        charstats.attack += Damage;
    }

    public Sprite Sprite;

    public Sprite walkingSprite;
    public Sprite GetWalkingSprite;

    public Sprite GetSprite() {
        return Sprite;
    }

    public override string ToString() {
        return $"{name}: hp:{charstats.currentHealth}/{health} dmg:{damage}";
    }




    public string GetInventoryDialogue() {


        if (currentHealth < maxHealth * 0.5 && Hungry) {
            return inventoryDialogues.LowHealthAndLowHunger();
        } else if (currentHealth < maxHealth * 0.5) {
            return inventoryDialogues.LowHealthDialogue();

        } else if (Hungry) {
            return inventoryDialogues.LowHungerDialogue();
        } else {
            return inventoryDialogues.NormalDialogue();
        }

    }
}
