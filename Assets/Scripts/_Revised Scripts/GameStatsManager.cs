using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

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

public class GameStatsManager : MonoBehaviour
{
    public static GameStatsManager Instance { get; private set;}
    public _PartyManager _partyManager;
    public _BattleUIHandler _battleUIHandler;

    // Combat-Related Stats
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
        { "Handy", new CharacterStats("Handy", 38, 170, 170, true, true)},
        { "Gregor", new CharacterStats("Gregor", 21, 210, 210, true, true)},
        { "Cuboid", new CharacterStats("Cuboid", 31, 180, 180, true, true)}
    };
    public Dictionary<string, CharacterStats> L2AEnemies = new Dictionary<string, CharacterStats>
    {
        
    };
    public Dictionary<string, CharacterStats> L2BEnemies = new Dictionary<string, CharacterStats>
    {

    };
    public List<CharacterStats> currentPlayerStats = new List<CharacterStats>();
    public List<CharacterStats> currentPartyMembers = new List<CharacterStats>();
    public List<GameObject> spawnedPartyMembers = new List<GameObject>();

    // Sprint-Related Stats
    public bool infSprint, staminaRecharging, sprintLocked, isCurrentlySprinting;
    public float currStamina, maxStamina = 100, sprintCost = 35;
    Image staminaBar;
    private Coroutine recharge;


    public bool CanSprint() { return currStamina > 0 && !sprintLocked;}
    public void Sprint()
    {
        if (infSprint) return;

        if (currStamina > 0)
        {
            if (isCurrentlySprinting) {
                currStamina -= sprintCost * Time.deltaTime;
                if (staminaRecharging)
                {
                    staminaRecharging = false;
                    StopCoroutine(RechargeStamina());
                    recharge = null;
                }
            }
            // isCurrentlySprinting = true;

            if (!isCurrentlySprinting && currStamina < maxStamina) {StartRecharge();}
            if (currStamina <= 0)
            {
                currStamina = 0;
                sprintLocked = true;
                StartRecharge();
            }
        }
    }
    public void StartRecharge()
    {
        if (staminaRecharging) {return;}

        if (currStamina>=maxStamina)
        {
            currStamina = maxStamina;
            staminaRecharging = sprintLocked = false;
            return;
        }
        staminaRecharging = true;
        if (recharge != null) {StopCoroutine(recharge);}
        recharge = StartCoroutine(RechargeStamina());
    }
    private IEnumerator RechargeStamina()
    {
        yield return new WaitForSeconds(1f);
        while (currStamina < maxStamina)
        {
            if (isCurrentlySprinting) // Stop recharging if sprinting resumes
            {
                staminaRecharging = false;
                recharge = null;
                yield break;
            }

            currStamina += (150) * Time.deltaTime;
            if (currStamina >= maxStamina) {
                currStamina = maxStamina;
                staminaRecharging = sprintLocked = false;
                recharge = null;
                yield break;
            }
            yield return new WaitForSeconds(0.01f);
        }
    }

    public void PrintCharacterStats(Dictionary<string, CharacterStats> characterDict, string groupName)
    {
        Debug.Log($"--- {groupName} Stats ---");
        foreach (var entry in characterDict)
        {
            CharacterStats stats = entry.Value;
            Debug.Log($"Name: {stats.Name}, Attack: {stats.attack}, HP: {stats.currentHealth}/{stats.maxHealth}, Combatant: {stats.isCombatant}, Enemy: {stats.isEnemy}");
        }
    }


    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else {Destroy(gameObject);}

        currStamina = maxStamina;
        sprintLocked = false;

        _partyManager = GetComponentInChildren<_PartyManager>();
        _battleUIHandler = GetComponentInChildren<_BattleUIHandler>();

        staminaBar = GameObject.FindGameObjectWithTag("Stamina Bar").GetComponent<Image>();

        currentPlayerStats.Add (playerStats["Player"]);
    }
    public void Start()
    {
        // PrintCharacterStats(playerStats, "Player");
        // PrintCharacterStats(allPartyMembers, "Party Members");
        // PrintCharacterStats(L1Enemies, "Enemies");
    }
    public void Update()
    {
        staminaBar.fillAmount = currStamina/maxStamina;
    }
}
