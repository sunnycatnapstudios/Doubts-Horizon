using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.EventSystems;

public class SideBarStats : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    private GameStatsManager gameStatsManager;
    private _PartyManager _partyManager;
    
    public RectTransform imageRectTransform;
    private Vector2 defaultSize = new Vector2(35, 35), expandedSize = new Vector2(45, 45);
    private float expandSpeed = 10f;
    private Coroutine sizeCoroutine;



    IEnumerator WaitForPartyManager()
    {
        while (GameStatsManager.Instance == null || GameStatsManager.Instance._partyManager == null)
        {
            yield return null; // Wait until it's ready
        }

        gameStatsManager = GameStatsManager.Instance;
        _partyManager = gameStatsManager._partyManager;
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        if (this.name == "Player") return; // Prevent animation for Player

        if (sizeCoroutine != null) StopCoroutine(sizeCoroutine);
        sizeCoroutine = StartCoroutine(AnimateSize(expandedSize));

        CancelInvoke(nameof(DelayedShrink));
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        if (this.name == "Player") return; // Prevent animation for Player

        // if (sizeCoroutine != null) StopCoroutine(sizeCoroutine);
        // sizeCoroutine = StartCoroutine(AnimateSize(defaultSize));
        Invoke(nameof(DelayedShrink), 0.2f);
    }

    private void DelayedShrink()
    {
        if (sizeCoroutine != null) StopCoroutine(sizeCoroutine);
        sizeCoroutine = StartCoroutine(AnimateSize(defaultSize));
    }

    IEnumerator AnimateSize(Vector2 targetSize)
    {
        while (Vector2.Distance(imageRectTransform.sizeDelta, targetSize) > 0.1f)
        {
            imageRectTransform.sizeDelta = Vector2.Lerp(imageRectTransform.sizeDelta, targetSize, Time.deltaTime * expandSpeed);
            yield return null;
        }
        imageRectTransform.sizeDelta = targetSize;
    }

    void Awake()
    {
        // gameStatsManager = GameStatsManager.Instance;
        // _partyManager = GameStatsManager.Instance._partyManager;
        StartCoroutine(WaitForPartyManager());
        imageRectTransform = transform.GetComponent<Image>().rectTransform;
    }

    void Update()
    {
        if (this.name == "Player" && gameStatsManager != null && _partyManager != null)
        {
            CharacterStats member = gameStatsManager.playerStats["Player"];
            this.transform.Find("Health").GetComponent<TMP_Text>().text = $"|{member.currentHealth}/{member.maxHealth}";
        } else if (gameStatsManager != null && _partyManager != null)
        {
            CharacterStats member = gameStatsManager.currentPartyMembers.Find(partymember => partymember.Name == this.transform.Find("Name").GetComponent<TMP_Text>().text);
            this.transform.Find("Health").GetComponent<TMP_Text>().text = $"{member.currentHealth}/{member.maxHealth}";
        }
    }
}