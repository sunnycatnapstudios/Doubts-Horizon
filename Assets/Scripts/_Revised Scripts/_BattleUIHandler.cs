using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
// using UnityEngine.UIElements;
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
    public GameObject floatingText;

    private int healAmount;
    public List<string> defeatedInCombat = new List<string>();
    public string endCause;
    public bool endTurn;
    public PartySlotHandler partySlotHandler;
    public RectTransform defendIndicator;

    [Serializable]
    private struct AudioClips {
        public AudioClip battleMusic;
        [HideInInspector] public AudioClip oldAmbience;      // Use to swap back to old scene
        [HideInInspector] public AudioClip oldMusic;         // Use to swap back to old scene
        public AudioClip sfxBell;
        public AudioClip sfxSwing;
        public AudioClip uiSelect;
        public AudioClip uiOpenDrawer;
        public AudioClip uiCloseDrawer;
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

        currentEnemies = new List<CharacterStats>(gameStatsManager.L1Enemies.Values);
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
                partySlotHandler = combatUI.GetComponentInChildren<PartySlotHandler>();
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
            } if (obj.CompareTag("Defend Indicator")) {
                defendIndicator = obj.GetComponent<RectTransform>();
            } // if (obj.CompareTag("")) {

            // } if (obj.CompareTag("")) {

            // }

            // if (combatUI != null && overworldUI != null && enemySlot != null) {break;}
        }

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
        endCause = "";
        endTurn = false;
        defendIndicator.SetParent(partyUIAnimator.transform, false); defendIndicator.SetSiblingIndex(0);
        defendIndicator.anchoredPosition = new Vector2(-2000, 48);
        battleOrder.Clear();
        // Survivor player = GameObject.FindGameObjectWithTag("Player").GetComponent<PartyManager>().getPlayer();
            // Debug.Log(player);
        CharacterStats player = gameStatsManager.playerStats["Player"];
        // CharStats playerStats = new CharStats(player.Name, player.Damage, player.Health, false);
        //CharStats playerStats = new CharStats("player.Name", 21, 321, false);
        battleOrder.Add(player);

        // partySlots[0].GetComponent<PartySlot>().Name = player.Name;
        // partySlots[0].GetComponent<PartySlot>().SetHealth(player.currentHealth, player.maxHealth);
        // partySlots[0].GetComponent<PartySlot>().profile.sprite = _partyManager.characterProfiles.Find(image => image.name == player.Name);

        int slotIndex = 1;
        foreach (var member in gameStatsManager.currentPartyMembers)
        {
            // if (member.isCombatant)
            {
                battleOrder.Add(member);

                // if (slotIndex < partySlots.Count)
                // {
                //     // Assign correct profile image
                //     partySlots[slotIndex].GetComponent<PartySlot>().Name = member.Name;
                //     partySlots[slotIndex].GetComponent<PartySlot>().SetHealth(member.currentHealth, member.maxHealth);
                //     partySlots[slotIndex].GetComponent<PartySlot>().profile.sprite = _partyManager.characterProfiles.Find(image => image.name == member.Name);
                // }
                slotIndex++;
            }
        }

        enemySlot.GetComponent<EnemyHealthbar>().SetHealth(enemyStats.currentHealth);
        battleOrder.Add(enemyStats);
        playerParty = battleOrder.FindAll(c => !c.isEnemy); // Exclude Enemy from selection
        partySlotHandler.UpdateSlots();

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

            GameObject simpleUIAnimator = GameObject.FindGameObjectWithTag("Simple Enemy");
            enemyUIAnimator = simpleUIAnimator.GetComponent<Animator>();
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
    void EnemyAttack() {;}
    void EnemyGotAttacked() {;}

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
                EndEncounter(endCause);
                battleInProgress = false;
                yield break;
            }

            currentTurnIndex++;
            endTurn = false;

        }
    }
    void SpawnDefendIndicator(CharacterStats player)
    {
        PartySlot targetSlot = partySlotHandler.partySlots.Find(slot => slot.playerStats == player);
        defendIndicator.sizeDelta = new Vector2(25, 25);
        defendIndicator.GetComponent<DefendIndicator>().inAnimation = true;
        defendIndicator.GetComponent<DefendIndicator>().isAssigned = true;
        defendIndicator.GetComponent<DefendIndicator>().parentRectTransform = partySlotHandler.GetComponent<RectTransform>();

        if (targetSlot != null)
        {
            defendIndicator.SetParent(targetSlot.transform, false);
            Vector2 targetPos = new Vector2(35, 15);

            defendIndicator.localScale = Vector3.zero;
            defendIndicator.anchoredPosition = targetPos;

            StartCoroutine(ZoomIntoFrame(defendIndicator, targetPos, 0.5f));
        }
        else {Debug.LogWarning("No matching party slot");}
    }
    IEnumerator ZoomIntoFrame(RectTransform indicator, Vector2 targetAnchoredPosition, float duration)
    {
        Vector3 startScale = Vector3.zero;
        Vector3 endScale = Vector3.one;

        Vector2 startPos = indicator.anchoredPosition;
        Vector2 overshootPos = targetAnchoredPosition + new Vector2(15, -15); // Slight overshoot
        Vector2 endPos = targetAnchoredPosition;

        float elapsed = 0f;

        while (elapsed < duration)
        {
            elapsed += Time.unscaledDeltaTime;
            float t = elapsed / duration;

            // Apply ease-out-back style overshoot curve
            t = Mathf.SmoothStep(0, 1, t);

            indicator.localScale = Vector3.Lerp(startScale, endScale, t);

            // First move towards the overshoot position
            if (t < 0.7f)
                indicator.anchoredPosition = Vector2.Lerp(startPos, overshootPos, t / 0.7f);
            else
                indicator.anchoredPosition = Vector2.Lerp(overshootPos, endPos, (t - 0.7f) / 0.3f);

            yield return null;
        }

        // Ensure it lands precisely at the end position
        indicator.localScale = endScale;
        indicator.anchoredPosition = endPos;

        defendIndicator.GetComponent<DefendIndicator>().inAnimation = false;
    }
    IEnumerator ShakeDefendIndicator(float duration, float strength)
    {
        defendIndicator.GetComponent<DefendIndicator>().inAnimation = true;

        Vector2 startPos = defendIndicator.anchoredPosition;
        float elapsedTime = 0f;

        while (elapsedTime < duration)
        {
            float jutterAmount = Mathf.Sin(elapsedTime * 30f) * strength;  // Small oscillations
            defendIndicator.anchoredPosition = startPos + new Vector2(jutterAmount, 0f); // Jutter left/right

            elapsedTime += Time.unscaledDeltaTime;
            yield return null; // Wait for next frame
        }

        defendIndicator.anchoredPosition = startPos; // Return to original position after jutter

        defendIndicator.GetComponent<DefendIndicator>().inAnimation = false;
    }
    IEnumerator DestroyDefend()
    {
        defendIndicator.GetComponent<DefendIndicator>().inAnimation = true;
        float jitterDuration = 0.3f;
        float expandDuration = 0.5f;
        float jitterAmount = 3f;
        float elapsed = 0f;

        Vector2 originalPos = defendIndicator.anchoredPosition;

        while (elapsed < jitterDuration)
        {
            elapsed += Time.unscaledDeltaTime;
            float offsetX = Random.Range(-jitterAmount, jitterAmount);
            float offsetY = Random.Range(-jitterAmount, jitterAmount);
            defendIndicator.anchoredPosition = originalPos + new Vector2(offsetX, offsetY);
            yield return new WaitForSecondsRealtime(0.05f);
        }
        defendIndicator.anchoredPosition = originalPos;

        elapsed = 0f;
        Vector2 originalScale = defendIndicator.sizeDelta;
        Vector2 alteredScale = new Vector2(150, 150);

        // Expand and fade out
        while (elapsed < expandDuration)
        {
            elapsed += Time.unscaledDeltaTime;
            float t = elapsed / expandDuration;

            // Scale up
            defendIndicator.sizeDelta = Vector3.Lerp(originalScale, alteredScale, t);

            // Fade out
            Color color = defendIndicator.GetComponent<Image>().color;
            color.a = Mathf.Lerp(1f, 0f, t);
            defendIndicator.GetComponent<Image>().color = color;

            yield return null;
        }
        defendIndicator.SetParent(partyUIAnimator.transform, false); defendIndicator.SetSiblingIndex(0);
        defendIndicator.anchoredPosition = new Vector2(-2000, 48);
        defendIndicator.GetComponent<Image>().color = Color.white;
        defendIndicator.GetComponent<DefendIndicator>().inAnimation = false;
        defendIndicator.GetComponent<DefendIndicator>().isAssigned = false;
    }

    private IEnumerator PlayerTurn(CharacterStats player)
    {
        Debug.Log($"{player.Name}'s turn. Choose an action!");
        partySlotHandler.MoveToActivePlayer(player, false);

        selectedAction = null;
        while (selectedAction == null)
        {
            yield return null;
        }

        if (endTurn) {yield break;}

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
                    Debug.Log("Action switched");
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
                    SpawnDefendIndicator(player);
                    Debug.Log($"{player.Name} chose to defend the Party!!!");
                    canSelect = false;
                }
                else
                {
                    StartCoroutine(ShakeDefendIndicator(Random.Range(.2f, .4f), Random.Range(20f, 30f)));
                    Debug.Log("There's already someone defending :(");

                    selectedAction = null;
                    selectedTarget = null;
                    continue;
                }
                // selectedTarget = player.Name;
            }
            else if (selectedAction == "Heal")
            {
                CharacterStats healTarget = battleOrder.Find(member => member.Name == selectedTarget);

                if (player.isCombatant)  {healAmount = (1+(60 - player.attack)/60)*(Random.Range(20, 40));}
                else {healAmount = Random.Range(20, 40);}

                if (selectedTarget == player.Name) {healAmount=(int)(healAmount*.666f); Debug.Log("Pretty greedy to try healing yourself");}
                healTarget.currentHealth += healAmount;

                foreach (PartySlot mem in partySlotHandler.partySlots)
                {
                    if (mem.playerStats.Name == healTarget.Name)
                    {
                        if (healTarget.currentHealth > mem.playerStats.maxHealth)
                        {
                            healTarget.currentHealth = healTarget.maxHealth;
                        }
                        mem.ShowHealthChange();
                        ShowFloatingText(healAmount, Color.green, mem.transform.position, true);
                    }
                }

                Debug.Log($"{selectedTarget} was healed by {player.Name} for {healAmount} HP!");
                canSelect = false;
            }
            break; // Move forward in the turn after completing Heal/Defend
        }

        if (selectedAction == "Attack")
        {
            // Desperate Attack: Scales with current health
            int desperateDamage = (int)(player.attack * (1+((player.maxHealth-player.currentHealth)/player.maxHealth*.7f)));
            int playerDamage = (int)(Random.Range(desperateDamage*.8f, desperateDamage*1.6f));

            currentEnemyCurrentHealth -= playerDamage;
            Debug.Log($"{player.Name} attacks {enemyStats.Name} for {playerDamage} damage!");

            AudioManager.Instance.PlayUiSound(audioClips.sfxSwing);
            ShowFloatingText(playerDamage, Color.red, (enemySlot.transform.position+(new Vector3(-80f,0f,0f))), false);

            if (currentEnemyCurrentHealth <= 0)
            {
                Debug.Log($"{enemyStats.Name} has been defeated!");
                endCause = "Lose";
                battleOrder.Remove(enemyStats);
            }
        }

        enemySlot.GetComponent<EnemyHealthbar>().UpdateHealthBar(currentEnemyCurrentHealth);

        Debug.Log($"Player chose {selectedAction}");
        yield return new WaitForSecondsRealtime(.5f);
    }

    private IEnumerator EnemyTurn(CharacterStats enemy)
    {
        Debug.Log($"Enemy's Turn: {enemy.Name}");
        yield return new WaitForSecondsRealtime(1f);

        if (playerParty.Count > 0)
        {
            CharacterStats target = playerParty[Random.Range(0, playerParty.Count)];
            partySlotHandler.MoveToActivePlayer(target, true);
            int enemyDamage = (int)Random.Range(enemy.attack * 0.6f, enemy.attack * 1.2f);

            if (currentDefender != null && currentDefender != target) // If there's an active defender
            {
                // Calculate negation percentage
                float healthRatio = currentDefender.currentHealth / (float)currentDefender.maxHealth;
                float negationPercentage = 0.4f * healthRatio * (currentDefender.attack / 100f); // Adjust 100f for balance
                negationPercentage = Mathf.Clamp(negationPercentage, 0f, 0.4f); // Ensure within 0-40%

                // Calculate negated damage
                int damageNegated = (int)(enemyDamage * negationPercentage);
                int remainingDamage = enemyDamage - damageNegated;

                // Calculate defender's share of the remaining damage (50% - 100%)
                float defenderRatio = 0.5f + 0.5f * healthRatio;
                int defenderDamage = (int)(remainingDamage * defenderRatio);
                int targetDamage = remainingDamage - defenderDamage;

                // Apply damage
                currentDefender.currentHealth -= defenderDamage;
                target.currentHealth -= targetDamage;

                Debug.Log($"{enemy.Name} attacks {target.Name}, but is blocked by {currentDefender.Name}!" +
                        $"\nDamage Negated: {damageNegated}" +
                        $"\n{currentDefender.Name} takes {defenderDamage}, {target.Name} takes {targetDamage}");

                // // Update Defender's Health UI
                // foreach (GameObject mem in partySlots)
                // {
                //     if (mem.GetComponent<PartySlot>().Name == currentDefender.Name)
                //     {
                //         mem.GetComponent<PartySlot>().UpdateHealthBar(currentDefender.currentHealth);
                //         mem.GetComponent<PartySlot>().ShowHealthChange();
                //         ShowFloatingText(defenderDamage, Color.blue, mem.transform.position, false);
                //         StartCoroutine(mem.GetComponent<PartySlot>().JutterHealthBar(0.2f, 10f));
                //     }
                // }
                foreach (PartySlot mem in partySlotHandler.partySlots)
                {
                    if (mem.playerStats.Name == currentDefender.Name)
                    {
                        mem.ShowHealthChange();
                        ShowFloatingText(defenderDamage, Color.blue, mem.transform.position, false);
                        StartCoroutine(mem.JutterHealthBar(0.2f, 10f));
                    }
                }

                // // Update Target's Health UI
                // foreach (GameObject mem in partySlots)
                // {
                //     if (mem.GetComponent<PartySlot>().Name == target.Name)
                //     {
                //         mem.GetComponent<PartySlot>().UpdateHealthBar(target.currentHealth);
                //         mem.GetComponent<PartySlot>().ShowHealthChange();
                //         ShowFloatingText(targetDamage, Color.red, mem.transform.position, false);
                //         StartCoroutine(mem.GetComponent<PartySlot>().JutterHealthBar(0.2f, 10f));
                //     }
                // }
                foreach (PartySlot mem in partySlotHandler.partySlots)
                {
                    if (mem.playerStats.Name == target.Name)
                    {
                        mem.ShowHealthChange();
                        ShowFloatingText(targetDamage, Color.red, mem.transform.position, false);
                        StartCoroutine(mem.JutterHealthBar(0.2f, 10f));
                    }
                }

                // Check if defender is defeated
                if (currentDefender.currentHealth <= 0)
                {
                    Debug.Log($"{currentDefender.Name} has been defeated!");
                    defeatedInCombat.Add(currentDefender.Name);
                    battleOrder.Remove(currentDefender);
                    currentDefender = null;
                }
            }
            else // No defender, target takes full damage
            {
                target.currentHealth -= enemyDamage;
                Debug.Log($"{enemy.Name} attacks {target.Name} for {enemyDamage} damage!");

                foreach (PartySlot mem in partySlotHandler.partySlots)
                {
                    if (mem.playerStats.Name == target.Name)
                    {
                        mem.ShowHealthChange();
                        ShowFloatingText(enemyDamage, Color.red, mem.transform.position, false);
                        StartCoroutine(mem.JutterHealthBar(0.2f, 10f));
                    }
                }
            }

            // Check if target is defeated
            if (target.currentHealth <= 0)
            {
                Debug.Log($"{target.Name} has been defeated!");
                defeatedInCombat.Add(target.Name);
                battleOrder.Remove(target);
            }

            // Reset defender at the end of the turn
            currentDefender = null;
            StartCoroutine(DestroyDefend());
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
        Vector3 spawnPosition = targetTransform + new Vector3(0, 60f, 0);
        floatingText = Instantiate(floatingTextPrefab, spawnPosition, Quaternion.identity, GameObject.FindGameObjectWithTag("Combat UI").transform);
        floatingText.SetActive(true);
        floatingText.GetComponent<DamageIndicator>().SetText(damage.ToString(), color);
        floatingText.GetComponent<DamageIndicator>().isHealing = ishealing;
        floatingText.GetComponent<DamageIndicator>().textMesh.color = color;
    }


    private bool CheckForBattleEnd()
    {
        bool playersAlive = battleOrder.Exists(c => !c.isEnemy);
        bool enemiesAlive = battleOrder.Exists(c => c.isEnemy);

        endCause = playersAlive? "Natural": "Lose";

        return !playersAlive || !enemiesAlive;
        // return false;
    }

    // Called when the battle should end. Use to transition back to overworld
    private void EndEncounter(string reason)
    {
        if (reason == "Natural")
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
            if (floatingText != null)
            {
                Destroy(floatingText);
            }

            actOptionBList.SetActive(actOption);
            itemOptionBList.SetActive(itemOption);
            overworldUI.SetActive(true);
            combatUI.SetActive(false);

            turnIndicator.ClearTurnIndicators();

            // Switch back to original sounds
            AudioManager.Instance.CrossFadeAmbienceSound(audioClips.oldAmbience, 1f, 1f, 1f);
            AudioManager.Instance.CrossFadeMusicSound(audioClips.oldMusic, 1f, 1f, 1f);

            battleInProgress = false;
            Time.timeScale = 1;
        }
    }

    public void OnActionButtonPressed(string action)
    {
        AudioManager.Instance.PlayUiSound(audioClips.uiSelect);

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
                AudioManager.Instance.PlayUiSound(audioClips.uiCloseDrawer);
                actOption = false;
            } else if (itemOption) {
                partyUIAnimator.SetTrigger("Reset");
                AudioManager.Instance.PlayUiSound(audioClips.uiCloseDrawer);
                itemOption = false;
                actOption = true;
            }
            else {
                partyUIAnimator.SetTrigger("Open");
                AudioManager.Instance.PlayUiSound(audioClips.uiOpenDrawer);
                actOption = true;
            }
            // actOptionBList.SetActive(actOption);
            StartCoroutine(WaitForCloseThenToggle(actOptionBList, actOption));
            itemOptionBList.SetActive(itemOption);
        }
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
                AudioManager.Instance.PlayUiSound(audioClips.uiCloseDrawer);
                itemOption = false;
            } else if (actOption) {
                partyUIAnimator.SetTrigger("Reset");
                AudioManager.Instance.PlayUiSound(audioClips.uiCloseDrawer);
                actOption = false;
                itemOption = true;
            }
            else {
                partyUIAnimator.SetTrigger("Open");
                AudioManager.Instance.PlayUiSound(audioClips.uiOpenDrawer);
                itemOption = true;
            }

            actOptionBList.SetActive(actOption);
            // itemOptionBList.SetActive(itemOption);
            StartCoroutine(WaitForCloseThenToggle(itemOptionBList, itemOption));
        }
    }
    public void Escape()
    {
        selectedAction = "Escape";

        AudioManager.Instance.PlayUiSound(audioClips.uiSelect);

        float totalCurrentHealth = 0f, totalMaxHealth = 0f;
        int partyCount = 0;

        foreach (CharacterStats partyMember in battleOrder)
        {
            if (!partyMember.isEnemy)
            {
                partyCount++;
                totalCurrentHealth += partyMember.currentHealth;
                totalMaxHealth += partyMember.maxHealth;
            }
        }
        int escapeChance = (int)(35+(((totalMaxHealth-totalCurrentHealth)/totalMaxHealth)*65));
        int roll = Random.Range(0, 100);

        Debug.Log($"Chance of escape: {escapeChance}%");

        if (roll<= escapeChance)
        {
            Debug.Log($"Wow, rolled a {roll}, you made it!!!");
            endCause = "Natural";
            EndEncounter(endCause);
        }
        else
        {
            Debug.Log($"Oof, rolled a {roll}, didn't make it lol");
            SkipTurns();
        }


        // EndEncounter();
    }
    void SkipTurns()
    {
        int enemyIndex = battleOrder.IndexOf(enemyStats);
        endTurn = true;
        currentTurnIndex = enemyIndex-1;
    }

    private IEnumerator WaitForCloseThenToggle(GameObject targetContent, bool state)
    {
        AnimatorStateInfo stateInfo = partyUIAnimator.GetCurrentAnimatorStateInfo(0);

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
