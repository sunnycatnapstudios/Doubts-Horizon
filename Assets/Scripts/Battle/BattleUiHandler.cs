using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using Random = UnityEngine.Random;

public class CharStats {
    public string Name;
    public int Attack;
    public int Health;
    public bool IsEnemy;

    public CharStats(string name, int attack, int health, bool type) {
        Name = name;
        Attack = attack;
        Health = health;
        IsEnemy = type;
    }
}

public class BattleUiHandler : MonoBehaviour {
    public Animator partyUIAnimator, enemyUIAnimator, enemyStatsAnimator;
    public bool actOption = false, itemOption = false, canSelect = false;
    public GameObject overworldUI, combatUI, actOptionBList, itemOptionBList, enemySlot;

    // public List<PartySlot> partySlots;
    public List<GameObject> partySlots = new List<GameObject>();
    public PartyManager partyManager;

    public List<CharStats> battleOrder = new List<CharStats>();
    public int currentTurnIndex = 0;
    private bool battleInProgress = false;

    public Dictionary<string, CharStats> L1Enemies =
        new Dictionary<string, CharStats> //i think these should also be prefabs
        {
            { "Handy", new CharStats("Handy", 38, 100, true) },
            { "Gregor", new CharStats("Gregor", 21, 120, true) },
            { "Turf", new CharStats("Turf", 31, 150, true) }
        };

    public Dictionary<string, CharStats> L2Enemies = new Dictionary<string, CharStats>();
    public List<CharStats> currentEnemies = new List<CharStats>();
    public List<Sprite> profileImages;
    public TurnIndicator turnIndicator;
    public GameObject floatingTextPrefab;
    public TextMeshProUGUI enemyName;
    private string selectedAction = "", selectedTarget = null;
    CharStats enemyStats;
    public TextMeshProUGUI damageButtonText;

    [Serializable]
    private struct AudioClips {
        public AudioClip battleMusic;
        [HideInInspector] public AudioClip oldAmbience; // Use to swap back to old scene
        [HideInInspector] public AudioClip oldMusic; // Use to swap back to old scene
        public AudioClip sfxBell;
        public AudioClip sfxSwing;
        public AudioClip uiSelect;
        public AudioClip uiOpenDrawer;
        public AudioClip uiCloseDrawer;
    }

    [SerializeField] private AudioClips audioClips;

    void OnEnable() {
        partyUIAnimator.updateMode = AnimatorUpdateMode.UnscaledTime;
        enemyUIAnimator.updateMode = AnimatorUpdateMode.UnscaledTime;
        enemyStatsAnimator.updateMode = AnimatorUpdateMode.UnscaledTime;

        currentEnemies = new List<CharStats>(L1Enemies.Values);

        StartCoroutine(StartBattle());
    }

    public IEnumerator StartBattle() {
        battleOrder.Clear();
        partyManager = GameObject.FindGameObjectWithTag("Player").GetComponent<PartyManager>();
        Survivor player = GameObject.FindGameObjectWithTag("Player").GetComponent<PartyManager>().getPlayer();
        Debug.Log(player);
        CharStats playerStats = new CharStats(player.Name, player.Damage, player.Health, false);
        //CharStats playerStats = new CharStats("player.Name", 21, 321, false);
        battleOrder.Add(playerStats);
        partySlots[0].GetComponent<PartySlot>().Name = player.Name;
        partySlots[0].GetComponent<PartySlot>().SetHealth(player.CurHealth);
        partySlots[0].GetComponent<PartySlot>().profile.sprite = player.GetSprite();

        int slotIndex = 1;
        foreach (var member in partyManager.currentPartyMembers) {
            if (member.IsCombatant) {
                CharStats
                    newChar = new CharStats(member.Name, member.Damage, member.Health,
                        false); //probably need to change abit to be able to use current health as well as max health
                battleOrder.Add(newChar);

                if (slotIndex < partySlots.Count) {
                    // Assign correct profile image
                    // partySlots[slotIndex].profile.sprite = member.profileSprite; // Ensure PartyMember has profileSprite
                    partySlots[slotIndex].GetComponent<PartySlot>().Name = member.Name;
                    partySlots[slotIndex].GetComponent<PartySlot>().SetHealth(member.Health);
                    partySlots[slotIndex].GetComponent<PartySlot>().profile.sprite =
                        member.GetSprite(); //can create add/create other sprites and getters if want to use different sprite
                }

                slotIndex++;
            }
        }

        enemyStats = currentEnemies[Random.Range(0, currentEnemies.Count)];
        enemyName.text = ":" + enemyStats.Name;
        enemySlot.GetComponent<EnemyHealthbar>().SetHealth(enemyStats.Health);
        battleOrder.Add(enemyStats);

        battleOrder = ShuffleList(battleOrder);


        foreach (var Char in battleOrder) {
            Debug.Log($"{Char.Name}: {Char.Attack} Damage, {Char.Health} HP");
        }

        battleInProgress = true;
        turnIndicator.SetupTurnIndicator(battleOrder.Count);

        // Switch to new music
        audioClips.oldAmbience = AudioManager.Instance.AmbienceCurrentClip;
        audioClips.oldMusic = AudioManager.Instance.MusicCurrentClip;

        AudioManager.Instance.PlayUiSound(audioClips.sfxBell);
        AudioManager.Instance.CrossFadeAmbienceToZero(1f);
        AudioManager.Instance.CrossFadeMusicSound(audioClips.battleMusic, 2f, 1f, 1f);

        yield return new WaitForSecondsRealtime(2.0f);
        StartCoroutine(TurnLoop());
    }

    public List<CharStats> ShuffleList(List<CharStats> list) {
        for (int i = 0; i < list.Count; i++) {
            int randomIndex = Random.Range(0, list.Count);
            (list[i], list[randomIndex]) = (list[randomIndex], list[i]);
        }

        return list;
    }

    private IEnumerator TurnLoop() {
        while (battleInProgress) {
            if (currentTurnIndex >= battleOrder.Count) {
                currentTurnIndex = 0; // Loop back to the first combatant
            }

            CharStats currentCombatant = battleOrder[currentTurnIndex];

            Debug.Log($"It is {currentCombatant.Name}'s turn!");

            if (currentCombatant.IsEnemy) // Enemy turn
            {
                yield return EnemyTurn(currentCombatant);
            } else if (!currentCombatant.IsEnemy) // Player turn
            {
                damageButtonText.text = $"Attack:{currentCombatant.Attack}";
                yield return PlayerTurn(currentCombatant);
            }

            // Check if the battle is over
            if (CheckForBattleEnd()) {
                Debug.Log("Battle has ended!");

                enemyUIAnimator.Play("Handy Stop");
                EndEncounter();
                battleInProgress = false;
                yield break;
            }

            currentTurnIndex++;
        }
    }

    private IEnumerator PlayerTurn(CharStats player) {
        Debug.Log($"{player.Name}'s turn. Choose an action!");

        selectedAction = null;
        while (selectedAction == null) {
            yield return null;
        }

        while (selectedAction == "Heal" || selectedAction == "Defend") {
            canSelect = true;
            selectedTarget = null;

            while (selectedTarget == null) {
                yield return null;

                // If the action is changed mid-selection, restart decision phase
                if (selectedAction == "Attack") {
                    Debug.Log("Action switched to Attack. Restarting action selection...");
                    break; // Go back to the start of the loop
                }
            }

            // If we broke out of the loop due to switching to Attack, restart the process
            if (selectedAction == "Attack") continue;

            Debug.Log($"Target chosen: {selectedTarget}");

            if (selectedAction == "Defend") {
                Debug.Log($"{selectedTarget} is protected by {player.Name}!");
            } else if (selectedAction == "Heal") {
                CharStats healTarget = battleOrder.Find(member => member.Name == selectedTarget);
                int healAmount = Random.Range(30, 50);
                healTarget.Health += healAmount;

                foreach (GameObject mem in partySlots) {
                    if (mem.GetComponent<PartySlot>().Name == healTarget.Name) {
                        if (healTarget.Health > mem.GetComponent<PartySlot>().maxHealth) {
                            healTarget.Health = (int)mem.GetComponent<PartySlot>().maxHealth;
                        }

                        mem.GetComponent<PartySlot>().UpdateHealthBar(healTarget.Health);
                        mem.GetComponent<PartySlot>().ShowHealthChange();
                        ShowFloatingText(healAmount, Color.green, mem.transform.position, true);
                    }
                }

                Debug.Log($"{selectedTarget} was healed by {player.Name} for {healAmount} HP!");
            }

            canSelect = false;
            break; // Move forward in the turn after completing Heal/Defend
        }

        if (selectedAction == "Attack") {
            int playerDamage = (int)Random.Range(player.Attack * .8f, player.Attack * 1.6f);
            enemyStats.Health -= playerDamage;
            AudioManager.Instance.PlayUiSound(audioClips.sfxSwing);
            Debug.Log($"{player.Name} attacks {enemyStats.Name} for {playerDamage} damage!");

            ShowFloatingText(playerDamage, Color.red, (enemySlot.transform.position + (new Vector3(-80f, 0f, 0f))),
                false);

            if (enemyStats.Health <= 0) {
                Debug.Log($"{enemyStats.Name} has been defeated!");
                battleOrder.Remove(enemyStats);
            }
        }

        // if (selectedAction == "Defend" || selectedAction == "Heal")
        // {
        //     canSelect = true;

        //     selectedTarget = null;
        //     while (selectedTarget == null)
        //     {
        //         yield return null;

        //         if (selectedAction == "Attack")
        //         {
        //             Debug.Log("Action switched to Attack. Restarting turn...");
        //             StartCoroutine(PlayerTurn(player));
        //             yield break; // Stop current coroutine
        //         }
        //     }

        //     Debug.Log($"Target chosen: {selectedTarget}");

        //     if (selectedAction == "Defend")
        //     {
        //         Debug.Log($"{selectedTarget} is protected by {player.Name}!");
        //     }
        //     if (selectedAction == "Heal")
        //     {
        //         CharStats healTarget = battleOrder.Find(member => member.Name == selectedTarget);
        //         int healAmount = Random.Range(30, 50);
        //         healTarget.Health+=100;

        //         foreach (GameObject mem in partySlots) {
        //             if (mem.GetComponent<PartySlot>().Name == healTarget.Name)
        //                 {
        //                     if (healTarget.Health > mem.GetComponent<PartySlot>().maxHealth) {
        //                         healTarget.Health = (int)mem.GetComponent<PartySlot>().maxHealth;
        //                         mem.GetComponent<PartySlot>().UpdateHealthBar(healTarget.Health);
        //                         Debug.Log($"HEALTH EXCEEDED THE MAX, CURRENT HEALTH IS NOW {healTarget.Health}");
        //                     } else {
        //                         mem.GetComponent<PartySlot>().UpdateHealthBar(healTarget.Health);
        //                     }
        //                 }
        //             }
        //         Debug.Log($"{selectedTarget} was healed by {player.Name} for {healAmount} HP!");
        //     }
        //     canSelect = false;
        // }

        enemySlot.GetComponent<EnemyHealthbar>().UpdateHealthBar(enemyStats.Health);
        foreach (GameObject mem in partySlots) {
            if (mem.GetComponent<PartySlot>().Name == player.Name) {
                mem.GetComponent<PartySlot>().UpdateHealthBar(player.Health);
            }
        }

        Debug.Log($"Player chose {selectedAction}");
        yield return new WaitForSecondsRealtime(.5f);
    }

    private IEnumerator EnemyTurn(CharStats enemy) {
        yield return new WaitForSecondsRealtime(1f);
        // Select a random target from the player's party
        List<CharStats> playerParty = battleOrder.FindAll(c => !c.IsEnemy); // Exclude Will from selection
        if (playerParty.Count > 0) {
            CharStats target = playerParty[Random.Range(0, playerParty.Count)];
            Survivor guyGettingHit;
            if (target.Name == partyManager.getPlayer().Name) {
                guyGettingHit = partyManager.getPlayer();
            } else {
                guyGettingHit = partyManager.currentPartyMembers.Find(c => c.Name == target.Name);
            }

            // Simulate attack
            int enemyDamage = (int)Random.Range(enemy.Attack * .6f, enemy.Attack * 1.2f);
            Debug.Log($"{enemy.Name} attacks {target.Name} for {enemyDamage} damage!");
            partyManager.TakeDamage(guyGettingHit, enemyDamage);
            //target.Health -= enemyDamage;
            foreach (GameObject mem in partySlots) {
                if (mem.GetComponent<PartySlot>().Name == target.Name) {
                    mem.GetComponent<PartySlot>().UpdateHealthBar(target.Health);
                    mem.GetComponent<PartySlot>().ShowHealthChange();
                    ShowFloatingText(enemyDamage, Color.red, mem.transform.position, false);
                    StartCoroutine(mem.GetComponent<PartySlot>().JutterHealthBar(0.2f, 10f));
                }
            }

            // Check if target is defeated
            Debug.Log(guyGettingHit.ToString());
            if (guyGettingHit.Health <= 0) {
                Debug.Log($"{target.Name} has been defeated!");
                battleOrder.Remove(target);
            }
        }

        yield return new WaitForSecondsRealtime(.3f);
    }

    public void ReceiveTargetSelection(string targetName) {
        selectedTarget = targetName;
        Debug.Log($"Target selected: {selectedTarget}");
    }

    private IEnumerator WaitForTargetSelection(System.Action<string> callback) {
        string selectedTarget = null;
        while (string.IsNullOrEmpty(selectedTarget)) {
            yield return null;
        }

        callback(selectedTarget); // Return the selected name
    }


    void ShowFloatingText(int damage, Color color, Vector3 targetTransform, bool ishealing) {
        Vector3 spawnPosition = targetTransform + new Vector3(0, 20f, 0);
        GameObject floatingText = Instantiate(floatingTextPrefab, spawnPosition, Quaternion.identity,
            GameObject.FindGameObjectWithTag("Combat UI").transform);
        floatingText.SetActive(true);
        floatingText.GetComponent<DamageIndicator>().SetText(damage.ToString(), color);
        floatingText.GetComponent<DamageIndicator>().isHealing = ishealing;
        floatingText.GetComponent<DamageIndicator>().textMesh.color = color;
    }


    private bool CheckForBattleEnd() {
        bool playersAlive = battleOrder.Exists(c => !c.IsEnemy);
        bool enemiesAlive = battleOrder.Exists(c => c.IsEnemy);

        return !playersAlive || !enemiesAlive;
        // return false;
    }

    // Called when the battle should end. Use to transition back to overworld
    private void EndEncounter() {
        if (partyUIAnimator != null) {
            partyUIAnimator.ResetTrigger("Open");
            partyUIAnimator.ResetTrigger("Close");
            partyUIAnimator.ResetTrigger("Reset");

            if (itemOption || actOption) {
                partyUIAnimator.SetTrigger("Close");
                actOption = itemOption = false;
            }
        }

        actOptionBList.SetActive(actOption);
        itemOptionBList.SetActive(itemOption);
        overworldUI.SetActive(true);
        combatUI.SetActive(false);

        turnIndicator.ClearTurnIndicators();

        // Switch back to original sounds
        AudioManager.Instance.CrossFadeAmbienceSound(audioClips.oldAmbience, 1f, 1f, 1f);
        AudioManager.Instance.CrossFadeMusicSound(audioClips.oldMusic, 1f, 1f, 1f);

        Time.timeScale = 1;
    }

    public void OnActionButtonPressed(string action) {
        AudioManager.Instance.PlayUiSound(audioClips.uiSelect);

        if (selectedAction == "Heal" || selectedAction == "Defend") {
            selectedTarget = null;
            canSelect = false;
        }

        selectedAction = action;
        Debug.Log("Current Action: " + selectedAction);
    }

    public void Act() {
        if (partyUIAnimator != null) {
            partyUIAnimator.ResetTrigger("Open");
            partyUIAnimator.ResetTrigger("Close");
            partyUIAnimator.ResetTrigger("Reset");

            if (actOption) {
                partyUIAnimator.SetTrigger("Close");
                AudioManager.Instance.PlayUiSound(audioClips.uiCloseDrawer);
                actOption = false;
            } else if (itemOption) {
                partyUIAnimator.SetTrigger("Reset");
                AudioManager.Instance.PlayUiSound(audioClips.uiCloseDrawer);
                itemOption = false;
                actOption = true;
            } else {
                partyUIAnimator.SetTrigger("Open");
                AudioManager.Instance.PlayUiSound(audioClips.uiOpenDrawer);
                actOption = true;
            }

            // actOptionBList.SetActive(actOption);
            StartCoroutine(WaitForCloseThenToggle(actOptionBList, actOption));
            itemOptionBList.SetActive(itemOption);
        }
    }

    public void Item() {
        if (partyUIAnimator != null) {
            partyUIAnimator.ResetTrigger("Open");
            partyUIAnimator.ResetTrigger("Close");
            partyUIAnimator.ResetTrigger("Reset");

            if (itemOption) {
                partyUIAnimator.SetTrigger("Close");
                AudioManager.Instance.PlayUiSound(audioClips.uiCloseDrawer);
                itemOption = false;
            } else if (actOption) {
                partyUIAnimator.SetTrigger("Reset");
                AudioManager.Instance.PlayUiSound(audioClips.uiCloseDrawer);
                actOption = false;
                itemOption = true;
            } else {
                partyUIAnimator.SetTrigger("Open");
                AudioManager.Instance.PlayUiSound(audioClips.uiOpenDrawer);
                itemOption = true;
            }

            actOptionBList.SetActive(actOption);
            // itemOptionBList.SetActive(itemOption);
            StartCoroutine(WaitForCloseThenToggle(itemOptionBList, itemOption));
        }
    }

    public void Escape() {
        AudioManager.Instance.PlayUiSound(audioClips.uiSelect);
        EndEncounter();
    }

    private IEnumerator WaitForCloseThenToggle(GameObject targetContent, bool state) {
        AnimatorStateInfo stateInfo = partyUIAnimator.GetCurrentAnimatorStateInfo(0);

        // while (stateInfo.IsName("Slot Closed") && stateInfo.normalizedTime < 1.0f || stateInfo.IsName("Reset"))
        // {
        //     yield return null; // Wait for next frame
        //     stateInfo = partyUIAnimator.GetCurrentAnimatorStateInfo(0); // Update state info

        //     if (stateInfo.IsName("Slot Open")) {break;}
        // }
        if (!state) {
            while (!stateInfo.IsName("Slot Closed") || stateInfo.normalizedTime < 1.0f) {
                yield return null;
                stateInfo = partyUIAnimator.GetCurrentAnimatorStateInfo(0);
            }
        } else // If opening, activate immediately when "Slot Open" starts
        {
            while ((stateInfo.IsName("Slot Closed") && stateInfo.normalizedTime < 1.0f) || stateInfo.IsName("Reset")) {
                yield return null;
                stateInfo = partyUIAnimator.GetCurrentAnimatorStateInfo(0);

                if (stateInfo.IsName("Slot Open")) {
                    break;
                }
            }
        }

        targetContent.SetActive(state);
    }
}
