using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharacterStats
{

    public string Name;
    public int attack, currentHealth, maxHealth;
    public bool isCombatant, isEnemy, isfed;
    public CharacterStats(string name, int att, int maxhealth, int currhealth, bool iscombatant, bool isenemy) {
        Name = name;
        attack = att;
        currentHealth = currhealth;
        maxHealth = maxhealth;
        isCombatant = iscombatant;
        isEnemy = isenemy;
    }
}
