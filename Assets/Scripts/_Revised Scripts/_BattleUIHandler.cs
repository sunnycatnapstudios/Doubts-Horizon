using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using UnityEngine.UIElements;
using Button = UnityEngine.UIElements.Button;
using TMPro;
using Random = UnityEngine.Random;

public class _BattleUIHandler : MonoBehaviour
{
    private GameStatsManager gameStatsManager;
    private _PartyManager _partyManager;
    
    public Animator partyUIAnimator, enemyUIAnimator, enemyStatsAnimator;
    public bool actOption = false, itemOption = false, canSelect = false;
    public GameObject overworldUI, combatUI;
    public GameObject actOptionBList, itemOptionBList, enemySlot, floatingTextPrefab;
    public List<GameObject> partySlots = new List<GameObject>();
    public List<CharacterStats> battleOrder = new List<CharacterStats>();
    public int currentTurnIndex = 0;
    private bool battleInProgress = false;
    public List<CharacterStats> currentEnemies = new List<CharacterStats>();
    public TurnIndicator turnIndicator;
    public TextMeshProUGUI enemyName, damageButtonText;
    public string selectedAction = "", selectedTarget = null;
    public CharacterStats enemyStats;
    public int currentEnemyCurrentHealth, currentEnemyMaxHealth;
    public List<CharacterStats> playerParty;

    private CharacterStats currentDefender = null;
    public bool isDefending;

    [Serializable]
    private struct AudioClips {
        public AudioClip battleMusic;
        [HideInInspector] public AudioClip oldAmbience;      // Use to swap back to old scene
        [HideInInspector] public AudioClip oldMusic;         // Use to swap back to old scene
        public AudioClip sfxBell;
        public AudioClip uiSelected;
        public AudioClip uiUnselected;
        public AudioClip uiDrawer;
    }
    [SerializeField] private AudioClips audioClips;

    void Awake()
    {
        StartCoroutine(WaitForPartyManager());
    }

    IEnumerator WaitForPartyManager()
    {
        while (GameStatsManager.Instance == null || GameStatsManager.Instance._partyManager == null)
        {
            yield return null; // Wait until it's ready
        }

        gameStatsManager = GameStatsManager.Instance;
        _partyManager = gameStatsManager._partyManager;
    }
    void Start()
    {
        GameObject[] allObjects = Resources.FindObjectsOfTypeAll<GameObject>();
        foreach (GameObject obj in allObjects) {
            if (obj.CompareTag("Overworld UI")) {
                overworldUI = obj;
            } if (obj.CompareTag("Combat UI")) {
                combatUI = obj;
                turnIndicator = combatUI.GetComponentInChildren<TurnIndicator>();
            } if (obj.CompareTag("EnemyUI")) {
                enemySlot = obj;
                enemyStatsAnimator = obj.GetComponent<Animator>();
                enemyStatsAnimator.updateMode = AnimatorUpdateMode.UnscaledTime;
                enemyName = enemySlot.GetComponentInChildren<TextMeshProUGUI>();
            } if (obj.CompareTag("Act Options")) {
                actOptionBList = obj;
                damageButtonText = actOptionBList.GetComponentInChildren<TextMeshProUGUI>();
            } if (obj.CompareTag("Item Options")) {
                itemOptionBList = obj;
            } if (obj.CompareTag("PartyCombatUI")) {
                partyUIAnimator = obj.GetComponent<Animator>();
                partyUIAnimator.updateMode = AnimatorUpdateMode.UnscaledTime;
            } // if (obj.CompareTag("")) {

            // } if (obj.CompareTag("")) {

            // } if (obj.CompareTag("")) {

            // }
            
            // if (combatUI != null && overworldUI != null && enemySlot != null) {break;}
        }

        currentEnemies = new List<CharacterStats>(gameStatsManager.L1Enemies.Values);
        Debug.Log("MAKE SURE TO IMPLEMENT A METHOD FOR ENEMIES TO ASSIGN SPECIFIC LOCATIONS TO SPECIFIC ENEMY LISTS");
    }

    public void EnterCombat()
    {
        AudioManager.Instance.PlayUiSound(audioClips.sfxBell);
        AudioManager.Instance.CrossFadeAmbienceToZero(1f);
        AudioManager.Instance.CrossFadeMusicSound(audioClips.battleMusic, 2f, 1f, 1f);
        StartCoroutine(CaptureScreen());
        Time.timeScale = 0;
    }
    public IEnumerator CaptureScreen()
    {
        yield return new WaitForEndOfFrame();
        yield return null;

        Camera.main.Render();

        Texture2D screenTexture = ScreenCapture.CaptureScreenshotAsTexture();

        GameObject screenOverlay = new GameObject("ScreenOverlay");
        combatUI.SetActive(true);
        overworldUI.SetActive(false);

        SetupEnemy();

        screenOverlay.transform.SetParent(combatUI.transform, false);
        RawImage overlayImage = screenOverlay.AddComponent<RawImage>();
        overlayImage.texture = screenTexture;

        RectTransform overlayRect = screenOverlay.GetComponent<RectTransform>();
        overlayRect.sizeDelta = new Vector2(Screen.width, Screen.height);
        overlayRect.anchorMin = Vector2.zero;
        overlayRect.anchorMax = Vector2.one;
        overlayRect.pivot = new Vector2(0.5f, 0.5f);
        
        // Start the animation
        StartCoroutine(ZoomInAnimation(screenOverlay, overlayImage));
    }
    IEnumerator ZoomInAnimation(GameObject overlay, RawImage overlayImage)
    {
        RectTransform rect = overlay.GetComponent<RectTransform>();

        float initialScale = .8f;
        // Start with slightly zoomed out
        rect.localScale = new Vector3(initialScale, initialScale, 1f);

        yield return new WaitForSecondsRealtime(0.8f); // Short Delay before Zoom In

        StartCoroutine(StartBattle());

        float duration = 1.5f;  // Animation duration
        float time = 0f;
        
        Color startColor = overlayImage.color, targetColor = new Color(0, 0, 0, 0);  // Starting Color, Fully dark and transparent
        
        while (time < duration)
        {
            time += Time.unscaledDeltaTime;
            float t = time / duration;

            // Zoom in effect
            float scale = Mathf.Lerp(initialScale, 2.5f, t);
            rect.localScale = new Vector3(scale, scale, 1f);

            // Rotation effect
            float rotation = Mathf.Lerp(0f, 30f, t);
            rect.rotation = Quaternion.Euler(0, 0, rotation);

            // Darkening & Opacity fade-out
            overlayImage.color = Color.Lerp(startColor, targetColor, t);

            yield return null;
        }
        // Destroy object after animation completes
        Destroy(overlay);
    }

    public IEnumerator StartBattle()
    {
        battleOrder.Clear();
        // Survivor player = GameObject.FindGameObjectWithTag("Player").GetComponent<PartyManager>().getPlayer();
            // Debug.Log(player);
        CharacterStats player = gameStatsManager.playerStats["Player"];
        // CharStats playerStats = new CharStats(player.Name, player.Damage, player.Health, false);
        //CharStats playerStats = new CharStats("player.Name", 21, 321, false);
        battleOrder.Add(player);

        partySlots[0].GetComponent<PartySlot>().Name = player.Name;
        partySlots[0].GetComponent<PartySlot>().SetHealth(player.currentHealth, player.maxHealth);
        partySlots[0].GetComponent<PartySlot>().profile.sprite = _partyManager.characterProfiles.Find(image => image.name == player.Name);

        int slotIndex = 1;
        foreach (var member in gameStatsManager.currentPartyMembers)
        {
            if (member.isCombatant)
            {
                battleOrder.Add(member);

                if (slotIndex < partySlots.Count)
                {
                    // Assign correct profile image
                    partySlots[slotIndex].GetComponent<PartySlot>().Name = member.Name;
                    partySlots[slotIndex].GetComponent<PartySlot>().SetHealth(member.currentHealth, member.maxHealth);
                    partySlots[slotIndex].GetComponent<PartySlot>().profile.sprite = _partyManager.characterProfiles.Find(image => image.name == member.Name);
                }
                slotIndex++;
            }
        }

        enemySlot.GetComponent<EnemyHealthbar>().SetHealth(enemyStats.currentHealth);
        battleOrder.Add(enemyStats);

        battleOrder = ShuffleList(battleOrder);
        
       

        foreach (var Char in battleOrder)
        {
            Debug.Log($"{Char.Name}: {Char.attack} Damage, {Char.currentHealth} HP");
        }
        battleInProgress = true;
        turnIndicator.SetupTurnIndicator(battleOrder.Count);

        // Switch to new music
        audioClips.oldAmbience = AudioManager.Instance.AmbienceCurrentClip;
        audioClips.oldMusic = AudioManager.Instance.MusicCurrentClip;

        yield return new WaitForSecondsRealtime(2.0f);
        StartCoroutine(TurnLoop());
    }
    void SetupEnemy()
    {
        enemyStats = currentEnemies[Random.Range(0, currentEnemies.Count)];

        enemyName.text = ":"+enemyStats.Name;
        currentEnemyCurrentHealth = enemyStats.currentHealth;
        currentEnemyMaxHealth = enemyStats.maxHealth;

        if (enemyStats.Name == "Cuboid" || enemyStats.Name == "Handy")
        {
            // Debug.Log("ITS A SIMPLE ENEMY");
            GameObject[] allObjects = Resources.FindObjectsOfTypeAll<GameObject>();
            foreach (GameObject obj in allObjects) {
                if (obj.CompareTag("Simple Enemy")) {
                    obj.SetActive(true);
                } if (obj.CompareTag("Complex Enemy")) {
                    obj.SetActive(false);
                }
            }
            enemyUIAnimator = GameObject.Find("Simple Enemy").GetComponent<Animator>();
            enemyUIAnimator.updateMode = AnimatorUpdateMode.UnscaledTime;
            enemyUIAnimator.Play($"{enemyStats.Name} Idle");
        }
        else if (enemyStats.Name == "Gregor")
        {
            // Debug.Log("ITS A COMPLEX ENEMY");
            GameObject[] allObjects = Resources.FindObjectsOfTypeAll<GameObject>();
            foreach (GameObject obj in allObjects) {
                if (obj.CompareTag("Simple Enemy")) {
                    obj.SetActive(false);
                } if (obj.CompareTag("Complex Enemy")) {
                    obj.SetActive(obj.name == enemyStats.Name);
                }
            }
        }
    }
    public List<CharacterStats> ShuffleList(List<CharacterStats> list)
    {
        for (int i = 0; i < list.Count; i++)
        {
            int randomIndex = Random.Range(0, list.Count);
            (list[i], list[randomIndex]) = (list[randomIndex], list[i]);
        }
        return list;
    }

    private IEnumerator TurnLoop()
    {
        playerParty = battleOrder.FindAll(c => !c.isEnemy); // Exclude Enemy from selection
        
        while (battleInProgress)
        {
            if (currentTurnIndex >= battleOrder.Count)
            {
                currentTurnIndex = 0; // Loop back to the first combatant
            }

            CharacterStats currentCombatant = battleOrder[currentTurnIndex];

            Debug.Log($"It is {currentCombatant.Name}'s turn!");

            if (currentCombatant.isEnemy) // Enemy turn
            {
                yield return EnemyTurn(currentCombatant);
            }
            else if (!currentCombatant.isEnemy)// Player turn
            {
                damageButtonText.text = $"Attack:{currentCombatant.attack}";
                yield return PlayerTurn(currentCombatant);
            }

            // Check if the battle is over
            if (CheckForBattleEnd())
            {
                Debug.Log("Battle has ended!");

                // enemyUIAnimator.Play("Handy Stop");
                EndEncounter();
                battleInProgress = false;
                yield break;
            }

            currentTurnIndex++;

        }
    }
    private IEnumerator PlayerTurn(CharacterStats player)
    {
        Debug.Log($"{player.Name}'s turn. Choose an action!");

        selectedAction = null;
        while (selectedAction == null)
        {
            yield return null;
        }

        while (selectedAction == "Heal" || selectedAction == "Defend")
        {
            canSelect = true;
            selectedTarget = null;

            while (selectedTarget == null)
            {
                yield return null;

                // If the action is changed mid-selection, restart decision phase
                if (selectedAction == "Attack" || selectedAction == "Defend")
                {
                    Debug.Log("Action switched to Attack. Restarting action selection...");
                    break;  // Go back to the start of the loop
                }
            }

            // If we broke out of the loop due to switching to Another Option, restart the process
            if (selectedAction == "Attack") continue;

            Debug.Log($"Target chosen: {selectedTarget}");

            if (selectedAction == "Defend")
            {
                if (currentDefender == null)
                {
                    currentDefender = player;
                    Debug.Log($"{player.Name} chose to defend the Party!!!");
                }
                else {Debug.Log("There's already someone defending :(");}
                selectedTarget = player.Name;
            }
            else if (selectedAction == "Heal")
            {
                CharacterStats healTarget = battleOrder.Find(member => member.Name == selectedTarget);

                int healAmount = Random.Range(30, 50);
                if (selectedTarget == player.Name) {healAmount/=2; Debug.Log("Pretty greedy to try healing yourself");}
                healTarget.currentHealth += healAmount;

                foreach (GameObject mem in partySlots)
                {
                    if (mem.GetComponent<PartySlot>().Name == healTarget.Name)
                    {
                        if (healTarget.currentHealth > mem.GetComponent<PartySlot>().maxHealth)
                        {
                            healTarget.currentHealth = (int)mem.GetComponent<PartySlot>().maxHealth;
                        }
                        mem.GetComponent<PartySlot>().UpdateHealthBar(healTarget.currentHealth);
                        mem.GetComponent<PartySlot>().ShowHealthChange();
                        ShowFloatingText(healAmount, Color.green, mem.transform.position, true);
                    }
                }

                Debug.Log($"{selectedTarget} was healed by {player.Name} for {healAmount} HP!");
            }

            canSelect = false;
            break; // Move forward in the turn after completing Heal/Defend
        }

        if (selectedAction == "Attack")
        {
            // Desperate Attack: Scales with current health
            int desperateDamage = (int)(player.attack * (1+((player.maxHealth-player.currentHealth)/player.maxHealth*.7f)));
            int playerDamage = (int)(Random.Range(desperateDamage*.8f, desperateDamage*1.6f));

            currentEnemyCurrentHealth -= playerDamage;
            Debug.Log($"{player.Name} attacks {enemyStats.Name} for {playerDamage} damage!");

            ShowFloatingText(playerDamage, Color.red, (enemySlot.transform.position+(new Vector3(-80f,0f,0f))), false);

            if (currentEnemyCurrentHealth <= 0)
            {
                Debug.Log($"{enemyStats.Name} has been defeated!");
                battleOrder.Remove(enemyStats);
            }
        }

        enemySlot.GetComponent<EnemyHealthbar>().UpdateHealthBar(currentEnemyCurrentHealth);
        foreach (GameObject mem in partySlots) {
            if (mem.GetComponent<PartySlot>().Name == player.Name) {mem.GetComponent<PartySlot>().UpdateHealthBar(player.currentHealth);}
        }

        Debug.Log($"Player chose {selectedAction}");
        yield return new WaitForSecondsRealtime(.5f);
    }

    private IEnumerator EnemyTurn(CharacterStats enemy)
    {
        Debug.Log($"Enemy's Turn: {enemy.Name}");
        yield return new WaitForSecondsRealtime(1f);
        // Select a random target from the player's party
        
        if (playerParty.Count > 0)
        {
            CharacterStats target = playerParty[Random.Range(0, playerParty.Count)];
            // Survivor guyGettingHit;
            // if (target.Name == partyManager.getPlayer().Name) {

            //      guyGettingHit = partyManager.getPlayer();
            // } else {
            //      guyGettingHit = partyManager.currentPartyMembers.Find(c=>c.Name ==target.Name);
            // }

            // Simulate attack
            int enemyDamage = (int)Random.Range(enemy.attack*.6f, enemy.attack*1.2f);

            if (currentDefender != null && currentDefender != target) // Defender takes the hit
            {
                int defenderDamage = (int)(enemyDamage * Random.Range(0.5f, 0.9f));
                int remainingDamage = enemyDamage - defenderDamage;

                currentDefender.currentHealth -= defenderDamage;
                target.currentHealth -= remainingDamage;

                Debug.Log($"{enemy.Name} attacks {target.Name}, but is blocked by {currentDefender.Name}" +
                $"\n{currentDefender.Name} takes {defenderDamage}, while {target.Name} takes {remainingDamage}");

                foreach (GameObject mem in partySlots)
                {
                    if (mem.GetComponent<PartySlot>().Name == currentDefender.Name)
                    {
                        mem.GetComponent<PartySlot>().UpdateHealthBar(currentDefender.currentHealth);
                        mem.GetComponent<PartySlot>().ShowHealthChange();
                        ShowFloatingText(defenderDamage, Color.red, mem.transform.position, false);
                        StartCoroutine(mem.GetComponent<PartySlot>().JutterHealthBar(0.2f, 10f));
                    }
                }

                foreach (GameObject mem in partySlots)
                {
                    if (mem.GetComponent<PartySlot>().Name == target.Name)
                    {
                        mem.GetComponent<PartySlot>().UpdateHealthBar(target.currentHealth);
                        mem.GetComponent<PartySlot>().ShowHealthChange();
                        ShowFloatingText(remainingDamage, Color.red, mem.transform.position, false);
                        StartCoroutine(mem.GetComponent<PartySlot>().JutterHealthBar(0.2f, 10f));
                    }
                }

                if (currentDefender.currentHealth <= 0)
                {
                    Debug.Log($"{currentDefender.Name} has been defeated!");
                    battleOrder.Remove(currentDefender);
                }
            }
            else
            {
                target.currentHealth -= enemyDamage;
                Debug.Log($"{enemy.Name} attacks {target.Name} for {enemyDamage} damage!");
                
                foreach (GameObject mem in partySlots)
                {
                    if (mem.GetComponent<PartySlot>().Name == target.Name)
                    {
                        mem.GetComponent<PartySlot>().UpdateHealthBar(target.currentHealth);
                        mem.GetComponent<PartySlot>().ShowHealthChange();
                        ShowFloatingText(enemyDamage, Color.red, mem.transform.position, false);
                        StartCoroutine(mem.GetComponent<PartySlot>().JutterHealthBar(0.2f, 10f));
                    }
                }
            }
            
            
            // Check if target is defeated
            if (target.currentHealth <= 0)
            {
                Debug.Log($"{target.Name} has been defeated!");
                battleOrder.Remove(target);
            }
            currentDefender = null; // Reset Current Defender
        }
        yield return new WaitForSecondsRealtime(.3f);
    }

    public void ReceiveTargetSelection(string targetName)
    {
        selectedTarget = targetName;
        Debug.Log($"Target selected: {selectedTarget}");
    }
    private IEnumerator WaitForTargetSelection(System.Action<string> callback)
    {
        string selectedTarget = null;
        while (string.IsNullOrEmpty(selectedTarget))
        {
            yield return null;
        }
        callback(selectedTarget); // Return the selected name
    }


    void ShowFloatingText(int damage, Color color, Vector3 targetTransform, bool ishealing)
    {
        Vector3 spawnPosition = targetTransform + new Vector3(0, 20f, 0);
        GameObject floatingText = Instantiate(floatingTextPrefab, spawnPosition, Quaternion.identity, GameObject.FindGameObjectWithTag("Combat UI").transform);
        floatingText.SetActive(true);
        floatingText.GetComponent<DamageIndicator>().SetText(damage.ToString(), color);
        floatingText.GetComponent<DamageIndicator>().isHealing = ishealing;
        floatingText.GetComponent<DamageIndicator>().textMesh.color = color;
    }


    private bool CheckForBattleEnd()
    {
        bool playersAlive = battleOrder.Exists(c => !c.isEnemy);
        bool enemiesAlive = battleOrder.Exists(c => c.isEnemy);

        return !playersAlive || !enemiesAlive;
        // return false;
    }

    // Called when the battle should end. Use to transition back to overworld
    private void EndEncounter()
    {
        if (partyUIAnimator != null)
        {
            partyUIAnimator.ResetTrigger("Open");
            partyUIAnimator.ResetTrigger("Close");
            partyUIAnimator.ResetTrigger("Reset");

            if (itemOption||actOption) {
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

    public void OnActionButtonPressed(string action)
    {
        AudioManager.Instance.PlayUiSound(audioClips.uiSelected);

        if (selectedAction == "Heal" || selectedAction == "Defend"){
            selectedTarget = null;
            canSelect = false;
        }

        selectedAction = action;
        Debug.Log("Current Action: " + selectedAction);
    }

    public void Act()
    {
        if (partyUIAnimator != null)
        {
            partyUIAnimator.ResetTrigger("Open");
            partyUIAnimator.ResetTrigger("Close");
            partyUIAnimator.ResetTrigger("Reset");

            if (actOption) {
                partyUIAnimator.SetTrigger("Close");
                actOption = false;
            } else if (itemOption) {
                partyUIAnimator.SetTrigger("Reset");
                itemOption = false;
                actOption = true;
            }
            else {
                partyUIAnimator.SetTrigger("Open");
                actOption = true;
            }
            // actOptionBList.SetActive(actOption);
            StartCoroutine(WaitForCloseThenToggle(actOptionBList, actOption));
            itemOptionBList.SetActive(itemOption);
        }

        AudioManager.Instance.PlayUiSound(audioClips.uiDrawer);
    }
    public void Item()
    {
        if (partyUIAnimator != null)
        {
            partyUIAnimator.ResetTrigger("Open");
            partyUIAnimator.ResetTrigger("Close");
            partyUIAnimator.ResetTrigger("Reset");

            if (itemOption) {
                partyUIAnimator.SetTrigger("Close");
                itemOption = false;
            } else if (actOption) {
                partyUIAnimator.SetTrigger("Reset");
                actOption = false;
                itemOption = true;
            }
            else {
                partyUIAnimator.SetTrigger("Open");
                itemOption = true;
            }

            actOptionBList.SetActive(actOption);
            // itemOptionBList.SetActive(itemOption);
            StartCoroutine(WaitForCloseThenToggle(itemOptionBList, itemOption));
        }
        AudioManager.Instance.PlayUiSound(audioClips.uiDrawer);
    }
    public void Escape()
    {
        AudioManager.Instance.PlayUiSound(audioClips.uiDrawer);
        EndEncounter();
    }

    private IEnumerator WaitForCloseThenToggle(GameObject targetContent, bool state)
    {
        AnimatorStateInfo stateInfo = partyUIAnimator.GetCurrentAnimatorStateInfo(0);

        // while (stateInfo.IsName("Slot Closed") && stateInfo.normalizedTime < 1.0f || stateInfo.IsName("Reset"))
        // {
        //     yield return null; // Wait for next frame
        //     stateInfo = partyUIAnimator.GetCurrentAnimatorStateInfo(0); // Update state info

        //     if (stateInfo.IsName("Slot Open")) {break;}
        // }
        if (!state)
        {
            while (!stateInfo.IsName("Slot Closed") || stateInfo.normalizedTime < 1.0f)
            {
                yield return null;
                stateInfo = partyUIAnimator.GetCurrentAnimatorStateInfo(0);
            }
        } else // If opening, activate immediately when "Slot Open" starts
        {
            while ((stateInfo.IsName("Slot Closed") && stateInfo.normalizedTime < 1.0f) || stateInfo.IsName("Reset"))
            {
                yield return null;
                stateInfo = partyUIAnimator.GetCurrentAnimatorStateInfo(0);

                if (stateInfo.IsName("Slot Open")) {break;}
            }
        }
        targetContent.SetActive(state);
    }
}
