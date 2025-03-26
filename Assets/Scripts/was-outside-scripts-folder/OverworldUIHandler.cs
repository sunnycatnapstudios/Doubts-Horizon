using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OverworldUIHandler : MonoBehaviour {
    private GameStatsManager gameStatsManager;
    private _PartyManager _partyManager;
    private _DialogueHandler _dialogueHandler;
    private _BattleUIHandler _battleUIHandler;

    public Player player;
    public GameObject overworldUI, combatUI, overworldMenu, combatMenu;
    public Animator overworldMenuAnimator, combatMenuAnimator, overworldDarkScreen, combatDarkScreen;
    public bool isPaused, overworldUIActive, combatUIActive;
    public Animator currentDarkScreen;

    void Awake() {
        StartCoroutine(WaitForPartyManager());
        player = GameObject.FindGameObjectWithTag("Player").GetComponent<Player>();
    }
    IEnumerator WaitForPartyManager() {
        while (GameStatsManager.Instance == null || GameStatsManager.Instance._partyManager == null) {
            yield return null; // Wait until it's ready
        }

        gameStatsManager = GameStatsManager.Instance;
        _partyManager = gameStatsManager._partyManager;
        _dialogueHandler = gameStatsManager._dialogueHandler;
        _battleUIHandler = gameStatsManager._battleUIHandler;
    }

    void CheckOpenMenu() {
        if (Input.GetKeyDown(KeyCode.Tab) && player.canControlCam) { Open_CloseMenu(isPaused); }
    }
    public void Open_CloseMenu(bool ispaused) {
        isPaused = !ispaused;
        // Time.timeScale = isPaused ? 0 : 1;
        if (isPaused) {
            overworldMenuAnimator.Play("OpenMenu");
            if (overworldUI != null && overworldUI.activeSelf) {
                overworldDarkScreen.Play("Darken Screen");
                Time.timeScale = 0;
            } else if (combatUI != null && combatUI.activeSelf) {
                combatDarkScreen.Play("Darken Screen");
            }
        } else {
            overworldMenuAnimator.Play("CloseMenu");
            if (overworldUI != null && overworldUI.activeSelf) {
                overworldDarkScreen.Play("Lighten Screen");
                Time.timeScale = 1;
            } else if (combatUI != null && combatUI.activeSelf) {
                combatDarkScreen.Play("Lighten Screen");
            }
        }
    }

    void Update() {
        if (combatUI == null) combatUI = GameObject.FindGameObjectWithTag("Combat UI");
        if (overworldUI == null) overworldUI = GameObject.FindGameObjectWithTag("Overworld UI");
        if (overworldMenu == null) overworldMenu = GameObject.FindGameObjectWithTag("OverworldMenu");
        if (combatMenu == null) combatMenu = GameObject.FindGameObjectWithTag("CombatMenu");

        if (combatUI != null && combatUI.activeSelf) {
            // combatUIActive = combatUI.activeSelf;
            combatDarkScreen = GameObject.FindGameObjectWithTag("Dark Screen").GetComponent<Animator>();
            combatDarkScreen.updateMode = AnimatorUpdateMode.UnscaledTime;
        }
        if (overworldUI != null && overworldUI.activeSelf) {
            // overworldUIActive = overworldUI.activeSelf;
            overworldDarkScreen = GameObject.FindGameObjectWithTag("Dark Screen").GetComponent<Animator>();
            overworldDarkScreen.updateMode = AnimatorUpdateMode.UnscaledTime;
            overworldMenuAnimator = overworldMenu.GetComponent<Animator>();
            overworldMenuAnimator.updateMode = AnimatorUpdateMode.UnscaledTime;
        }

        CheckOpenMenu();
    }
}
