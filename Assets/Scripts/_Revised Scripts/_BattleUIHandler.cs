using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using UnityEditor;
// using UnityEngine.UIElements;
using Button = UnityEngine.UIElements.Button;
using TMPro;
using Random = UnityEngine.Random;

public class _BattleUIHandler : MonoBehaviour
{
    private GameStatsManager gameStatsManager;
    private _PartyManager _partyManager;
    private _DialogueHandler _dialogueHandler;
    private PartyManager partyManager;
    private Inventory _inventory;

    public Animator partyUIAnimator, enemyUIAnimator, enemyStatsAnimator;
    public bool actOption = false, itemOption = false, canSelect = false;
    public GameObject overworldUI, combatUI;
    public GameObject actOptionBList, itemOptionBList, enemySlot, floatingTextPrefab;
    public List<GameObject> partySlots = new List<GameObject>();
    public List<CharacterStats> battleOrder = new List<CharacterStats>();
    public int currentTurnIndex = 0;
    private bool battleInProgress = false, escapeSuccessful = false;
    public List<CharacterStats> currentEnemies = new List<CharacterStats>();
    public TurnIndicator turnIndicator;
    public TextMeshProUGUI enemyName, damageButtonText, healButtonText;
    public string selectedAction = "", selectedTarget = null;
    public CharacterStats enemyStats;
    public int currentEnemyCurrentHealth, currentEnemyMaxHealth;
    public List<CharacterStats> playerParty;
    public List<Survivor> survivors;
    public EnemyObjectManager enemyObjectManager;

    [System.NonSerialized]
    public CharacterStats currentDefender = null;
    public GameObject floatingText;
    public GameObject curEnemy = null;
    private TMP_Text battleExplanation = null;

    private int healAmount, numKits;
    public List<string> defeatedInCombat = new List<string>();
    public string endCause;
    public bool endTurn;
    public PartySlotHandler partySlotHandler;
    public RectTransform defendIndicator;
    private BattleTransition battleTransition;
    private DefendStatusIcon defendStatusIcon;

    public int roll, escapeChance;
    public EscapePrompt escapePrompt;

    [Serializable]
    private struct AudioClips {
        public AudioClip battleMusic;
        [HideInInspector] public AudioClip oldAmbience;      // Use to swap back to old scene
        [HideInInspector] public AudioClip oldMusic;         // Use to swap back to old scene
        public AudioClip battlePlayerDied;
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
        while (GameStatsManager.Instance == null || GameStatsManager.Instance.partyManager == null)
        {
            yield return null; // Wait until it's ready
        }

        gameStatsManager = GameStatsManager.Instance;
        _partyManager = gameStatsManager._partyManager;
        partyManager = gameStatsManager.partyManager;
        _dialogueHandler = gameStatsManager._dialogueHandler;

        currentEnemies = new List<CharacterStats>(gameStatsManager.L1Enemies.Values);
    }
    void Start()
    {
        // GameObject[] gameObjects;
        // gameObjects = FindObjectsOfType<GameObject>(true);

        var allObjects = GetAllObjectsOnlyInScene();

        foreach (GameObject obj in allObjects) {
            if (obj.CompareTag("Overworld UI")) {
                overworldUI = obj;
            } if (obj.CompareTag("Combat UI")) {
                combatUI = obj;
                turnIndicator = combatUI.GetComponentInChildren<TurnIndicator>();
                partySlotHandler = combatUI.GetComponentInChildren<PartySlotHandler>();
                defendStatusIcon = combatUI.GetComponentInChildren<DefendStatusIcon>();
                escapePrompt = combatUI.GetComponentInChildren<EscapePrompt>();
                battleExplanation = partySlotHandler.GetComponentInChildren<TMP_Text>();
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
                healButtonText = itemOptionBList.GetComponentInChildren<TextMeshProUGUI>(); // TODO
            } if (obj.CompareTag("PartyCombatUI")) {
                partyUIAnimator = obj.GetComponent<Animator>();
                partyUIAnimator.updateMode = AnimatorUpdateMode.UnscaledTime;
            } if (obj.CompareTag("Defend Indicator")) {
                defendIndicator = obj.GetComponent<RectTransform>();
            } if (obj.CompareTag("Damage Numbers")) {
                floatingTextPrefab = obj;
            } if (obj.GetComponent<BattleTransition>() != null) {
                battleTransition = obj.GetComponent<BattleTransition>();
            } if (obj.GetComponent<Inventory>() != null) {
                _inventory = obj.GetComponent<Inventory>();
            }

            // if (combatUI != null && overworldUI != null && enemySlot != null) {break;}
        }

        // MAKE SURE TO IMPLEMENT A METHOD FOR ENEMIES TO ASSIGN SPECIFIC LOCATIONS TO SPECIFIC ENEMY LISTS
    }

    public List<GameObject> GetAllObjectsOnlyInScene()
    {
        List<GameObject> objectsInScene = new List<GameObject>();

        foreach (GameObject go in Resources.FindObjectsOfTypeAll(typeof(GameObject)) as GameObject[])
        {
#if UNITY_EDITOR
            if (!UnityEditor.EditorUtility.IsPersistent(go.transform.root.gameObject)
                && !(go.hideFlags == HideFlags.NotEditable || go.hideFlags == HideFlags.HideAndDontSave))
#endif
            objectsInScene.Add(go);
        }

        return objectsInScene;
    }

    public void SetBossBattleMusic(AudioClip audioClip) {
        audioClips.battleMusic = audioClip;
    }

    public void EnterCombat()
    {
        _dialogueHandler.CloseDialogueBox();
        AudioManager.Instance.PlayUiSound(audioClips.sfxBell);
        AudioManager.Instance.CrossFadeAmbienceToZero(1f);
        AudioManager.Instance.CrossFadeMusicSound(audioClips.battleMusic, 2f, 1f, 1f);
        battleExplanation.text = "";
        Slot healthKit = _inventory.GetSlotItem("Health Kit");
        if (healthKit != null) {
            numKits = healthKit.getCount();
        }
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
        Canvas.ForceUpdateCanvases();

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
        currentDefender = null;
        battleOrder.Clear();
        currentTurnIndex = 0; //how did we forget this sadge

        //CharacterStats player = gameStatsManager.playerStats;

        //battleOrder.Add(player);

        int slotIndex = 1;
        foreach (CharacterStats member in gameStatsManager.currentPartyMembers)
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
        survivors = partyManager.currentPartyMembers;
        //playerParty = battleOrder.FindAll(c => !c.isEnemy); // Exclude Enemy from selection
        playerParty = partyManager.getStats();


        partySlotHandler.UpdateSlots();
        SetEscapePercentage();

        int attempts = 0;
        do {
            battleOrder = ShuffleList(battleOrder);
            attempts++;
        } while (battleOrder[currentTurnIndex].isEnemy && attempts < 100);

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
        enemyStats = new CharacterStats(enemyObjectManager.Name, enemyObjectManager.Damage, enemyObjectManager.Health, enemyObjectManager.Health,true, true);
        bool complex = enemyObjectManager.complexEnemy;
        enemyName.text = ":"+enemyStats.Name;
        currentEnemyCurrentHealth = enemyStats.currentHealth;
        currentEnemyMaxHealth = enemyStats.maxHealth;

        if (!complex)
        {
            // Debug.Log("ITS A SIMPLE ENEMY");
            var allObjects = GetAllObjectsOnlyInScene();
            foreach (GameObject obj in allObjects) {
                if (obj.CompareTag("Simple Enemy")) {
                    obj.SetActive(true);
                    if(enemyStats.Name == "Hector") {
                        obj.transform.localScale = new Vector3(2.5f,2.5f,1);
                    } else{
                        obj.transform.localScale = Vector3.one;
                    }
                    enemyUIAnimator = obj.GetComponent<Animator>();
                } if (obj.CompareTag("Complex Enemy")) {
                    obj.SetActive(false);
                }
            }

            // GameObject simpleUIAnimator = GameObject.FindGameObjectWithTag("Simple Enemy");
            // enemyUIAnimator = simpleUIAnimator.GetComponent<Animator>();
            enemyUIAnimator.updateMode = AnimatorUpdateMode.UnscaledTime;
            enemyUIAnimator.Play($"{enemyStats.Name} Idle");
            //enemyUIAnimator.Play(enemyObjectManager.BattleAnimeController.)
        }
        else if (complex)
        {
            // Debug.Log("ITS A COMPLEX ENEMY");
            var allObjects = GetAllObjectsOnlyInScene();
            foreach (GameObject obj in allObjects) {
                if (obj.CompareTag("Simple Enemy")) {
                    obj.SetActive(false);
                } if (obj.CompareTag("Complex Enemy")) {
                    Debug.Log("Setting complex enemy to "+ enemyStats.Name);
                    obj.SetActive(obj.name == enemyStats.Name);
                }
            }
        }
    }
    void EnemyIsAttacking(string enemyName)
    {
        switch (enemyName)
        {
            case "Gregor":
                Eyeball gregorController = GameObject.Find("Gregor").GetComponent<Eyeball>();
                gregorController.Attack();
                break;
            // case:
            //     Debug.Log("No attacked animation"); break;
        }
    }
    void EnemyGotAttacked(string enemyName)
    {
        switch (enemyName)
        {
            case "Gregor":
                Eyeball gregorController = GameObject.Find("Gregor").GetComponent<Eyeball>();
                gregorController.Hit();
                break;
            // case:
            //     Debug.Log("No attacked animation"); break;
        }
    }
    void EnemyDefeated(string enemyName)
    {
        Debug.Log($"{enemyStats.Name} was Defeated");
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
        while (battleInProgress)
        {
            yield return new WaitForSecondsRealtime(.3f);
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
                healButtonText.text = $"Health Kit:{numKits}";
                yield return PlayerTurn(currentCombatant);
            }

            // Check if the battle is over
            if (CheckForBattleEnd())
            {
                if (endCause == "Win" || endCause == "Escape") {
                    yield return new WaitForSecondsRealtime(1.5f);
                    battleTransition.LeaveBattle();
                    yield return new WaitForSecondsRealtime(.5f);
                } else if (endCause == "Lose") {
                    yield return new WaitForSecondsRealtime(.5f);
                    battleTransition.HadDied();
                }

                EndEncounter(endCause);
                battleInProgress = false;
                yield break;
            }
            SetEscapePercentage();
            currentTurnIndex++;
            endTurn = false;

        }
    }
    private bool runningAnimation = false;

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

            StartCoroutine(WaitForAnimationAndPlay(() => ZoomIntoFrame(defendIndicator, targetPos, 0.5f)));
            // StartCoroutine(defendStatusIcon.SlideIn());
        }
        else { Debug.LogWarning("No matching party slot"); }
    }

    IEnumerator WaitForAnimationAndPlay(Func<IEnumerator> newAnimation)
    {
        while (runningAnimation) yield return null; // Wait if an animation is running

        runningAnimation = true;
        yield return StartCoroutine(newAnimation()); // Run the new animation
        runningAnimation = false;
    }

    IEnumerator ZoomIntoFrame(RectTransform indicator, Vector2 targetAnchoredPosition, float duration)
    {
        StartCoroutine(defendStatusIcon.SlideIn());
        Vector3 startScale = Vector3.zero;
        Vector3 endScale = Vector3.one;

        Vector2 startPos = indicator.anchoredPosition;
        Vector2 overshootPos = targetAnchoredPosition + new Vector2(15, -15); // Slight overshoot
        Vector2 endPos = targetAnchoredPosition;

        float elapsed = 0f;

        while (elapsed < duration)
        {
            elapsed += Time.unscaledDeltaTime;
            float t = Mathf.SmoothStep(0, 1, elapsed / duration);

            indicator.localScale = Vector3.Lerp(startScale, endScale, t);

            if (t < 0.7f)
                indicator.anchoredPosition = Vector2.Lerp(startPos, overshootPos, t / 0.7f);
            else
                indicator.anchoredPosition = Vector2.Lerp(overshootPos, endPos, (t - 0.7f) / 0.3f);

            yield return null;
        }

        indicator.localScale = endScale;
        indicator.anchoredPosition = endPos;

        defendIndicator.GetComponent<DefendIndicator>().inAnimation = false;
    }

    IEnumerator ShakeDefendIndicator()
    {
        yield return StartCoroutine(WaitForAnimationAndPlay(() => ShakeDefendInternal(Random.Range(.2f, .4f), Random.Range(5f, 20f))));
    }

    IEnumerator ShakeDefendInternal(float duration, float strength)
    {
        defendIndicator.GetComponent<DefendIndicator>().inAnimation = true;
        StartCoroutine(defendStatusIcon.ShakeDefendIcon(.3f, .8f));
        Vector2 startPos = defendIndicator.anchoredPosition;
        float elapsedTime = 0f;

        while (elapsedTime < duration)
        {
            float jitterAmount = Mathf.Sin(elapsedTime * 30f) * strength;
            defendIndicator.anchoredPosition = startPos + new Vector2(jitterAmount, 0f);

            elapsedTime += Time.unscaledDeltaTime;
            yield return null;
        }

        defendIndicator.anchoredPosition = startPos;
        defendIndicator.GetComponent<DefendIndicator>().inAnimation = false;
    }

    IEnumerator DestroyDefend()
    {
        defendIndicator.GetComponent<DefendIndicator>().isAssigned = false;
        yield return StartCoroutine(WaitForAnimationAndPlay(() => DestroyDefendInternal()));
    }

    IEnumerator DestroyDefendInternal()
    {
        defendIndicator.GetComponent<DefendIndicator>().inAnimation = true;
        StartCoroutine(defendStatusIcon.SlideOut());
        float jitterDuration = 0.1f;
        float expandDuration = 0.3f;
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

        while (elapsed < expandDuration)
        {
            elapsed += Time.unscaledDeltaTime;
            float t = elapsed / expandDuration;

            defendIndicator.sizeDelta = Vector3.Lerp(originalScale, alteredScale, t);

            Color color = defendIndicator.GetComponent<Image>().color;
            color.a = Mathf.Lerp(1f, 0f, t);
            defendIndicator.GetComponent<Image>().color = color;

            yield return null;
        }

        defendIndicator.SetParent(partyUIAnimator.transform, false);
        defendIndicator.SetSiblingIndex(0);
        defendIndicator.anchoredPosition = new Vector2(-2000, 48);
        defendIndicator.GetComponent<Image>().color = Color.white;
        defendIndicator.GetComponent<DefendIndicator>().inAnimation = false;
        defendIndicator.GetComponent<DefendIndicator>().isAssigned = false;

        // Update party slot
        partySlotHandler.UpdateSlots();
    }

    private IEnumerator PlayerTurn(CharacterStats player) {
        battleExplanation.text = "Choose an action!";
        partySlotHandler.MoveToActivePlayer(player, false);
        partySlotHandler.ViewPortCanvasGroup.blocksRaycasts = true;

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

            if (selectedAction == "Heal") {
                // Check if we have health kit in inventory
                if (numKits > 0) {
                    partySlotHandler.ViewPortCanvasGroup.blocksRaycasts = false;
                    canSelect = true;
                    battleExplanation.text = "Select a survivor to heal.";
                } else {
                    // Reject prompt
                    battleExplanation.text = "You are out of Health Kits";
                }
            }

            while (selectedTarget == null)
            {
                yield return null;

                // If the action is changed mid-selection, restart decision phase
                if (selectedAction == "Attack" || selectedAction == "Defend" || selectedAction == "Escape")
                {

                    partySlotHandler.ViewPortCanvasGroup.blocksRaycasts = true;
                    //Debug.Log("Action switched");
                    if (selectedAction == "Attack" || selectedAction == "Escape") break;  // Go back to the start of the loop

                    //Debug.Log($"Current Action: {selectedAction}");
                    if (selectedAction == "Defend" && currentDefender == null)
                    {
                        break;
                    }
                    else if (selectedAction == "Defend" && currentDefender != null)
                    {
                        partySlotHandler.MoveToActivePlayer(currentDefender, false);
                        StartCoroutine(ShakeDefendIndicator());
                        selectedAction = selectedTarget = null;
                    }
                }
            }

            // If we broke out of the loop due to switching to Another Option, restart the process
            if (selectedAction == "Attack" || selectedAction == "Escape") continue;

            if (selectedAction == "Defend")
            {
                if (currentDefender == null)
                {
                    currentDefender = player;
                    SpawnDefendIndicator(player);
                    partySlotHandler.MoveToActivePlayer(player, false);

                    // Debug.Log($"{player.Name} chose to defend the Party!!!");
                    canSelect = false;
                    selectedTarget = player.Name;
                    break;
                }
                else
                {
                    // StartCoroutine(ShakeDefendIndicator());
                    // Debug.Log("There's already someone defending :(");
                    // partySlotHandler.MoveToActivePlayer(currentDefender, false);

                    selectedAction = null;
                    selectedTarget = null;
                    continue;
                }
            }
            else if (selectedAction == "Heal")
            {
                CharacterStats healTarget = battleOrder.Find(member => member.Name == selectedTarget);

                partySlotHandler.ViewPortCanvasGroup.blocksRaycasts = false;

                if (player.isCombatant)  {healAmount = (1+(60 - player.attack)/60)*(Random.Range(20, 40));}
                else {healAmount = player.maxHealth / 2;}

                _inventory.removeItemByName("Health Kit"); // Use a health kit
                numKits--;

                if (selectedTarget == player.Name) {
                    healAmount=(int)(healAmount*.8f);
                    //Debug.Log($"Pretty greedy to try healing yourself {healAmount/.4f}:{healAmount}");
                }
                healTarget.currentHealth += healAmount;
                partySlotHandler.MoveToActivePlayer(healTarget, true);

                foreach (PartySlot mem in partySlotHandler.partySlots)
                {
                    if (mem.isCharacter) {
                        if (mem.playerStats.Name == healTarget.Name)
                        {
                            if (healTarget.currentHealth > mem.playerStats.maxHealth)
                            {
                                healTarget.currentHealth = healTarget.maxHealth;
                            }
                            mem.HealHealthBar();
                            ShowFloatingText(healAmount, Color.green, mem.transform.position, true);
                        }
                    }
                }

                // Debug.Log($"{selectedTarget} was healed by {player.Name} for {healAmount} HP!");
                // canSelect = false;
                break; // Move forward in the turn after completing Heal/Defend
            }
        }

        if (selectedAction == "Attack")
        {
            // Desperate Attack: Scales with current health
            int desperateDamage = (int)(player.attack * (1+((player.maxHealth-player.currentHealth)/player.maxHealth*.7f)));
            int playerDamage = (int)(Random.Range(desperateDamage*.8f, desperateDamage*1.6f));

            currentEnemyCurrentHealth -= playerDamage;
            Debug.Log($"{player.Name} attacks {enemyStats.Name} for {playerDamage} damage!");

            ShowFloatingText(playerDamage, Color.red, (enemySlot.transform.position+(new Vector3(-80f,0f,0f))), false);
            EnemyGotAttacked(enemyStats.Name);

            if (currentEnemyCurrentHealth <= 0)
            {
                Debug.Log($"{enemyStats.Name} has been defeated!");
                endCause = "Win";
                battleOrder.Remove(enemyStats);
            }
        }

        enemySlot.GetComponent<EnemyHealthbar>().UpdateHealthBar(currentEnemyCurrentHealth);
        partySlotHandler.ViewPortCanvasGroup.blocksRaycasts = true;
        canSelect = false;

        //Debug.Log($"Player chose {selectedAction}");
        yield return new WaitForSecondsRealtime(.6f);
    }

    private IEnumerator EnemyTurn(CharacterStats enemy) {
        battleExplanation.text = "Enemy turn.";

        partyUIAnimator.SetTrigger("Close");
        actOption = false;
        itemOption = false;

        yield return new WaitForSecondsRealtime(1f);

        if (playerParty.Count > 0)
        {
            bool defenderWasAttacked = false;

            CharacterStats target = playerParty[Random.Range(0, playerParty.Count)];
            int enemyDamage = (int)Random.Range(enemy.attack * 0.6f, enemy.attack * 1.2f);

            if (currentDefender != null) // If there's an active defender
            {
                defenderWasAttacked = true;
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

                partySlotHandler.MoveToActivePlayer(currentDefender, true);

                Debug.Log($"{enemy.Name} attacks {target.Name}, but is blocked by {currentDefender.Name}!" +
                        $"\nDamage Negated: {damageNegated}" +
                        $"\n{currentDefender.Name} takes {defenderDamage}, {target.Name} takes {targetDamage}");

                yield return new WaitForSecondsRealtime(.2f);

                foreach (PartySlot mem in partySlotHandler.partySlots)
                {
                    if (mem.isCharacter) {
                        if (mem.playerStats.Name == currentDefender.Name) {
                            mem.ShowHealthChange();
                            ShowFloatingText(defenderDamage, Color.blue, mem.transform.position, false);
                            StartCoroutine(mem.JutterHealthBar(0.2f, 10f));
                        }
                    }
                }
                foreach (PartySlot mem in partySlotHandler.partySlots)
                {
                    if (mem.isCharacter && mem.playerStats.Name == target.Name)
                    {
                        mem.ShowHealthChange();
                        ShowFloatingText(targetDamage, Color.red, mem.transform.position, false);
                        StartCoroutine(mem.JutterHealthBar(0.2f, 10f));
                    }
                }

                // Check if defender is defeated
                if (currentDefender.currentHealth <= 0)
                {
                    Debug.Log($"Defender {currentDefender.Name} has been defeated!");

                    // Clear the turn indicator
                    int indicatorIndex = battleOrder.IndexOf(currentDefender);
                    turnIndicator.ClearCharAtIndexIndicator(indicatorIndex);

                    // Move the defend indicator out of the turn slot before being deleted
                    //StartCoroutine(DestroyDefend());

                    // Remove from other lists
                    defeatedInCombat.Add(currentDefender.Name);
                    battleOrder.Remove(currentDefender);
                    playerParty.Remove(currentDefender);

                    if (currentDefender.Name != "Me") {
                        AudioManager.Instance.PlaySound(audioClips.sfxBell);
                        battleTransition.teammMateDeath(partyManager.currentPartyMembers.Find(x => x.Name == currentDefender.Name));
                    }

                    partyManager.removeFromPartyByName(currentDefender.Name);
                    StartCoroutine(DestroyDefend());

                    currentDefender = null;
                }
            }
            else // No defender, target takes full damage
            {
                bool splashhit = false;
                if (enemyStats.Name == "Hector") {
                    int rngAttack = Random.Range(0, playerParty.Count);
                    if (rngAttack > 1) {
                        foreach(CharacterStats member in playerParty) {
                            member.currentHealth -= enemyDamage / 2;
                            partySlotHandler.MoveToActivePlayer(member, true);

                            Debug.Log($"{enemy.Name} attacks all for {enemyDamage/2} damage!");
                            battleExplanation.text = "Hector hits all party members!";
                        }
                        splashhit = true;

                    } else {
                        target.currentHealth -= enemyDamage;
                        partySlotHandler.MoveToActivePlayer(target, true);
                        Debug.Log($"{enemy.Name} attacks {target.Name} for {enemyDamage} damage!");

                    }
                } else {
                    target.currentHealth -= enemyDamage;
                    partySlotHandler.MoveToActivePlayer(target, true);
                    Debug.Log($"{enemy.Name} attacks {target.Name} for {enemyDamage} damage!");

                }


                yield return new WaitForSecondsRealtime(.2f);


                if (splashhit) {
                    foreach (PartySlot mem in partySlotHandler.partySlots) {
                        if (mem.isCharacter) {

                                mem.ShowHealthChange();
                                ShowFloatingText(enemyDamage, Color.red, mem.transform.position, false);
                                StartCoroutine(mem.JutterHealthBar(0.2f, 10f));

                        }
                    }


                } else {

                    foreach (PartySlot mem in partySlotHandler.partySlots) {
                        if (mem.isCharacter) {
                            if (mem.playerStats.Name == target.Name) {
                                mem.ShowHealthChange();
                                ShowFloatingText(enemyDamage, Color.red, mem.transform.position, false);
                                StartCoroutine(mem.JutterHealthBar(0.2f, 10f));
                            }
                        }
                    }
                }

            }


            // Check if target is defeated
            if (!defenderWasAttacked) {
                foreach (CharacterStats person in new List<CharacterStats>(playerParty)) {

                    if (person.currentHealth <= 0) {
                        Debug.Log($"Target {person.Name} has been defeated!");
                        // Remove from turn indicator
                        int indicatorIndex = battleOrder.IndexOf(person);
                        turnIndicator.ClearCharAtIndexIndicator(indicatorIndex);

                        // Remove from other lists
                        defeatedInCombat.Add(person.Name);
                        battleOrder.Remove(person);
                        playerParty.Remove(person);

                        if (person.Name != "Me") {
                            battleTransition.teammMateDeath(partyManager.currentPartyMembers.Find(x => x.Name == person.Name));
                            AudioManager.Instance.PlaySound(audioClips.sfxBell);
                        }
                        // Remove from party manager after transition
                        partyManager.removeFromPartyByName(person.Name);
                        partySlotHandler.UpdateSlots();
                    }
                }
            }

                // Reset defender at the end of the turn
                if (currentDefender != null) StartCoroutine(DestroyDefend());
            currentDefender = null;
            currentTurnIndex = currentTurnIndex%battleOrder.Count;
            EnemyIsAttacking(enemyStats.Name);
        }
        yield return new WaitForSecondsRealtime(.6f);


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
        if (combatUI == null) return;

        Vector3 spawnPosition = targetTransform + new Vector3(0, 60f, 0);
        floatingText = Instantiate(floatingTextPrefab, spawnPosition, Quaternion.identity, GameObject.FindGameObjectWithTag("Combat UI").transform);
        floatingText.SetActive(true);
        floatingText.GetComponent<DamageIndicator>().SetText(damage.ToString(), color);
        floatingText.GetComponent<DamageIndicator>().isHealing = ishealing;
        floatingText.GetComponent<DamageIndicator>().textMesh.color = color;
    }


    private bool CheckForBattleEnd() {
        if (escapeSuccessful) {
            endCause = "Escape";
            escapeSuccessful = false;
            return true;
        }

        bool playersAlive = battleOrder.Exists(c => !c.isEnemy);
        if (!playersAlive) {
            endCause = "Lose";
            return true;
        }

        if (!battleOrder.Exists(c => c.Name == "Me")) {
            endCause = "Lose";
            return true;
        }

        bool enemiesAlive = battleOrder.Exists(c => c.isEnemy);
        if (!enemiesAlive) {
            EnemyDefeated(enemyStats.Name);
            endCause = "Win";
            return true;
        }

        return false;
    }

    // Called when the battle should end. Use to transition back to overworld
    private void EndEncounter(string reason) {
        // Clean up
        if (partyUIAnimator != null) {
            partyUIAnimator.ResetTrigger("Open");
            partyUIAnimator.ResetTrigger("Close");
            partyUIAnimator.ResetTrigger("Reset");
            if (itemOption || actOption) {
                partyUIAnimator.SetTrigger("Close");
                actOption = itemOption = false;
            }
        }
        if (floatingText != null) {
            Destroy(floatingText);
        }
        actOptionBList.SetActive(actOption);
        itemOptionBList.SetActive(itemOption);
        turnIndicator.ClearTurnIndicators();


        if (reason == "Win" || reason == "Escape") {
            // Restart stuff
            overworldUI.SetActive(true);
            combatUI.SetActive(false);
            Canvas.ForceUpdateCanvases();
            // Switch back to original sounds
            AudioManager.Instance.CrossFadeAmbienceSound(audioClips.oldAmbience, 1f, 1f, 1f);
            AudioManager.Instance.CrossFadeMusicSound(audioClips.oldMusic, 1f, 1f, 1f);

            battleInProgress = false;
            Time.timeScale = 1;
        }
        if (reason == "Win") {

            String item  = GameObject.FindGameObjectWithTag("Player").GetComponent<Inventory>().GrabRandomItem();
            battleExplanation.text = "You did it! You gain one "+item;
        } if (reason == "Escape") {
            Destroy(curEnemy);
        } else if (reason == "Lose") {
            Debug.Log("Game Over");
            battleExplanation.text = "Uh oh...";
            AudioManager.Instance.CrossFadeMusicToZero(0.5f, 0f);
            AudioManager.Instance.PlaySound(audioClips.battlePlayerDied);
        }

//         GameObject.FindWithTag("Player").GetComponent<Player>().isPlayerInControl = false;
    }

    // Used in death button. Reset current scene, restart at title screen
    public void ReturnToTitle() {
        // Reset current scene
        Debug.Log("Returning to the title");
        Time.timeScale = 1;
        AudioManager.Instance.RestartToDefault();
        SceneManager.LoadScene("Title", LoadSceneMode.Single);
    }

    public void OnActionButtonPressed(string action) {
        AudioManager.Instance.PlayUiSound(audioClips.uiSelected);

        if (selectedAction == "Heal" || selectedAction == "Defend"){
            selectedTarget = null;
            // canSelect = false;
        }

        selectedAction = action;
        partyUIAnimator.SetTrigger("Close");
        actOption = false;
        itemOption = false;
    }

    public void Act() {
        if (battleOrder[currentTurnIndex].isEnemy) {
            battleExplanation.text = "It is not your turn.";
        } else if (partyUIAnimator != null) {
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
    public void Item() {
        if (battleOrder[currentTurnIndex].isEnemy) {
            battleExplanation.text = "It is not your turn.";
        } if (partyUIAnimator != null) {
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
    private bool escapePressedOnce = false;
    private float escapeTimer = 0f,  escapeTimeout = 2f; // Time window for second press
    public void Escape() {
        if (battleOrder[currentTurnIndex].isEnemy) {
            battleExplanation.text = "It is not your turn.";
            AudioManager.Instance.PlayUiSound(audioClips.uiDrawer);
            return;
        }
        Debug.Log("Hey i get here");
        if (enemyStats.Name == "Hector") {
            Debug.Log("Hey i get here");
            battleExplanation.text = "You cannot run";
            return;
        }

        escapePrompt.OpenPrompt();
        if (!escapePressedOnce)
        {
            escapePressedOnce = true;
            escapeTimer = Time.time; // Start the timer
            return; // Wait for second press
        }

        // If too much time passed, reset and require another double press
        if (Time.time - escapeTimer > escapeTimeout)
        {
            escapePressedOnce = true;
            escapeTimer = Time.time;
            return;
        }

        selectedAction = "Escape";
        AudioManager.Instance.PlayUiSound(audioClips.uiDrawer);

        SetEscapePercentage();
        // float totalCurrentHealth = 0f, totalMaxHealth = 0f;
        // int partyCount = 0;

        // foreach (CharacterStats partyMember in battleOrder)
        // {
        //     if (!partyMember.isEnemy)
        //     {
        //         partyCount++;
        //         totalCurrentHealth += partyMember.currentHealth;
        //         totalMaxHealth += partyMember.maxHealth;
        //     }
        // }
        // escapeChance = (int)(35+(((totalMaxHealth-totalCurrentHealth)/totalMaxHealth)*65));
    }
    public void TryEscape() {
        roll = Random.Range(0, 100);


        Debug.Log($"Chance of escape: {escapeChance}%");

        if (roll<= escapeChance) {
            Debug.Log($"Wow, rolled a {roll}, you made it!!!");
            endTurn = true;
            escapeSuccessful = true;
            battleExplanation.text = "You got away!";
        } else {
            Debug.Log($"Oof, rolled a {roll}, didn't make it lol");
            SkipTurns();
            escapeSuccessful = false;
            battleExplanation.text = "Couldn't get away :(";
        }
        // Reset confirmation state after execution
        escapePressedOnce = false;

        escapePrompt.ClosePrompt();
        selectedAction = "Escape";
    }
    void SkipTurns()
    {
        int enemyIndex = battleOrder.IndexOf(enemyStats);
        endTurn = true;
        currentTurnIndex = enemyIndex-1;
    }
    void SetEscapePercentage()
    {
        float totalCurrentHealth = 0f, totalMaxHealth = 0f;
        int partyCount = 0;

        foreach (CharacterStats partyMember in playerParty)
        {
            // if (!partyMember.isEnemy)
            {
                partyCount++;
                totalCurrentHealth += partyMember.currentHealth;
                totalMaxHealth += partyMember.maxHealth;
            }
        }
        escapeChance = (int)(35 + (((totalMaxHealth-totalCurrentHealth)/totalMaxHealth)*65));
        if (enemyStats.Name == "Hector") {
            escapeChance = -99999;
        }
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
