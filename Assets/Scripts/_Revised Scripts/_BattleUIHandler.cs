using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class _BattleUIHandler : MonoBehaviour
{
    private GameStatsManager gameStatsManager;
    private _PartyManager _partyManager;
    
    public Animator partyUIAnimator, enemyUIAnimator, enemyStatsAnimator;
    public bool actOption = false, itemOption = false, canSelect = false;
    public GameObject overworldUI, combatUI, actOptionBList, itemOptionBList, enemySlot, floatingTextPrefab;
    public List<GameObject> partySlots = new List<GameObject>();
    public List<CharacterStats> battleOrder = new List<CharacterStats>();
    public int currentTurnIndex = 0;
    private bool battleInProgress = false;
    public List<CharacterStats> currentEnemies = new List<CharacterStats>();
    public List<Sprite> profileImages;
    public TurnIndicator turnIndicator;
    public TextMeshProUGUI enemyName, damageButtonText;
    private string selectedAction = "", selectedTarget = null;
    CharacterStats enemyStats;

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
        
    }

    void Update()
    {
        
    }
}
