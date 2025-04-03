using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

[System.Serializable]
//public class CharacterStats
//{
//    public string Name;
//    public int attack, currentHealth, maxHealth;
//    public bool isCombatant, isEnemy;
//    public CharacterStats(string name, int att, int currhealth, int maxhealth, bool iscombatant, bool isenemy)
//    {
//        Name = name;
//        attack = att;
//        currentHealth = currhealth;
//        maxHealth = maxhealth;
//        isCombatant = iscombatant;
//        isEnemy = isenemy;
//    }
//}

public class GameStatsManager : MonoBehaviour
{
    public static GameStatsManager Instance { get; private set;}
    public _PartyManager _partyManager;
    public PartyManager partyManager;
    public _BattleUIHandler _battleUIHandler;
    public _DialogueHandler _dialogueHandler;
    public GameObject nightFilter;

    // Combat-Related Stats
    //public Dictionary<string, CharacterStats> playerStats = new Dictionary<string, CharacterStats>
    //{
    //    { "Player", new CharacterStats("Player", 45, 150, 150, true, false)}
    //};
    public Survivor player;

    public int CampfireNPCInteractedWith = 0;

    public CharacterStats playerStats { get { return partyManager.getPlayer().GetCharStats(); } }

    public Dictionary<string, CharacterStats> allPartyMembers = new Dictionary<string, CharacterStats>
    {
        { "MemberA", new CharacterStats("MemberA", 35, 100, 100, true, false)},
        { "MemberB", new CharacterStats("MemberB", 7, 120, 120, false, false)},
        { "MemberC", new CharacterStats("MemberC", 5, 150, 150, false, false)},
        { "MemberD", new CharacterStats("MemberD", 25, 210, 210, true, false)},
        { "MemberE", new CharacterStats("MemberE", 42, 170, 170, true, false)}
    };
    public Dictionary<string, CharacterStats> L1Enemies = new Dictionary<string, CharacterStats>
    {
        { "Handy", new CharacterStats("Handy", 18, 170, 170, true, true)},
        { "Gregor", new CharacterStats("Gregor", 16, 180, 180, true, true)},
        { "Cuboid", new CharacterStats("Cuboid", 15, 150, 150, true, true)}
    };

    public List<CampfireExitDialogue> allBeds = new List<CampfireExitDialogue>();


    public Dictionary<string, CharacterStats> L2AEnemies = new Dictionary<string, CharacterStats>
    {

    };
    public Dictionary<string, CharacterStats> L2BEnemies = new Dictionary<string, CharacterStats>
    {

    };
    public List<CharacterStats> currentPlayerStats = new List<CharacterStats>();
    //
    //
    //public List<CharacterStats> currentPartyMembers = new List<CharacterStats>();
    public List<CharacterStats> currentPartyMembers { get { return partyManager.getStats(); } }
    public List<Survivor> currentSurvivors { get { return partyManager.currentPartyMembers; } }
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
            //Debug.Log($"Name: {stats.Name}, Attack: {stats.attack}, HP: {stats.currentHealth}/{stats.maxHealth}, Combatant: {stats.isCombatant}, Enemy: {stats.isEnemy}");
        }
    }


    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else if (Instance != this) {Destroy(gameObject);}
        //DontDestroyOnLoad(gameObject);    Don't need this while in title screen

        currStamina = maxStamina;
        sprintLocked = false;

        _partyManager = GetComponentInChildren<_PartyManager>();
        _battleUIHandler = GetComponentInChildren<_BattleUIHandler>();
        _dialogueHandler = GetComponentInChildren<_DialogueHandler>();

        staminaBar = GameObject.FindGameObjectWithTag("Stamina Bar").GetComponent<Image>();

        currentPlayerStats.Add (playerStats);
    }
    public void Start()
    {
        nightFilter = GameObject.FindGameObjectWithTag("NightFilter");
        nightFilter.SetActive(false);

    }
    public void Update()
    {
        staminaBar.fillAmount = currStamina/maxStamina;

        // Test dialogue without NPC
        // You need a game object with
        // 1. DialogueBoxHandler
        // 2. NPCDialogueHandler
        // 3. A dialogue Script
        if (Input.GetKeyDown(KeyCode.T)) {
            _dialogueHandler.OpenDialogueWith(GameObject.Find("TestDialogue"));
        }
    }
    public void resetNpcCounter() {
        CampfireNPCInteractedWith = 0;    }
    public void interactedWithCampfireNPC() {
        CampfireNPCInteractedWith++;
    }
    public void updateBedStatus() {
        //call every beggining of feed and after every fed i think

        //bool highlightBed = true;
        //foreach (Survivor survivor in partyManager.currentPartyMembers) {
        //    if (!survivor.Fed) {
        //        highlightBed = false;
        //    }

        //}
        //Inventory inventory = GameObject.FindGameObjectWithTag("Player").GetComponent<Inventory>();
        //if (!inventory.hasItemByName("Ration")) {
        //    highlightBed = true;

        //}

        bool highlightBed = false;
        if (CampfireNPCInteractedWith == partyManager.currentPartyMembers.Count-1) {
            highlightBed = true;
        }

        if (highlightBed) {
            foreach (CampfireExitDialogue bed in allBeds) {
                bed.highlightBedSprite();

            }
        } else{
            foreach (CampfireExitDialogue bed in allBeds) {
                bed.deselectBedSprite();


            }
        }
    }
}
