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
    
    public RectTransform imageRectTransform, healthBarTransform;
    private Vector2 defaultSize = new Vector2(35, 35), expandedSize = new Vector2(40, 40);
    private Vector2 defaultPosition = new Vector2(-75, 0), expandedPosition;
    private float expandSpeed = 10f;
    private Coroutine imageCoroutine;
    private Coroutine healthBarCoroutine;



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

        if (imageCoroutine != null) StopCoroutine(imageCoroutine);
        imageCoroutine = StartCoroutine(AnimateSize(expandedSize));

        if (healthBarCoroutine != null) StopCoroutine(healthBarCoroutine);
        healthBarCoroutine = StartCoroutine(AnimateHealthbar(expandedPosition));

        CancelInvoke(nameof(DelayedShrink));
        CancelInvoke(nameof(DelayedHealthbar));
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        if (this.name == "Player") return; // Prevent animation for Player

        // if (imageCoroutine != null) StopCoroutine(imageCoroutine);
        // imageCoroutine = StartCoroutine(AnimateSize(defaultSize));
        Invoke(nameof(DelayedShrink), 0.2f);
        Invoke(nameof(DelayedHealthbar), 0.2f);
    }

    private void DelayedShrink()
    {
        if (imageCoroutine != null) StopCoroutine(imageCoroutine);
        imageCoroutine = StartCoroutine(AnimateSize(defaultSize));
    }
    private void DelayedHealthbar()
    {
        if (healthBarCoroutine != null) StopCoroutine(healthBarCoroutine);
        healthBarCoroutine = StartCoroutine(AnimateHealthbar(defaultPosition));
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
    IEnumerator AnimateHealthbar(Vector2 targetPos)
    {
        while (Vector2.Distance(healthBarTransform.anchoredPosition, targetPos) > 0.1f)
        {
            healthBarTransform.anchoredPosition = Vector2.Lerp(healthBarTransform.anchoredPosition, targetPos, Time.deltaTime * expandSpeed*2f);
            yield return null;
        }
        healthBarTransform.anchoredPosition = targetPos;
    }

    void Awake()
    {
        // gameStatsManager = GameStatsManager.Instance;
        // _partyManager = GameStatsManager.Instance._partyManager;
        StartCoroutine(WaitForPartyManager());
        imageRectTransform = transform.GetComponent<Image>().rectTransform;
        healthBarTransform = transform.Find("Health Bar Base").GetComponent<Image>().rectTransform;

        expandedPosition = healthBarTransform.anchoredPosition;
        if (this.name != "Player") {
            healthBarTransform.anchoredPosition = defaultPosition;
        }
    }

    void Update()
    {
        if (this.name == "Player" && gameStatsManager != null && _partyManager != null)
        {
            CharacterStats member = gameStatsManager.playerStats["Player"];
            this.transform.Find("Health Bar Base").Find("Health").GetComponent<TMP_Text>().text = $"{member.currentHealth}/{member.maxHealth}";
            this.transform.Find("Health Bar Base").Find("Healthbar").GetComponent<Image>().fillAmount = (float)member.currentHealth/member.maxHealth;
        } else if (gameStatsManager != null && _partyManager != null)
        {
            CharacterStats member = gameStatsManager.currentPartyMembers.Find(partymember => partymember.Name == this.transform.Find("Name").GetComponent<TMP_Text>().text);
            // this.transform.Find("Health").GetComponent<TMP_Text>().text = $"{member.currentHealth}/{member.maxHealth}";
            this.transform.Find("Health Bar Base").Find("Health").GetComponent<TMP_Text>().text = $"{member.currentHealth}/{member.maxHealth}";
            this.transform.Find("Health Bar Base").Find("Healthbar").GetComponent<Image>().fillAmount = (float)member.currentHealth/member.maxHealth;
        }
    }
}