using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PartySlotHandler : MonoBehaviour
{
    private GameStatsManager gameStatsManager;
    private _PartyManager _partyManager;
    private _BattleUIHandler _battleUIHandler;

    public HorizontalLayoutGroup horizLayoutGroup;
    private RectTransform horizRectTransform;
    private int previousChildCount = -1, slotCount;
    public GameObject partySlotPrefab;
    private Scrollbar scrollbar;

    public List<CharacterStats> playerParty = new List<CharacterStats>();
    public List<PartySlot> partySlots = new List<PartySlot>();

    private Vector2 baseRectSize = new Vector2(430, 130);
    private Vector2 baseRectPosition = new Vector2(-215, 65);
    public float sizeIncrement = 50f;
    
    void OnEnable()
    {
        gameStatsManager = GameStatsManager.Instance;
        _partyManager = GameStatsManager.Instance.GetComponentInChildren<_PartyManager>();
        _battleUIHandler = GameStatsManager.Instance.GetComponentInChildren<_BattleUIHandler>();
        
        horizLayoutGroup = this.GetComponentInChildren<HorizontalLayoutGroup>();
        if (horizLayoutGroup != null)
            horizRectTransform = horizLayoutGroup.GetComponent<RectTransform>();
        
        scrollbar = this.GetComponentInChildren<Scrollbar>();

        UpdateSlots();
    }

    public void UpdateSlots()
    {
        playerParty = _battleUIHandler.playerParty;
        Debug.Log($"{playerParty.Count}");

        // Destroy old slots
        foreach (Transform child in horizRectTransform.transform)
        {
            if (!child.gameObject.activeSelf) continue;
            Destroy(child.gameObject);
        }
        partySlots.Clear();
        slotCount = 0;

        // Instantiate new slots
        foreach (CharacterStats member in playerParty)
        {
            GameObject newSlot = Instantiate(partySlotPrefab, horizRectTransform.transform);
            newSlot.SetActive(true);
            newSlot.name = "PartySlot"+(partySlots.Count+1);
            PartySlot slotComponent = newSlot.GetComponent<PartySlot>();
            slotCount++;

            if (slotComponent != null)
            {
                slotComponent.Initialize(member);
                partySlots.Add(slotComponent);
            }
        }
        if (slotCount < 4)
        {
            for (int currentSlots = slotCount; currentSlots < 4; currentSlots++)
            {
                GameObject newSlot = Instantiate(partySlotPrefab, horizRectTransform.transform);
                newSlot.SetActive(true);
                newSlot.name = "FillerSlot" + (currentSlots + 1);
                PartySlot slotComponent = newSlot.GetComponent<PartySlot>();
                slotCount++;

                // Filler slots don't need to initialize CharacterStats
                partySlots.Add(slotComponent);
            }
        }

        // Update layout based on new slot count
        AdjustSize(slotCount);
    }
    public void AdjustSize(int childCount)
    {
        horizRectTransform.sizeDelta = new Vector2(childCount*108, baseRectSize.y);
        horizRectTransform.anchoredPosition = new Vector2(baseRectPosition.x+1000, baseRectPosition.y);
        Debug.Log("Adjusting size with child count: " + childCount);
    }

    public int GetPlayerIndex(CharacterStats player)
    {
        for (int i = 0; i < playerParty.Count; i++)
        {
            if (playerParty[i] == player)
            {
                return i;
            }
        }
        return -1;  // Player not found
    }
    public void MoveToActivePlayer(CharacterStats activePlayer)
    {
        int playerIndex = GetPlayerIndex(activePlayer);
        if (playerIndex != -1 && scrollbar != null)
        {
            float targetPosition = Mathf.Clamp01((float)playerIndex / (playerParty.Count - 1));
            StartCoroutine(SmoothScroll(targetPosition, .3f));
        }
    }

    private IEnumerator SmoothScroll(float targetValue, float duration)
    {
        float startValue = scrollbar.value;
        float time = 0f;

        while (time < duration)
        {
            time += Time.unscaledDeltaTime; // Use unscaledDeltaTime to keep it smooth even when paused
            float t = time / duration;
            t = t * t * (3f - 2f * t); // Lil "Smoothstep" trick I picked up from a friend

            scrollbar.value = Mathf.Lerp(startValue, targetValue, t);
            yield return null; // Wait for the next frame
        }

        scrollbar.value = targetValue;
    }

    void Update()
    {

        if (horizRectTransform == null) return;

        int currentChildCount = horizRectTransform.childCount;
        if (currentChildCount != previousChildCount)
        {
            AdjustSize(slotCount);
            previousChildCount = currentChildCount;
        }

    }
}
