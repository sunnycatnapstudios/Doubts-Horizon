using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PartySlotHandler : MonoBehaviour {
    private GameStatsManager gameStatsManager;
    public PartyManager _partyManager;
    public _BattleUIHandler _battleUIHandler;

    public HorizontalLayoutGroup horizLayoutGroup;
    public CanvasGroup ViewPortCanvasGroup;
    private RectTransform horizRectTransform;
    private int slotCount;
    public GameObject partySlotPrefab;
    public Scrollbar scrollbar;

    public List<CharacterStats> playerParty = new List<CharacterStats>();
    public List<PartySlot> partySlots = new List<PartySlot>();
    public List<Survivor> survivors = new List<Survivor>();

    private Vector2 baseRectSize = new Vector2(430, 130);
    private Vector2 baseRectPosition = new Vector2(-215, 65);


    void Start() {
        //     gameStatsManager = GameStatsManager.Instance;
        //     _partyManager = GameStatsManager.Instance.GetComponentInChildren<_PartyManager>();
        //     _battleUIHandler = GameStatsManager.Instance.GetComponentInChildren<_BattleUIHandler>();
        StartCoroutine(WaitForPartyManager());
    }
    IEnumerator WaitForPartyManager() {
        while (GameStatsManager.Instance == null || GameStatsManager.Instance.partyManager == null || GameStatsManager.Instance._battleUIHandler == null) {
            yield return null; // Wait until it's ready
        }

        gameStatsManager = GameStatsManager.Instance;
        _partyManager = gameStatsManager.partyManager;
        _battleUIHandler = gameStatsManager._battleUIHandler;

        UpdateSlots();
    }
    void OnEnable() {
        horizLayoutGroup = this.GetComponentInChildren<HorizontalLayoutGroup>();
        if (horizLayoutGroup != null)
            horizRectTransform = horizLayoutGroup.GetComponent<RectTransform>();

        // scrollbar = GetComponentInChildren<Scrollbar>();

        // StartCoroutine(WaitForPartyManager());
    }

    public void UpdateSlots() {
        survivors = _battleUIHandler.survivors;
        Debug.Log($"{playerParty.Count}");

        // Destroy old slots
        foreach (Transform child in horizRectTransform.transform) {
            if (!child.gameObject.activeSelf) continue;
            Destroy(child.gameObject);
        }
        partySlots.Clear();
        slotCount = 0;

        // Instantiate new slots
        foreach (Survivor member in _partyManager.currentPartyMembers) {
            GameObject newSlot = Instantiate(partySlotPrefab, horizRectTransform.transform);
            newSlot.SetActive(true);
            newSlot.name = "PartySlot" + (partySlots.Count + 1);
            PartySlot slotComponent = newSlot.GetComponent<PartySlot>();
            slotCount++;
            Debug.Log("anderler" + member.ToString());

            if (slotComponent != null) {
                slotComponent.Initialize(member);
                partySlots.Add(slotComponent);
            }
        }
        if (slotCount < 4) {
            for (int currentSlots = slotCount; currentSlots < 4; currentSlots++) {
                GameObject newSlot = Instantiate(partySlotPrefab, horizRectTransform.transform);
                newSlot.SetActive(true);
                newSlot.name = "FillerSlot" + (currentSlots + 1);
                PartySlot slotComponent = newSlot.GetComponent<PartySlot>();
                slotCount++;
                slotComponent.isCharacter = false;
                // Filler slots don't need to initialize CharacterStats
                partySlots.Add(slotComponent);
            }
        }

        // Update layout based on new slot count
        AdjustSize(slotCount);
    }
    public void AdjustSize(int slotcount) {
        if (slotcount > 4) {
            horizRectTransform.sizeDelta = new Vector2(slotcount * 108, baseRectSize.y);
            // horizRectTransform.anchoredPosition = new Vector2(baseRectPosition.x+1000, baseRectPosition.y);
        } else {
            horizRectTransform.sizeDelta = new Vector2(100, baseRectSize.y);
            // horizRectTransform.anchoredPosition = new Vector2(baseRectPosition.x+1000, baseRectPosition.y);
        }
    }

    public int GetPlayerIndex(CharacterStats player) {
        for (int i = 0; i < playerParty.Count; i++) {
            if (playerParty[i] == player) {
                return i;
            }
        }
        return -1;  // Player not found
    }
    public void MoveToActivePlayer(CharacterStats activePlayer, bool dontBob) {
        int playerIndex = GetPlayerIndex(activePlayer);
        playerParty = _battleUIHandler.playerParty;

        if (playerIndex != -1 && scrollbar != null && playerParty.Count > 1 && this.gameObject.activeSelf) {
            float targetPosition = Mathf.Clamp01((float)playerIndex / (playerParty.Count - 1));
            StartCoroutine(SmoothScroll(targetPosition, .3f));

            if (!dontBob && playerParty.Count > 4) {
                StartCoroutine(BobSlot(partySlots[playerIndex].GetComponentInChildren<Image>().transform.GetComponent<RectTransform>(), 7.5f, 0.25f));
            }
        }
    }
    private IEnumerator SmoothScroll(float targetValue, float duration) {
        float startValue = scrollbar.value;
        float time = 0f;

        while (time < duration) {
            time += Time.unscaledDeltaTime; // Use unscaledDeltaTime to keep it smooth even when paused
            float t = time / duration;
            t = t * t * (3f - 2f * t); // Lil "Smoothstep" trick I picked up from a friend

            scrollbar.value = Mathf.Lerp(startValue, targetValue, t);
            yield return null; // Wait for the next frame
        }

        scrollbar.value = targetValue;
    }
    private IEnumerator BobSlot(RectTransform slot, float bobHeight, float duration) {
        Vector2 startPosition = slot.anchoredPosition;
        Vector2 targetPosition = startPosition + new Vector2(0, bobHeight);
        Vector2 endPosition = startPosition + new Vector2(0, 5);

        float time = 0f;

        yield return new WaitForSecondsRealtime(.3f);

        while (time < duration) {
            time += Time.unscaledDeltaTime;
            float t = time / duration;
            float offset = Mathf.Sin(t * Mathf.PI) * bobHeight;

            slot.anchoredPosition = startPosition + new Vector2(0, offset);
            yield return null;
        }
        slot.anchoredPosition = startPosition;
        // slot.anchoredPosition = startPosition;

        // while (time < duration / 2)
        // {
        //     time += Time.unscaledDeltaTime;
        //     float t = time / duration;
        //     // t = t * t * (3f - 2f * t);
        //     float offset = Mathf.Lerp(0, bobHeight, t);

        //     slot.anchoredPosition = startPosition + new Vector2(0, offset);
        //     yield return null;
        // }
        // slot.anchoredPosition = targetPosition;
        // time = 0;
        // while (time < duration / 2)
        // {
        //     time += Time.unscaledDeltaTime;
        //     float t = time / (duration / 2);
        //     // t = t * t * (3f - 2f * t);
        //     float offset = Mathf.Lerp(bobHeight, 5, t);

        //     slot.anchoredPosition = startPosition + new Vector2(0, offset);
        //     yield return null;
        // }
        // slot.anchoredPosition = endPosition;
    }

    void Update() {

        // if (horizRectTransform == null) return;

    }
}
