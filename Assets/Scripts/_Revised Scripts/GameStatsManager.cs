using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[System.Serializable]
public class CharacterStats
{
    public string Name;
    public int attack, currentHealth, maxHealth;
    public bool isCombatant, isEnemy;
    public CharacterStats(string name, int att, int currhealth, int maxhealth, bool iscombatant, bool isenemy)
    {
        Name = name;
        attack = att;
        currentHealth = currhealth;
        maxHealth = maxhealth;
        isCombatant = iscombatant;
        isEnemy = isenemy;
    }
}
[System.Serializable]
public class _Inventory
{
    public List<Item> items = new List<Item>();
}

[System.Serializable]
public class _Item
{
    public string itemName;
    public int quantity;
}

public class GameStatsManager : MonoBehaviour
{
    public static GameStatsManager Instance { get; private set;}

    // public PlayerStats playerStats;
    // public List<CharacterStats> partyStats = new List<CharacterStats>();
    // public Dictionary<int, EnemyStats> enemyStats = new Dictionary<int, EnemyStats>();
    // public Inventory inventory;
    public Dictionary<string, CharacterStats> playerStats = new Dictionary<string, CharacterStats>
    {
        { "Player", new CharacterStats("Player", 50, 150, 150, true, false)}
    };
    public Dictionary<string, CharacterStats> allPartyMembers = new Dictionary<string, CharacterStats>
    {
        { "MemberA", new CharacterStats("MemberA", 35, 100, 100, true, false)},
        { "MemberB", new CharacterStats("MemberB", 7, 120, 120, false, false)},
        { "MemberC", new CharacterStats("MemberC", 5, 150, 150, false, false)},
        { "MemberD", new CharacterStats("MemberD", 25, 80, 80, true, false)},
        { "MemberE", new CharacterStats("MemberE", 42, 170, 170, true, false)}
    };
    public Dictionary<string, CharacterStats> L1Enemies = new Dictionary<string, CharacterStats>
    {
        { "Handy", new CharacterStats("Handy", 38, 100, 100, true, true)},
        { "Gregor", new CharacterStats("Gregor", 21, 120, 120, true, true)},
        { "Turf", new CharacterStats("Turf", 31, 150, 150, true, true)}
    };


    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }
}
